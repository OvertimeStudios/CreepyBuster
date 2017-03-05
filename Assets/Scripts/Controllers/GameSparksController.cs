using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class GameSparksController : Singleton<GameSparksController> 
{
	private static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

	#region Action
	public static Action<bool> OnUserGSLogin;
	#endregion

	#region Get / Set
	public static bool IsUserLoggedIn
	{
		get { return isLoggedIn; }
	}

	public static string DAILY_LEADERBOARD
	{
		get 
		{ 
			DateTime today = DateTime.UtcNow;
			today = new DateTime(today.Year, today.Month, today.Day);
			int time = (int)(today.Subtract(epochStart)).TotalSeconds;

			return "DAILY.DAY." + time + "000";
		}
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

		new FacebookConnectRequest().SetAccessToken(FacebookController.AccessTokenForSession).SetDoNotLinkToCurrentPlayer(true).SetSwitchIfPossible(true).Send(GSLoginCallback);
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
				OnUserGSLogin((bool)authResponse.NewPlayer);
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
						rank = (int)entry.Rank;
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
						rank = (int)entry.Rank;
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

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode(DAILY_LEADERBOARD).SetEntryCount(1).SetSocial(false).Send((result) =>
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

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode(DAILY_LEADERBOARD).SetEntryCount(1).SetSocial(true).SetDontErrorOnNotSocial(true).Send((result) =>
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

	public static IEnumerator GetAllTimeWorldList(Action<List<FacebookFriend>> res)
	{
		bool isSearching = true;
		List<FacebookFriend> list = new List<FacebookFriend>();

		new LeaderboardDataRequest().SetLeaderboardShortCode("ALLTIME").SetEntryCount(10).SetSocial(false).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetAllTimeWorldList");
				}
				else
				{
					foreach(LeaderboardDataResponse._LeaderboardData entry in result.Data)
					{
						string name = entry.UserName;
						int score = int.Parse(entry.JSONData["SCORE"].ToString());
						int rank = (int)entry.Rank;
						string facebookID = entry.ExternalIds.BaseData["FB"].ToString();

						FacebookFriend friend = new FacebookFriend(facebookID, name, score, rank);
						list.Add(friend);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(list);
	}

	/// <summary>
	/// Gets the player all time world list. Count = 1
	/// </summary>
	/// <returns>The player all time world info.</returns>
	/// <param name="res">Res.</param>
	public static IEnumerator GetPlayerAllTimeWorldInfo(Action<FacebookFriend> res)
	{
		bool isSearching = true;
		FacebookFriend friend = null;

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode("ALLTIME").SetEntryCount(1).SetSocial(false).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetPlayerAllTimeWorldInfo");
				}
				else
				{
					foreach(AroundMeLeaderboardResponse._LeaderboardData entry in result.Data)
					{
						string name = entry.UserName;
						int score = int.Parse(entry.JSONData["SCORE"].ToString());
						int rank = (int)entry.Rank;
						string facebookID = entry.ExternalIds.BaseData["FB"].ToString();

						friend = new FacebookFriend(facebookID, name, score, rank);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(friend);
	}

	public static IEnumerator GetDailyWorldList(Action<List<FacebookFriend>> res)
	{
		bool isSearching = true;
		List<FacebookFriend> list = new List<FacebookFriend>();

		new LeaderboardDataRequest().SetLeaderboardShortCode(DAILY_LEADERBOARD).SetEntryCount(10).SetSocial(false).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetDailyWorldList");
				}
				else
				{
					foreach(LeaderboardDataResponse._LeaderboardData entry in result.Data)
					{
						string name = entry.UserName;
						int score = int.Parse(entry.JSONData["SCORE"].ToString());
						int rank = (int)entry.Rank;
						string facebookID = entry.ExternalIds.BaseData["FB"].ToString();

						FacebookFriend friend = new FacebookFriend(facebookID, name, score, rank);
						list.Add(friend);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(list);
	}

	/// <summary>
	/// Gets the player daily world info. Count = 1
	/// </summary>
	/// <returns>The player daily world info.</returns>
	/// <param name="res">Res.</param>
	public static IEnumerator GetPlayerDailyWorldInfo(Action<FacebookFriend> res)
	{
		bool isSearching = true;
		FacebookFriend friend = null;

		new AroundMeLeaderboardRequest().SetLeaderboardShortCode(DAILY_LEADERBOARD).SetEntryCount(1).SetSocial(false).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetPlayerDailyWorldInfo");
				}
				else
				{
					foreach(AroundMeLeaderboardResponse._LeaderboardData entry in result.Data)
					{
						string name = entry.UserName;
						int score = int.Parse(entry.JSONData["SCORE"].ToString());
						int rank = (int)entry.Rank;
						string facebookID = entry.ExternalIds.BaseData["FB"].ToString();

						friend = new FacebookFriend(facebookID, name, score, rank);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(friend);
	}

	public static IEnumerator GetAllTimeFriendsList(Action<List<FacebookFriend>> res)
	{
		bool isSearching = true;
		List<FacebookFriend> list = new List<FacebookFriend>();

		new LeaderboardDataRequest().SetLeaderboardShortCode("ALLTIME").SetEntryCount(10).SetSocial(true).SetDontErrorOnNotSocial(true).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetAllTimeFriendsList");
				}
				else
				{
					foreach(LeaderboardDataResponse._LeaderboardData entry in result.Data)
					{
						string name = entry.UserName;
						int score = int.Parse(entry.JSONData["SCORE"].ToString());
						int rank = (int)entry.Rank;
						string facebookID = entry.ExternalIds.BaseData["FB"].ToString();

						FacebookFriend friend = new FacebookFriend(facebookID, name, score, rank);
						list.Add(friend);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(list);
	}

	public static IEnumerator GetDailyFriendsList(Action<List<FacebookFriend>> res)
	{
		bool isSearching = true;
		List<FacebookFriend> list = new List<FacebookFriend>();

		new LeaderboardDataRequest().SetLeaderboardShortCode(DAILY_LEADERBOARD).SetEntryCount(10).SetSocial(true).SetDontErrorOnNotSocial(true).Send((result) =>
			{
				if(result.HasErrors)
				{
					Debug.Log("Error on GetDailyFriendsList");
				}
				else
				{
					foreach(LeaderboardDataResponse._LeaderboardData entry in result.Data)
					{
						string name = entry.UserName;
						int score = int.Parse(entry.JSONData["SCORE"].ToString());
						int rank = (int)entry.Rank;
						string facebookID = entry.ExternalIds.BaseData["FB"].ToString();

						FacebookFriend friend = new FacebookFriend(facebookID, name, score, rank);
						list.Add(friend);
					}
				}

				isSearching = false;
			});

		while(isSearching)
			yield return null;

		res(list);
	}
}
