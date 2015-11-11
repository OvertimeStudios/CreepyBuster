using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if FACEBOOK_IMPLEMENTED
using Facebook.MiniJSON;
using Facebook.Unity;
#endif

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
		get { return Global.FacebookID != ""; }
	}
	
	public static string FacebookID
	{
		get { return Global.FacebookID; }
	}
	#endregion

	#region singleton
	private static FacebookController instance;
	public static FacebookController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject .FindObjectOfType<FacebookController>();

			return instance;
		}
	}
	#endregion

	// Use this for initialization
	void Awake () 
	{
		Debug.Log("Is FB Inisitalized? " + FB.IsInitialized);

		if(!FB.IsInitialized)
		{
			FacebookHelper.Init ();
			StartCoroutine(WaitForAlreadyLogin());
		}
		else
			FacebookHelper.ActivateApp();
	}

	private IEnumerator WaitForAlreadyLogin()
	{
		float time = 0;

		while(true)
		{
			time += Time.deltaTime;

			if(FB.IsLoggedIn || time > 3f)
				break;

			yield return null;
		}

		OnInitCompleted();
	}

	private void OnInitCompleted()
	{
		Debug.Log(string.Format("Facebook init Completed. Is user already logged in? {0} and FB.IsLoggedIn? {1}", IsLoggedIn, FB.IsLoggedIn));

		if(FB.IsInitialized)
		{
			FacebookHelper.ActivateApp();
			
			//already logged in
			if(FB.IsLoggedIn)
				Login ();
		}
		else
			Debug.Log("Failed to Initialize the Facebook SDK");
	}

	public void Login()
	{
		if(!FB.IsLoggedIn)
		{
			Debug.Log("New Login");

			List<string> perms = new List<string>()
			{
				"public_profile",
				"email",
				"user_friends"
			};

			SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
			FacebookHelper.Login (perms, LoginCallback);
			//StartCoroutine(WaitForLogin());
		}
		else
		{
			Debug.Log("FB.API");
			FacebookHelper.FetchData(FetchProfileNameCallback, FacebookHelper.FIRST_NAME, FacebookHelper.LAST_NAME, FacebookHelper.GENDER, FacebookHelper.EMAIL, FacebookHelper.TOKEN_FOR_BUSINESS);
			Debug.Log("User Already Logged In");
		}
	}

	private void LoginCallback(ILoginResult result)
	{
		Debug.Log("Login error: " + result.Error + ": " + result.RawResult);
		StartCoroutine(WaitForLogin());
	}

	private IEnumerator WaitForLogin()
	{
		float time = 0;

		while(!FB.IsLoggedIn)
		{
			time += Time.deltaTime;
			Debug.Log("time waiting: " + time);
			if(time > 3f)
			{
				Debug.Log("User cancelled login");
				StopAllCoroutines();
			}

			yield return null;
		}

		foreach (string perm in Facebook.Unity.AccessToken.CurrentAccessToken.Permissions) 
			Debug.Log(perm);

		FacebookHelper.FetchData(FetchProfileNameCallback, FacebookHelper.FIRST_NAME, FacebookHelper.LAST_NAME, FacebookHelper.GENDER, FacebookHelper.EMAIL, FacebookHelper.TOKEN_FOR_BUSINESS);
	}

	public void FetchProfileNameCallback(IGraphResult result)
	{
		if (result.Error != null)
		{
			Debug.Log ("FB API error response:\n" + result.Error + " \n" + result.RawResult);
		}
		else
		{
			Debug.Log("fetching profile name from result: "+result.RawResult);
			
			Dictionary<string, object> data = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

			fbUser = new FacebookUser();

			//facebookID is unique for user per app!!!!
			fbUser.id = data["id"].ToString();

			fbUser.firstname = data["first_name"].ToString();
			fbUser.lastname = data["last_name"].ToString();
			fbUser.gender = (data.ContainsKey("gender")) ? data["gender"].ToString() : "";
			fbUser.email = (data.ContainsKey("email")) ? data["email"].ToString() : "";

			//tokenForBusiness is unique for user for all apps of the same company!!!!
			fbUser.tokenForBusiness = data["token_for_business"].ToString();

			Global.FacebookID = fbUser.id;

			FacebookHelper.GetFacebookFriends(FetchUserFriendsCallback);
		}
	}

	public void FetchUserFriendsCallback(IGraphResult result)
	{
		if (result.Error != null)
		{
			Debug.Log ("FB API error response:\n" + result.Error + " \n" + result.RawResult);
		}
		else
		{
			Debug.Log("FetchUserFriendsCallback: " + result.RawResult);

			Dictionary<string, object> data = Json.Deserialize(result.RawResult) as Dictionary<string, object>;

			List<System.Object> friendList = data["data"] as List<System.Object>;

			fbUser.friends = new List<FacebookFriend>();

			foreach(System.Object friend in friendList)
			{
				Dictionary<string, object> friendData = friend as Dictionary<string, object>;

				fbUser.AddFriend(friendData["id"].ToString(), friendData["name"].ToString());
			}

			fbUser.LoadFriendsScore();
			fbUser.OnAllFriendsScoreLoaded += AllFriendsScoreLoaded;

			Debug.Log("OnLoggedIn");
			if(OnLoggedIn != null)
				OnLoggedIn();
		}
	}

	private void AllFriendsScoreLoaded()
	{
		Debug.Log("All Friends Score Successfuly Loaded");
	}

	public void LikeUs()
	{
		Application.OpenURL("fb://page/" + 1663775267187885);
		//Application.OpenURL("https://www.facebook.com/" + 1663775267187885);
	}

	public void Logout()
	{
		Global.FacebookID = "";
		FacebookHelper.Logout ();

		fbUser = null;

		if(OnLoggedOut != null)
			OnLoggedOut();
	}
	#endif
}

public class FacebookUser
{
	#region Action
	public event Action OnAllFriendsScoreLoaded;
	#endregion

	public string id;
	public string firstname;
	public string lastname;
	public string gender;
	public string email;
	public string tokenForBusiness;
	public List<FacebookFriend> friends;
	public bool friendsScoreLoaded;

	public int score;
	public int rank;

	public FacebookUser()
	{
		friends = new List<FacebookFriend>();
	}

	public void AddFriend(FacebookFriend friend)
	{
		friends.Add(friend);

		friend.OnScoreLoaded += FriendScoreLoaded;

		Debug.Log("Friend added: " + friend.ToString());
	}

	public void AddFriend(string id, string name)
	{
		AddFriend(new FacebookFriend(id, name));
	}

	public void LoadFriendsScore()
	{
		Debug.Log("LoadFriendsScore()");

		if(friends.Count > 0)
		{
			foreach(FacebookFriend friend in friends)
				friend.LoadScore();
		}
		else
			friendsScoreLoaded = true;
	}

	private void FriendScoreLoaded()
	{
		foreach(FacebookFriend friend in friends)
		{
			if(!friend.scoreLoaded) return;
		}

		friendsScoreLoaded = true;

		if(OnAllFriendsScoreLoaded != null)
			OnAllFriendsScoreLoaded();
	}

	public override string ToString ()
	{
		return "{id: " + id + "; " +
				"firstname: " + firstname + "; " +
				"lastname: " + lastname + "; " +
				"gender: " + gender + "; " +
				"email: " + email + "; " +
				"tokenForBusiness: " + tokenForBusiness + "; " +
				"friends: " + friends.Count + "}";
	}
}

public class FacebookFriend : IComparable<FacebookFriend>
{
	#region Action
	public event Action OnScoreLoaded;
	#endregion

	public string id;
	public string name;

	public Texture picture;
	public string pictureURL;

	public int score;
	public bool scoreLoaded;

	public int rank;

	public FacebookFriend(string id, string name, int score, int rank)
	{
		this.id = id;
		this.name = name;
		this.pictureURL = "https://graph.facebook.com/" + this.id + "/picture?type=square";
		
		this.score = score;
		this.scoreLoaded = true;

		this.rank = rank;
	}

	public FacebookFriend(string id, string name, int score)
	{
		this.id = id;
		this.name = name;
		this.pictureURL = "https://graph.facebook.com/" + this.id + "/picture?type=square";

		this.score = score;
		this.scoreLoaded = true;
	}

	public FacebookFriend(string id, string name)
	{
		this.id = id;
		this.name = name;
		this.pictureURL = "https://graph.facebook.com/" + this.id + "/picture?type=square";

		this.scoreLoaded = false;
	}

	public void LoadScore()
	{
		Debug.Log("Loading score for friend " + name);
		FacebookController.Instance.StartCoroutine(GetScore());
	}

	private IEnumerator GetScore()
	{
		yield return FacebookController.Instance.StartCoroutine(DBHandler.GetUserScore(this.id, value => score = value));

		scoreLoaded = true;
		Debug.Log(name + " score loaded: " + score);

		if(OnScoreLoaded != null)
			OnScoreLoaded();
	}

	public IEnumerator UpdateScore()
	{
		scoreLoaded = false;

		yield return FacebookController.Instance.StartCoroutine(DBHandler.GetUserScore(this.id, value => score = value));
		
		scoreLoaded = true;

		if(OnScoreLoaded != null)
			OnScoreLoaded();
	}

	public IEnumerator GetProfilePicture(System.Action<Texture> result)
	{
		Debug.Log("Start loading at " + pictureURL);
		WWW imageRequest = new WWW(pictureURL);
		yield return imageRequest;

		result(imageRequest.texture);
	}

	public int CompareTo(FacebookFriend comparePart)
	{
		// A null value means that this object is greater.
		if (comparePart == null)
			return 1;		
		else
			return this.score.CompareTo(comparePart.score);
	}

	public override bool Equals(System.Object obj)
	{
		if (obj == null)
			return false;

		FacebookFriend f = obj as FacebookFriend ;

		if ((System.Object)f == null)
			return false;

		return id == f.id;
	}
	public bool Equals(FacebookFriend f)
	{
		if ((object)f == null)
			return false;

		return id == f.id;
	}

	public override string ToString ()
	{
		return "{rank: #" + rank + "; " + 
				"id: " + id + "; " +
				"name: " + name + "; " +
				"pictureURL: " + pictureURL + "; " +
				"score: " + score + "}";
	}
}
