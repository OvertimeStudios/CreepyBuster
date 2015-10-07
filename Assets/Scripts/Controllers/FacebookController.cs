using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;

public class FacebookController : MonoBehaviour 
{
	#if FACEBOOK_IMPLEMENTED
	#region Actions
	public static event Action OnLoggedIn;
	public static event Action OnLoggedOut;
	#endregion

	private static FacebookUser fbUser;

	#region get / set
	public static FacebookUser User
	{
		get { return fbUser; }
	}

	public static bool IsLoggedIn
	{
		get { return fbUser != null; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		FacebookHelper.Init ();
	}

	public void Login()
	{
		string scope = "public_profile,email";

		FacebookHelper.Login (scope, LoginCallback);
	}

	private void LoginCallback(FBResult result)
	{
		if (result.Error != null)//login error
			Debug.LogError (result.Error);
		else if (!FB.IsLoggedIn)//cancelled login
			Debug.Log (result.Text);
		else//login successful
		{
			Debug.Log(result.Text);
			
			//"me?fields=id,first_name,last_name,email,gender,token_for_business"
			FacebookHelper.FetchData(FetchProfileNameCallback, FacebookHelper.FIRST_NAME, FacebookHelper.LAST_NAME, FacebookHelper.GENDER, FacebookHelper.EMAIL, FacebookHelper.TOKEN_FOR_BUSINESS);
		}
	}

	public void FetchProfileNameCallback(FBResult result)
	{
		if (result.Error != null)
		{
			Debug.Log ("FB API error response:\n" + result.Error);
		}
		else
		{
			Debug.Log("fetching profile name from result: "+result.Text);
			
			Dictionary<string, object> data = Json.Deserialize(result.Text) as Dictionary<string, object>;

			fbUser = new FacebookUser();

			//facebookID is unique for user per app!!!!
			fbUser.id = data["id"].ToString();

			fbUser.firstname = data["first_name"].ToString();
			fbUser.lastname = data["last_name"].ToString();
			fbUser.gender = (data.ContainsKey("gender")) ? data["gender"].ToString() : "";
			fbUser.email = (data.ContainsKey("email")) ? data["email"].ToString() : "";
			//tokenForBusiness is unique for user for all apps of the same company!!!!
			fbUser.tokenForBusiness = data["token_for_business"].ToString();

			Debug.Log(fbUser.ToString());

			if(OnLoggedIn != null)
				OnLoggedIn();
		}
	}

	public void Logout()
	{
		FacebookHelper.Logout ();

		fbUser = null;

		if(OnLoggedOut != null)
			OnLoggedOut();
	}
	#endif
}

public class FacebookUser
{
	public string id;
	public string firstname;
	public string lastname;
	public string gender;
	public string email;
	public string tokenForBusiness;

	public override string ToString ()
	{
		return "{id: " + id + "; " +
				"firstname: " + firstname + ";" +
				"lastname: " + lastname + ";" +
				"gender: " + gender + ";" +
				"email: " + email + ";" +
				"tokenForBusiness: " + tokenForBusiness + ";";
	}
}
