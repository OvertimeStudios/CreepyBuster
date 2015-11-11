using UnityEngine;
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

	private static bool facebookInit = false;

	#region get/set
	public static bool IsFacebookInit
	{
		get { return facebookInit; }
	}

	public static bool IsUserLoggedIn
	{
		get { return Global.FacebookID != ""; }
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

	public static void Logout()
	{
		FB.LogOut ();
	}
	#endif
}
