using UnityEngine;
using System;
using System.Collections;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class GameSparksController : Singleton<GameSparksController> 
{
	#region Action
	public static Action OnUserGSLogin;
	#endregion

	#region Get / Set
	public static bool IsUserLoggedIn
	{
		get { return isLoggedIn; }
	}
	#endregion

	private static bool isLoggedIn = false;

	public void GSFacebookLogin()
	{
		if(!FacebookController.IsLoggedIn)
		{
			FacebookController.Instance.Login();

			FacebookController.OnJustLoggedIn -= OnFacebookLogin;
			FacebookController.OnJustLoggedIn += OnFacebookLogin;
		}
		else
			OnFacebookLogin();
	}

	private void OnFacebookLogin()
	{
		FacebookController.OnJustLoggedIn -= OnFacebookLogin;

		new FacebookConnectRequest().SetAccessToken(FacebookController.AccessTokenForSession).Send(GSLoginCallback);
	}

	private void GSLoginCallback(AuthenticationResponse authResponse)
	{
		if(authResponse.HasErrors)
		{
			Debug.Log("Oh oh! Something went wrong!");
		}
		else
		{
			isLoggedIn = true;
			Debug.Log("Logged in successfully!");

			if(OnUserGSLogin != null)
				OnUserGSLogin();
		}
	}

	public static void SendScore(int score)
	{
		new LogEventRequest().SetEventKey("SUBMIT_SCORE").SetEventAttribute("SCORE", score).Send((response) => 
			{
				if(!response.HasErrors)
					Debug.Log("Send Score Successfuly");
				else
					Debug.Log("Failed Send Score");
			});
	}

	public static IEnumerator GetUserAllTimeWorldRank(Action<int> res)
	{
		bool isSearching = true;
		int rank = -1;

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode("ALLTIME").SetEntryCount(1).SetSocial(false).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetUserAllTimeWorldRank");
				}
				else
				{
					foreach(AroundMeLeaderboardResponse._LeaderboardData entry in result.Data)
					{
						rank = (int)entry.Rank;
						Debug.Log(rank);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(rank);
	}

	public static IEnumerator GetUserAllTimeFriendsRank(Action<int> res)
	{
		bool isSearching = true;
		int rank = -1;

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode("ALLTIME").SetEntryCount(1).SetSocial(true).SetDontErrorOnNotSocial(true).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetUserAllTimeFriendsRank");
				}
				else
				{
					foreach(AroundMeLeaderboardResponse._LeaderboardData entry in result.Data)
					{
						rank = (int)entry.Rank;
						Debug.Log(rank);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(rank);
	}

	public static IEnumerator GetUserDailyWorldRank(Action<int> res)
	{
		bool isSearching = true;
		int rank = -1;

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode("DAILY").SetEntryCount(1).SetSocial(false).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetUserDailyWorldRank");
				}
				else
				{
					foreach(AroundMeLeaderboardResponse._LeaderboardData entry in result.Data)
						rank = (int)entry.Rank;
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(rank);
	}

	public static IEnumerator GetUserDailyFriendsRank(Action<int> res)
	{
		bool isSearching = true;
		int rank = -1;

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode("DAILY").SetEntryCount(1).SetSocial(true).SetDontErrorOnNotSocial(true).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetUserDailyFriendsRank");
				}
				else
				{
					foreach(AroundMeLeaderboardResponse._LeaderboardData entry in result.Data)
						rank = (int)entry.Rank;
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(rank);
	}

}
