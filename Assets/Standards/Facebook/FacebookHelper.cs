using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public static string UserID
	{
		get { return FB.UserId; }
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

	public static void Init(Facebook.InitDelegate del)
	{
		FB.Init (del);
	}

	private static void OnInitComplete()
	{
		facebookInit = true;

		Debug.Log("*****FB.Init completed: Is user logged in? " + FB.IsLoggedIn);
	}

	public static void Login(string scope)
	{
		Login (scope, null);
	}

	public static void Login(string scope, Facebook.FacebookDelegate del)
	{
		if (!FB.IsInitialized)
		{
			Debug.LogError("You must call FacebookHandler.Init() before doing any action");
			return;
		}

		if(!FB.IsLoggedIn)
			FB.Login (scope, del);
	}

	public static void FetchData (Facebook.FacebookDelegate del, params string[] data)
	{
		string query = "me?fields=";
		foreach(string d in data)
			query += d + ",";

		//remove last comma
		query = query.Substring (0, query.Length - 1);
		//Debug.Log (query);

		Dictionary<string, string> formData = new Dictionary<string, string> ();
		FB.API (query, Facebook.HttpMethod.GET, del, formData);
	}

	public static void Logout()
	{
		FB.Logout ();
	}
	#endif
}
