using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if FACEBOOK_IMPLEMENTED
using Facebook.Unity;
#endif

public class Ranking : MonoBehaviour 
{
	public UILabel highScore;
	public UILabel worldAllTimeRank;
	public UILabel worldDailyRank;
	public UILabel worldLoading;
	public UILabel friendsAllTimeRank;
	public UILabel friendsDailyRank;
	public UILabel friendsLoading;

	private bool listSorted = false;

	public GameObject general;

	public GameObject globalInfo;
	public GameObject friendsInfo;

	public GameObject globalLogin;
	public GameObject friendsLogin;

	void OnEnable()
	{
		highScore.text = Global.HighScore.ToString();
		worldAllTimeRank.text = "";
		worldDailyRank.text = "";
		worldLoading.enabled = GameSparksController.IsUserLoggedIn;
		friendsAllTimeRank.text = "";
		friendsDailyRank.text = "";
		friendsLoading.enabled = GameSparksController.IsUserLoggedIn;

		general.SetActive(true);
		
		globalInfo.SetActive(GameSparksController.IsUserLoggedIn);
		friendsInfo.SetActive(GameSparksController.IsUserLoggedIn);

		#if LEADERBOARDS_IMPLEMENTED
		if(FacebookController.IsLoggedIn)
		{
			if(GameSparksController.IsUserLoggedIn)
				GetRanks();
			else
			{
				GameSparksController.OnUserGSLogin += GetRanks;
				GameSparksController.Instance.GSFacebookLogin();
			}
		}
		else
			GameSparksController.OnUserGSLogin += GetRanks;
		#endif
	}

	void OnDisable()
	{
		GameSparksController.OnUserGSLogin -= GetRanks;
	}

	void OnDestroy()
	{
		#if LEADERBOARDS_IMPLEMENTED
		GameSparksController.OnUserGSLogin -= GetRanks;
		#endif
	}

	public void AuthenticatePlayer()
	{
		#if LEADERBOARDS_IMPLEMENTED

		#endif
	}

	private void GetRanks()
	{
		GameSparksController.OnUserGSLogin -= GetRanks;

		globalLogin.SetActive(false);
		friendsLogin.SetActive(false);
		worldLoading.enabled = true;
		friendsLoading.enabled = true;

		StartCoroutine(LoadRanks());
	}

	private IEnumerator LoadRanks()
	{
		Debug.Log("Load Ranks");
		yield return StartCoroutine(GetGlobalRank());
		yield return StartCoroutine(GetFriendsRank());
	}

	private IEnumerator GetGlobalRank()
	{
		Debug.Log("Getting All Time Global ranking...");

		#if LEADERBOARDS_IMPLEMENTED
		int allTimeRank = 0;
		int dailyRank = 0;

		yield return StartCoroutine(GameSparksController.GetUserAllTimeWorldRank((res) => allTimeRank = res));
		yield return StartCoroutine(GameSparksController.GetUserDailyWorldRank((res) => dailyRank = res));

		SetGlobalRank(allTimeRank, dailyRank);
		#else
		yield return null;
		#endif
	}

	private IEnumerator GetFriendsRank()
	{
		Debug.Log("Getting All Time Friends ranking...");

		#if LEADERBOARDS_IMPLEMENTED
		int allTimeRank = 0;
		int dailyRank = 0;

		yield return StartCoroutine(GameSparksController.GetUserAllTimeFriendsRank((res) => allTimeRank = res));
		yield return StartCoroutine(GameSparksController.GetUserDailyFriendsRank((res) => dailyRank = res));

		SetFriendsRank(allTimeRank, dailyRank);
		#else
		yield return null;
		#endif
	}

	private void SetGlobalRank(int allTimeRank, int dailyRank)
	{
		Debug.Log(string.Format("Rankings.SetGlobalRank({0},{1})", allTimeRank, dailyRank));

		worldAllTimeRank.text = (allTimeRank <= 0) ? "Error" : "#" + allTimeRank;
		worldDailyRank.text = (dailyRank <= 0) ? "Error" : "#" + dailyRank;

		worldLoading.enabled = false;

		if(allTimeRank > 0 || dailyRank > 0)
			globalInfo.SetActive(true);
	}

	private void SetFriendsRank(int allTimeRank, int dailyRank)
	{
		Debug.Log(string.Format("Rankings.SetFriendsRank({0},{1})", allTimeRank, dailyRank));

		friendsAllTimeRank.text = (allTimeRank <= 0) ? "Error" : "#" + allTimeRank;
		friendsDailyRank.text = (dailyRank <= 0) ? "Error" : "#" + dailyRank;

		friendsLoading.enabled = false;

		if(allTimeRank > 0 || dailyRank > 0)
			friendsInfo.SetActive(true);
	}

	public void OpenGlobalRank()
	{
		#if LEADERBOARDS_IMPLEMENTED

		#endif
	}

	public void OpenFriendsRank()
	{
		#if LEADERBOARDS_IMPLEMENTED

		#endif
	}

	public void CloseRank()
	{
		
	}
}
