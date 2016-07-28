using UnityEngine;
using System;
using System.Collections;

#if ACHIEVEMENTS_IMPLEMENTED
using Prime31;
#endif

public class AchievementsHelper : MonoBehaviour 
{
	#region EVENTS
	public static Action OnPlayerAuthenticated;
	#endregion

	void Start()
	{
		#if ACHIEVEMENTS_IMPLEMENTED

		#if UNITY_IOS
		GameCenterManager.playerAuthenticatedEvent += OnPlayerAthenticated;

		GameCenterBinding.authenticateLocalPlayer();
		#else

		#endif

		#endif
	}

	private static void OnPlayerAthenticated()
	{
		#if ACHIEVEMENTS_IMPLEMENTED

		#if UNITY_IOS
		Debug.Log("Player successfully authenticated");

		if(OnPlayerAuthenticated != null)
			OnPlayerAuthenticated();

		GameCenterManager.playerAuthenticatedEvent -= OnPlayerAthenticated;
		#endif

		#endif
	}

	public static void ReportAchievement(string identifier, float percent)
	{
		#if ACHIEVEMENTS_IMPLEMENTED

		#if UNITY_IOS

		if(GameCenterBinding.isPlayerAuthenticated())
		{
			GameCenterBinding.showCompletionBannerForAchievements();
			GameCenterBinding.reportAchievement(identifier, percent);

			GameCenterManager.reportAchievementFinishedEvent += OnReportSuccess;
			GameCenterManager.reportAchievementFailedEvent += OnReportFailed;
		}
		#else

		#endif

		#endif
	}

	private static void OnReportSuccess(string msg)
	{
		Debug.Log("Report Achievement Success: " + msg);

		#if ACHIEVEMENTS_IMPLEMENTED

		#if UNITY_IOS
		GameCenterManager.reportAchievementFinishedEvent -= OnReportSuccess;
		GameCenterManager.reportAchievementFailedEvent -= OnReportFailed;

		#endif

		#endif
	}

	private static void OnReportFailed(string msg)
	{
		Debug.Log("Report Achievement Failed: " + msg);	

		#if ACHIEVEMENTS_IMPLEMENTED

		#if UNITY_IOS

		GameCenterManager.reportAchievementFinishedEvent -= OnReportSuccess;
		GameCenterManager.reportAchievementFailedEvent -= OnReportFailed;

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
		#else

		#endif

		#endif
	}

	public static bool IsPlayerAuthenticated()
	{
		#if ACHIEVEMENTS_IMPLEMENTED

		#if UNITY_IOS
		return GameCenterBinding.isPlayerAuthenticated();
		#else

		#endif

		#else
		return false;
		#endif
	}
}
