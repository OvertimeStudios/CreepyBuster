using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if FACEBOOK_IMPLEMENTED
using Facebook;
using Facebook.Unity;
#endif

public class FacebookHelper : MonoBehaviour 
{
	#if FACEBOOK_IMPLEMENTED
	//List of all params 'me' have: https://developers.facebook.com/docs/graph-api/reference/v2.2/user
	public static string ID = "id";
	public static string FIRST_NAME = "first_name";
	public static string LAST_NAME = "last_name";
	public static string EMAIL = "email";
	public static string GENDER = "gender";
	public static string TOKEN_FOR_BUSINESS = "token_for_business";

	public static string SCORE = "score";
	public static string USER_LIMIT_30 = "user.limit(30)";

	private static bool facebookInit = false;

	private static bool isSearchingForAllTimeFriends;

	#region get/set
	public static bool IsFacebookInit
	{
		get { return facebookInit; }
	}

	public static bool IsUserLoggedIn
	{
		get { return FB.IsLoggedIn; }
	}
	#endregion

	#region singleton
	private static FacebookHelper instance;
	public static FacebookHelper Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<FacebookHelper>();

			return instance;
		}
	}
	#endregion
	public static void Init()
	{
		Init (OnInitComplete);
	}

	public static void Init(InitDelegate del)
	{
		FB.Init (del, OnHideUnity);
	}

	private static void OnInitComplete()
	{
		facebookInit = true;

		Debug.Log("*****FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
	}

	public static void ActivateApp()
	{
		FB.ActivateApp();
	}

	private static void OnHideUnity(bool isGameShown)
	{
		if (!isGameShown) 
		{
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} 
		else 
		{
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}

	public static void Login(List<string> perms)
	{
		Login (perms, null);
	}

	public static void Login(List<string> perms, FacebookDelegate<ILoginResult> del)
	{
		Debug.Log(string.Format("FB.IsInitialized: {0} and FB.IsLoggedIn: {1}",FB.IsInitialized, FB.IsLoggedIn));
		if (!FB.IsInitialized)
		{
			Debug.LogError("You must call FacebookHandler.Init() before doing any action");
			return;
		}

		if(!FB.IsLoggedIn)
			FB.LogInWithReadPermissions (perms, del);
	}

	public static void FetchData (FacebookDelegate<IGraphResult> del, params string[] data)
	{
		string query = "me?fields=";
		foreach(string d in data)
			query += d + ",";

		//remove last comma
		query = query.Substring (0, query.Length - 1);
		//Debug.Log (query);

		Dictionary<string, string> formData = new Dictionary<string, string> ();
		FB.API (query, Facebook.Unity.HttpMethod.GET, del, formData);
	}

	public static void GetFacebookFriends(FacebookDelegate<IGraphResult> del)
	{
		Debug.Log("GetFacebookFriends");
		string query = "me/friends?fields=id,name";
		FB.API (query, Facebook.Unity.HttpMethod.GET, del);
	}

	public static void SetScore(int score, FacebookDelegate<IGraphResult> del)
	{
		var scoreData = new Dictionary<string, string>();
		scoreData.Add("score", score.ToString());

		FB.API("/me/scores", Facebook.Unity.HttpMethod.POST, del, new Dictionary<string, string>());
	}

	private static void QueryScores(FacebookDelegate<IGraphResult> del, params string[] data)
	{
		string query = "/app/scores";

		FB.API(query, Facebook.Unity.HttpMethod.GET, del,new Dictionary<string, string> ());
	}
		
	public static IEnumerator GetAllTimeFriendsRank(Action<int> res)
	{
		bool isSearching = true;
		int rank = -1;

		QueryScores((IGraphResult result) => 
			{
				if(result.Error != null)
				{
					Debug.Log ("FB API error response:\n" + result.Error + " \n" + result.RawResult);
				}
				else
				{
					IDictionary<string, object> data = result.ResultDictionary;
					List<object> scoreList = (List<object>) data["data"];

					for(byte i = 0; i < scoreList.Count; i++)
					{
						var entry = (Dictionary<string, object>)scoreList[i];
						var user = (Dictionary<string, object>) entry["user"];

						if(user["id"].ToString() == FacebookController.User.id)
						{
							rank = i + 1;
							Debug.Log("Found " + user["name"].ToString() + ", " + entry["score"].ToString() + " at rank #" + rank);
						}
					}
				}
				isSearching = false;
			});

		while(isSearching) 
			yield return null;

		res(rank);
	}

	public static void Logout()
	{
		FB.LogOut ();
	}
	#endif
}
