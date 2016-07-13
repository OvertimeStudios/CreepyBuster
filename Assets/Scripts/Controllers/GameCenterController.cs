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
	public static int playerGlobalMaxRange;
	private static bool isSearchingPlayerGlobalPosition = false;

	public static int playerFriendsPosition;
	public static int playerFriendsMaxRange;
	private static bool isSearchingPlayerFriendsPosition = false;

	public static long playerScore;
	private static bool isSearchingPlayerScore = false;

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

	public static IEnumerator GetUserScore(System.Action<long> result)
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
		{
			isSearchingPlayerScore = true;

			GameCenterManager.scoresForPlayerIdLoadedEvent += OnPlayerScoreLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent += OnPlayerScoreFailed;
			GameCenterBinding.retrieveScoresForPlayerId(GameCenterBinding.playerIdentifier(), Instance.leaderboardID);

			while(isSearchingPlayerScore)
				yield return null;

			GameCenterManager.scoresForPlayerIdLoadedEvent -= OnPlayerScoreLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent -= OnPlayerScoreFailed;

			result(playerScore);
		}
		else
		{
			Debug.LogError("Player is not authenticated");

			result(0);
		}

		#endif
	}

	private static void OnPlayerScoreLoaded(GameCenterRetrieveScoresResult result)
	{
		playerScore = result.scores[0].value;

		isSearchingPlayerScore = false;
	}

	private static void OnPlayerScoreFailed(string errmsg)
	{
		playerScore = 0;

		isSearchingPlayerScore = false;

		Debug.Log("Error on Global Score: " + errmsg);
	}

	public static IEnumerator GetPlayerGlobalPosition(System.Action<int, int> result)
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
		{
			isSearchingPlayerGlobalPosition = true;

			GameCenterManager.scoresForPlayerIdLoadedEvent += OnPlayerGlobalScoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent += OnPlayerGlobalScoresFailed;
			GameCenterBinding.retrieveScoresForPlayerId(GameCenterBinding.playerIdentifier(), Instance.leaderboardID);

			while(isSearchingPlayerGlobalPosition)
				yield return null;

			GameCenterManager.scoresForPlayerIdLoadedEvent -= OnPlayerGlobalScoresLoaded;
			GameCenterManager.retrieveScoresForPlayerIdFailedEvent -= OnPlayerGlobalScoresFailed;

			result(playerGlobalPosition, playerGlobalMaxRange);
		}
		else
		{
			Debug.LogError("Player is not authenticated");

			result(0, 0);
		}

		#endif
	}

private static void OnPlayerGlobalScoresLoaded(GameCenterRetrieveScoresResult result)
{
	Debug.Log(string.Format("Did recieved OnPlayerGlobalScoresLoaded. Total results: {0}: \n" +
		"alias: {1}; \n" +
		"id: {2}; \n" +
		"rank: {3} \n" +
		"maxRange: {4}", result.scores.Count, result.scores[0].alias, result.scores[0].playerId, result.scores[0].rank, result.scores[0].maxRange));
	playerGlobalPosition = result.scores[0].rank;
	playerGlobalMaxRange = result.scores[0].maxRange;

	isSearchingPlayerGlobalPosition = false;
}

private static void OnPlayerGlobalScoresFailed(string errmsg)
{
	playerGlobalPosition = 0;

	isSearchingPlayerGlobalPosition = false;

	Debug.Log("Error on Global Score: " + errmsg);
}

	public static IEnumerator GetPlayerFriendsPosition(System.Action<int, int> result)
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
		{
			Debug.Log("GameCenterController.GetPlayerFriendsPosition");
			isSearchingPlayerFriendsPosition = true;

			GameCenterManager.scoresLoadedEvent += OnPlayerFriendsScoresLoaded;
			GameCenterManager.retrieveScoresFailedEvent += OnPlayerFriendsScoresFailed;
			GameCenterBinding.retrieveScores(true, GameCenterLeaderboardTimeScope.AllTime, 1, 100, Instance.leaderboardID);

			while(isSearchingPlayerFriendsPosition)
				yield return null;

			Debug.Log("Finish search");

			GameCenterManager.scoresLoadedEvent -= OnPlayerFriendsScoresLoaded;
			GameCenterManager.retrieveScoresFailedEvent -= OnPlayerFriendsScoresFailed;

			result(playerFriendsPosition, playerFriendsMaxRange);
		}
		else
		{
			Debug.LogError("Player is not authenticated");
			result(0, 0);
		}

		#endif
	}

	private static void OnPlayerFriendsScoresLoaded(GameCenterRetrieveScoresResult result)
	{
		foreach(GameCenterScore gcScore in result.scores)
		{
			if(gcScore.playerId == GameCenterBinding.playerIdentifier())
				playerFriendsPosition = gcScore.rank;
		}

		playerFriendsMaxRange = result.scores.Count;
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
		#endif
	}

	public static void ShowLeaderboards(string _leaderboardID)
	{
		#if GAMECENTER_IMPLEMENTED
		if(GameCenterBinding.isPlayerAuthenticated())
			GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime);
		#endif
	}
		
	public static void SendScore(long score, string leaderboardID)
	{
		Debug.Log("Sending score: " + score);
		#if GAMECENTER_IMPLEMENTED
		GameCenterBinding.reportScore(score, leaderboardID);
		#endif
	}
}
