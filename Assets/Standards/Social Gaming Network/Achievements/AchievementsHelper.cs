using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if ACHIEVEMENTS_IMPLEMENTED
using Prime31;
#endif

/// <summary>
/// Achievements helper.
/// 
/// Add it to a Game Object on scene.
/// </summary>
public class AchievementsHelper : MonoBehaviour 
{
	#region EVENTS
	public static Action OnPlayerAuthenticated;
	#endregion

	#if UNITY_ANDROID
	private static List<GPGAchievementMetadata> achievementsMetadata;
	#endif

	#region singleton
	private static AchievementsHelper instance;
	public static AchievementsHelper Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<AchievementsHelper>();

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
			GameCenterManager.reportAchievementFinishedEvent += OnReportSuccess;
			GameCenterManager.reportAchievementFailedEvent += OnReportFailed;
			GameCenterManager.playerAuthenticatedEvent += PlayerAuthenticated;
			#elif UNITY_ANDROID
			GPGManager.authenticationSucceededEvent += PlayerAuthenticated;
			GPGManager.authenticationFailedEvent += PlayerAuthenticatedFailed;
			GPGManager.incrementAchievementSucceededEvent += OnReportSuccess;
			GPGManager.incrementAchievementFailedEvent += OnReportFailed;
			#endif
		#endif
	}

	void Start()
	{
		#if ACHIEVEMENTS_IMPLEMENTED
		Debug.Log("****!!!!Authenicate Game Service");

		Authenticate();
		#endif
	}

	public static void Authenticate()
	{
		#if UNITY_IOS
		GameCenterBinding.authenticateLocalPlayer();
		#elif UNITY_ANDROID
		Debug.Log("****!!!!Authenicate Game Service ANDROID");
		if(Debug.isDebugBuild)
			PlayGameServices.enableDebugLog( true );

		PlayGameServices.authenticate();
		#endif
	}

	private static void PlayerAuthenticated()
	{
		PlayerAuthenticated("");
	}

	private static void PlayerAuthenticated(string msg)
	{
		Debug.Log("****!!!!OnPlayerAuthenticated " + msg);
		#if ACHIEVEMENTS_IMPLEMENTED
			#if UNITY_IOS
			Debug.Log("Player successfully authenticated");

			if(OnPlayerAuthenticated != null)
				OnPlayerAuthenticated();

			#elif UNITY_ANDROID
			achievementsMetadata = PlayGameServices.getAllAchievementMetadata();
			Debug.Log("achievementsMetadata: " + achievementsMetadata.Count);
			#endif

		#endif
	}

	private static void PlayerAuthenticatedFailed(string msg)
	{
		Debug.Log("****!!!!OnPlayerAthenticated FAILED " + msg);
	}

	/// <summary>
	/// Reports the achievement progress.
	/// </summary>
	/// <param name="identifier">Identifier. (com.overtimestudios.gamename.achievements.achievementname)</param>
	/// <param name="value">iOS: Progress percent (0 - 100).
	/// 					Android: progress value (class calculate the difference from current steps and send. i.e. 54/150 - send 54</param>
	public static void ReportAchievement(string identifier, float value)
	{
		#if ACHIEVEMENTS_IMPLEMENTED

			#if UNITY_IOS

			if(GameCenterBinding.isPlayerAuthenticated())
			{
				GameCenterBinding.showCompletionBannerForAchievements();
				GameCenterBinding.reportAchievement(identifier, value);
			}
			#else
			if(PlayGameServices.isSignedIn())
			{
				if(achievementsMetadata == null || achievementsMetadata.Count == 0)
					achievementsMetadata = PlayGameServices.getAllAchievementMetadata();
			
				foreach(GPGAchievementMetadata achievementMetadata in achievementsMetadata)
				{
					if(achievementMetadata.achievementId == identifier)
					{
						if(achievementMetadata.type == 0)//Standard
						{
							Debug.Log(string.Format("*** STANDARD!!! PlayGameServices.unlockAchievement({0}) = value: {1};", identifier, value));
							if(value > 0)
								PlayGameServices.unlockAchievement(identifier);
						}
						else if(achievementMetadata.type == 1)//Incremental
						{
							//android need to receive only steps achieved. So, if I have 60 points and achievement has 55, I need to send 5
							int steps = Mathf.Max((int)value - achievementMetadata.completedSteps, 0);
							achievementMetadata.completedSteps += steps;
							
							Debug.Log(string.Format("*** INCREMENTAL!!! PlayGameServices.incrementAchievement({0}, {1});", identifier, steps));
							PlayGameServices.incrementAchievement(identifier, steps);

							break;
						}
					}
				}
			}
			else
				PlayGameServices.authenticate();
			#endif

		#endif
	}

	private static void OnReportSuccess(string msg, bool newUnlocked)
	{
		OnReportSuccess(msg);
	}

	private static void OnReportSuccess(string msg)
	{
		Debug.Log("Report Achievement Success: " + msg);

		#if ACHIEVEMENTS_IMPLEMENTED

			#if UNITY_IOS

			#elif UNITY_ANDROID

			#endif

		#endif
	}

	private static void OnReportFailed(string id, string error)
	{
		OnReportFailed(error);
	}

	private static void OnReportFailed(string msg)
	{
		Debug.Log("Report Achievement Failed: " + msg);	

		#if ACHIEVEMENTS_IMPLEMENTED

			#if UNITY_IOS

			#endif

		#endif
	}

	public static void OpenAchievements()
	{
		#if ACHIEVEMENTS_IMPLEMENTED

			#if UNITY_IOS
			if(GameCenterBinding.isPlayerAuthenticated())
				GameCenterBinding.showAchievements();
			else
				GameCenterBinding.authenticateLocalPlayer();
			#elif UNITY_ANDROID
			if(PlayGameServices.isSignedIn())
				PlayGameServices.showAchievements();
			else
				PlayGameServices.authenticate();
			#endif

		#endif
	}

	public static bool IsPlayerAuthenticated()
	{
		#if ACHIEVEMENTS_IMPLEMENTED

			#if UNITY_IOS
			return GameCenterBinding.isPlayerAuthenticated();
			#elif UNITY_ANDROID
			return PlayGameServices.isSignedIn();
			#endif

		#endif

		return false;
	}
}
