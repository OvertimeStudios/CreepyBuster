using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if LEADERBOARDS_IMPLEMENTED && (UNITY_IOS || UNITY_ANDROID)
using Prime31;
#endif

/// <summary>
/// Leaderboards helper.
/// 
/// Add it to a Game Object on scene.
/// </summary>
public class LeaderboardsHelper : MonoBehaviour 
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

	public StoresID defaultLeaderboard;

	#region singleton
	private static LeaderboardsHelper instance;
	public static LeaderboardsHelper Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<LeaderboardsHelper>();

			return instance;
		}
	}
	#endregion

	void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;

		transform.parent = null;
		DontDestroyOnLoad(gameObject);

		#if ACHIEVEMENTS_IMPLEMENTED
			#if UNITY_IOS
			GameCenterManager.playerAuthenticatedEvent += OnPlayerAthenticated;
			#elif UNITY_ANDROID
			GPGManager.authenticationSucceededEvent += OnPlayerAthenticated;
			GPGManager.submitScoreSucceededEvent += SubmitScoreSuccess;
			GPGManager.submitScoreFailedEvent += SubmitScoreFailed;
			#endif
		#endif
	}

	// Use this for initialization
	void Start () 
	{
		#if LEADERBOARDS_IMPLEMENTED

			Debug.Log("Authenticating Local Player");

			Authenticate();

		#endif
	}

	public static void Authenticate()
	{
		#if UNITY_IOS
		GameCenterBinding.authenticateLocalPlayer();
		#elif UNITY_ANDROID
		PlayGameServices.authenticate();
		#endif
	}

	private static void OnPlayerAthenticated()
	{
		OnPlayerAthenticated("");
	}

	private static void OnPlayerAthenticated(string msg)
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			Debug.Log("Player successfully authenticated");

			GameCenterBinding.retrieveFriends( true, true );
			GameCenterBinding.loadLeaderboardTitles();

			#elif UNITY_ANDROID
			
			#endif

		#endif

		if(OnPlayerAuthenticated != null)
			OnPlayerAuthenticated();
		
	}

	public static bool IsPlayerAuthenticated()
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			return GameCenterBinding.isPlayerAuthenticated();
			#elif UNITY_ANDROID
			return PlayGameServices.isSignedIn();
			#endif

		#endif

		return false;
	}

	public static IEnumerator GetUserScore(System.Action<long> result)
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			if(GameCenterBinding.isPlayerAuthenticated())
			{
				isSearchingPlayerScore = true;

				GameCenterManager.scoresForPlayerIdLoadedEvent += OnPlayerScoreLoaded;
				GameCenterManager.retrieveScoresForPlayerIdFailedEvent += OnPlayerScoreFailed;
				GameCenterBinding.retrieveScoresForPlayerId(GameCenterBinding.playerIdentifier(), Instance.defaultLeaderboard.iOS);

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
			#elif UNITY_ANDROID
			yield return null;
			result(0);
			#endif

		#endif

		yield return null;
		result(0);
	}

	#if LEADERBOARDS_IMPLEMENTED && UNITY_IOS
	private static void OnPlayerScoreLoaded(GameCenterRetrieveScoresResult result)
	{
		playerScore = result.scores[0].value;

		isSearchingPlayerScore = false;
	}
	#endif

	private static void OnPlayerScoreFailed(string errmsg)
	{
		playerScore = 0;

		isSearchingPlayerScore = false;

		Debug.Log("Error on Global Score: " + errmsg);
	}

	public static IEnumerator GetPlayerGlobalPosition(System.Action<int, int> result)
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			if(GameCenterBinding.isPlayerAuthenticated())
			{
				isSearchingPlayerGlobalPosition = true;

				GameCenterManager.scoresForPlayerIdsLoadedEvent += OnPlayerGlobalScoresLoaded;
				GameCenterManager.retrieveScoresForPlayerIdsFailedEvent += OnPlayerGlobalScoresFailed;
				
				GameCenterBinding.retrieveScoresForPlayerIds(new string[1] { GameCenterBinding.playerIdentifier() }, Instance.defaultLeaderboard.iOS, false);
				//GameCenterBinding.retrieveScoresForPlayerId(GameCenterBinding.playerIdentifier(), Instance.defaultLeaderboard.iOS);

				while(isSearchingPlayerGlobalPosition)
					yield return null;

				GameCenterManager.scoresForPlayerIdsLoadedEvent -= OnPlayerGlobalScoresLoaded;
				GameCenterManager.retrieveScoresForPlayerIdsFailedEvent -= OnPlayerGlobalScoresFailed;

				result(playerGlobalPosition, playerGlobalMaxRange);
			}
			else
			{
				Debug.LogError("Player is not authenticated");

				result(0, 0);
			}
			#elif UNITY_ANDROID
			if(PlayGameServices.isSignedIn())
			{
				isSearchingPlayerGlobalPosition = true;

				GPGManager.loadCurrentPlayerLeaderboardScoreSucceededEvent += OnPlayerGlobalScoresLoaded;
				GPGManager.loadCurrentPlayerLeaderboardScoreFailedEvent += OnPlayerGlobalScoresFailed;
				PlayGameServices.loadCurrentPlayerLeaderboardScore(Instance.defaultLeaderboard.android, GPGLeaderboardTimeScope.AllTime, false);

				while(isSearchingPlayerGlobalPosition)
					yield return null;

				GPGManager.loadCurrentPlayerLeaderboardScoreSucceededEvent += OnPlayerGlobalScoresLoaded;
				GPGManager.loadCurrentPlayerLeaderboardScoreFailedEvent += OnPlayerGlobalScoresFailed;

				result(playerGlobalPosition, playerGlobalMaxRange);
			}
			else
			{
				Debug.LogError("Player is not authenticated");
				result(0, 0);
			}
			#endif

		#endif

		yield return null;
		result(0, 0);
	}

	#if LEADERBOARDS_IMPLEMENTED
	#if UNITY_IOS
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
	#elif UNITY_ANDROID

	private static void OnPlayerGlobalScoresLoaded(GPGScore score)
	{
		Debug.Log("OnPlayerGlobalScoresLoaded Sucess");

		playerGlobalPosition = playerFriendsPosition = (int)score.rank;
		playerGlobalMaxRange = playerFriendsMaxRange = 0;

		isSearchingPlayerGlobalPosition = false;
		isSearchingPlayerFriendsPosition = false;
	}


	private static void OnPlayerGlobalScoresFailed(string leaderboardID, string msg)
	{
		playerGlobalPosition = 0;
		playerGlobalMaxRange = 0;

		isSearchingPlayerGlobalPosition = false;
		isSearchingPlayerFriendsPosition = false;

		Debug.Log("Error on Global Score: " + msg);
	}
	#endif
	#endif

	public static IEnumerator GetPlayerFriendsPosition(System.Action<int, int> result)
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			if(GameCenterBinding.isPlayerAuthenticated())
			{
				Debug.Log("GameCenterController.GetPlayerFriendsPosition");
				isSearchingPlayerFriendsPosition = true;

				GameCenterManager.scoresForPlayerIdsLoadedEvent += OnPlayerFriendsScoresLoaded;
				GameCenterManager.retrieveScoresForPlayerIdsFailedEvent += OnPlayerFriendsScoresFailed;
				GameCenterBinding.retrieveScoresForPlayerIds(new string[1] { GameCenterBinding.playerIdentifier() }, Instance.defaultLeaderboard.iOS, true);
				//GameCenterBinding.retrieveScores(true, GameCenterLeaderboardTimeScope.AllTime, 1, 100, Instance.defaultLeaderboard.iOS);

				while(isSearchingPlayerFriendsPosition)
					yield return null;

				Debug.Log("Finish search");

				GameCenterManager.scoresForPlayerIdsLoadedEvent -= OnPlayerFriendsScoresLoaded;
				GameCenterManager.retrieveScoresForPlayerIdsFailedEvent -= OnPlayerFriendsScoresFailed;

				result(playerFriendsPosition, playerFriendsMaxRange);
			}
			else
			{
				Debug.LogError("Player is not authenticated");
				result(0, 0);
			}
			#elif UNITY_ANDROID
			if(PlayGameServices.isSignedIn())
			{
				isSearchingPlayerFriendsPosition = true;

				GPGManager.loadCurrentPlayerLeaderboardScoreSucceededEvent += OnPlayerGlobalScoresLoaded;
				GPGManager.loadCurrentPlayerLeaderboardScoreFailedEvent += OnPlayerGlobalScoresFailed;
				PlayGameServices.loadCurrentPlayerLeaderboardScore(Instance.defaultLeaderboard.android, GPGLeaderboardTimeScope.AllTime, true);

				while(isSearchingPlayerFriendsPosition)
					yield return null;

				GPGManager.loadCurrentPlayerLeaderboardScoreSucceededEvent -= OnPlayerGlobalScoresLoaded;
				GPGManager.loadCurrentPlayerLeaderboardScoreFailedEvent -= OnPlayerGlobalScoresFailed;

				result(playerGlobalPosition, playerGlobalMaxRange);
			}
			else
			{
				Debug.LogError("Player is not authenticated");
				result(0, 0);
			}
			#endif

		#endif

		yield return null;
		result(0, 0);
	}

	#if LEADERBOARDS_IMPLEMENTED && UNITY_IOS
	private static void OnPlayerFriendsScoresLoaded(GameCenterRetrieveScoresResult result)
	{
		Debug.Log(string.Format("Did recieved OnPlayerGlobalScoresLoaded. Total results: {0}: \n" +
			"alias: {1}; \n" +
			"id: {2}; \n" +
			"rank: {3} \n" +
			"maxRange: {4}", result.scores.Count, result.scores[0].alias, result.scores[0].playerId, result.scores[0].rank, result.scores[0].maxRange));
		playerGlobalPosition = result.scores[0].rank;
		playerGlobalMaxRange = result.scores[0].maxRange;

		isSearchingPlayerFriendsPosition = false;
	}


	private static void OnPlayerFriendsScoresFailed(string errmsg)
	{
		playerGlobalPosition = 0;

		isSearchingPlayerFriendsPosition = false;

		Debug.Log("Error on Friends Score: " + errmsg);
	}
	#endif

	public static void OpenLeaderboards()
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			if(GameCenterBinding.isPlayerAuthenticated())
				GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime );

			#elif UNITY_ANDROID
			if(PlayGameServices.isSignedIn())
				PlayGameServices.showLeaderboards();
			#endif

		#endif
	}

	public static void OpenLeaderboards(string leaderboardID)
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			if(GameCenterBinding.isPlayerAuthenticated())
				GameCenterBinding.showLeaderboardWithTimeScope( GameCenterLeaderboardTimeScope.AllTime);
			else
				GameCenterBinding.authenticateLocalPlayer();
			#elif UNITY_ANDROID
			if(PlayGameServices.isSignedIn())
				PlayGameServices.showLeaderboard(leaderboardID);
			else
				PlayGameServices.authenticate();
			#endif

		#endif

	}

	public static void SendScore(long score)
	{
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			SendScore(score, Instance.defaultLeaderboard.iOS);
			#elif UNITY_ANDROID
			SendScore(score, Instance.defaultLeaderboard.android);
			#endif

		#endif
	}

	public static void SendScore(long score, string leaderboardID)
	{
		Debug.Log("Sending score: " + score);
		#if LEADERBOARDS_IMPLEMENTED

			#if UNITY_IOS
			GameCenterBinding.reportScore(score, leaderboardID);
			#elif UNITY_ANDROID
			PlayGameServices.submitScore(leaderboardID, score);
			#endif

		#endif
	}

	private static void SubmitScoreSuccess(string leaderboardID, Dictionary<string, object> dic)
	{
		Debug.Log("Submit Score Success");
	}

	private static void SubmitScoreFailed(string leaderboardID, string msg)
	{
		Debug.Log("Submit Score Failed: " + msg);
	}
}