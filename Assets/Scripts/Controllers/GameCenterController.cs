using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if GAMECENTER_IMPLEMENTED
using Prime31;
#endif

public class GameCenterController : MonoBehaviour 
{
	#region EVENTS
	public static Action OnPlayerAuthenticated;
	#endregion

	public static int playerGlobalPosition;
	private static bool isSearchingPlayerGlobalPosition = false;

	public static int playerFriendsPosition;
	private static bool isSearchingPlayerFriendsPosition = false;

	public string leaderboardID;

	#region singleton
	private static GameCenterController instance;
	public static GameCenterController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<GameCenterController>();

			return instance;
		}
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		#if GAMECENTER_IMPLEMENTED
		Debug.Log("Authenticating Local Player");
		GameCenterManager.playerAuthenticatedEvent += OnPlayerAthenticated;

		GameCenterBinding.authenticateLocalPlayer();
		#endif
	}

	private static void OnPlayerAthenticated()
	{
		Debug.Log("Player successfully authenticated");

		if(OnPlayerAuthenticated != null)
			OnPlayerAuthenticated();

		GameCenterBinding.retrieveFriends( true, true );
		GameCenterBinding.loadLeaderboardTitles();
	}

	public static void AuthenticatePlayer()
	{
		if(!GameCenterBinding.isPlayerAuthenticated())
			GameCenterBinding.authenticateLocalPlayer();
	}

	public static bool IsPlayerAuthenticated()
	{
		#if GAMECENTER_IMPLEMENTED
		return GameCenterBinding.isPlayerAuthenticated();
		#endif
	}

	public static IEnumerator GetPlayerGlobalPosition(System.Action<int> result)
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
		{
			isSearchingPlayerGlobalPosition = true;

			GameCenterManager.scoresLoadedEvent += OnPlayerGlobalScoresLoaded;
			GameCenterManager.retrieveScoresFailedEvent += OnPlayerGlobalScoresFailed;
			GameCenterBinding.retrieveScores(false, GameCenterLeaderboardTimeScope.AllTime, 1, 1, Instance.leaderboardID);

			while(isSearchingPlayerGlobalPosition)
				yield return null;

			GameCenterManager.scoresLoadedEvent -= OnPlayerGlobalScoresLoaded;
			GameCenterManager.retrieveScoresFailedEvent -= OnPlayerGlobalScoresFailed;

			result(playerGlobalPosition);
		}
		else
		{
			Debug.LogError("Player is not authenticated");

			result(0);
		}

		#endif
	}

	private static void OnPlayerGlobalScoresLoaded(GameCenterRetrieveScoresResult result)
	{
		playerGlobalPosition = result.scores[0].rank;

		isSearchingPlayerGlobalPosition = false;
	}

	private static void OnPlayerGlobalScoresFailed(string errmsg)
	{
		playerGlobalPosition = 0;

		isSearchingPlayerGlobalPosition = false;

		Debug.Log("Error on Global Score: " + errmsg);
	}

	public static IEnumerator GetPlayerFriendsPosition(System.Action<int> result)
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
		{
			Debug.Log("GameCenterController.GetPlayerFriendsPosition");
			isSearchingPlayerFriendsPosition = true;

			GameCenterManager.scoresLoadedEvent += OnPlayerFriendsScoresLoaded;
			GameCenterManager.retrieveScoresFailedEvent += OnPlayerFriendsScoresFailed;
			GameCenterBinding.retrieveScores(true, GameCenterLeaderboardTimeScope.AllTime, 1, 1, Instance.leaderboardID);

			while(isSearchingPlayerFriendsPosition)
				yield return null;

			Debug.Log("Finish search");

			GameCenterManager.scoresLoadedEvent -= OnPlayerFriendsScoresLoaded;
			GameCenterManager.retrieveScoresFailedEvent -= OnPlayerFriendsScoresFailed;

			result(playerFriendsPosition);
		}
		else
		{
			Debug.LogError("Player is not authenticated");
			result(0);
		}

		#endif
	}

	private static void OnPlayerFriendsScoresLoaded(GameCenterRetrieveScoresResult result)
	{
		playerFriendsPosition = result.scores[0].rank;
		isSearchingPlayerFriendsPosition = false;
	}

	private static void OnPlayerFriendsScoresFailed(string errmsg)
	{
		playerGlobalPosition = 0;

		isSearchingPlayerFriendsPosition = false;

		Debug.Log("Error on Friends Score: " + errmsg);
	}

	public static void ShowLeaderboards()
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
			GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime );
		//GameCenterBinding.showGameCenterViewController( GameCenterViewControllerState.Leaderboards );
		#endif
	}
		
	public static void SendScore(long score, string leaderboardID)
	{
		GameCenterBinding.reportScore(score, leaderboardID);
	}
}
