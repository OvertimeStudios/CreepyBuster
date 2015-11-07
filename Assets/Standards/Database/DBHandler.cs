using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if DB_IMPLEMENTED
using Facebook.MiniJSON;
#endif

public class DBHandler : MonoBehaviour 
{
	#if DB_IMPLEMENTED
	private const string getGameIDURL = "http://www.overtimestudios.com/server/GetGameID.php?";
	private const string getUserURL = "http://www.overtimestudios.com/server/GetUser.php?";
	private const string createUserURL = "http://www.overtimestudios.com/server/CreateUser.php?";
	private const string getUserRankingURL = "http://www.overtimestudios.com/server/GetUserRanking.php?";
	private const string createUserRankingURL = "http://www.overtimestudios.com/server/CreateUserRanking.php?";
	private const string getUserScoreURL = "http://www.overtimestudios.com/server/GetUserScore.php?";
	private const string updateUserScoreURL = "http://www.overtimestudios.com/server/UpdateUserScore.php?";

	private static DBUser dbUser;

	#region get/set
	public static DBUser User
	{
		get { return dbUser; }
	}
	#endregion
	
	#region singleton
	private static DBHandler instance;
	public static DBHandler Instance
	{
		get 
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<DBHandler>();
			
			return instance;
		}
	}
	#endregion
	
	public static IEnumerator GetUser(string facebookID, System.Action<DBUser> result)
	{
		Debug.Log("DBHandler.GetUser()");
		DBUser user = null;

		string post_url = getUserURL + "token_for_business=" + facebookID;
		Debug.Log(string.Format("GetUser URL: {0}", post_url));

		// Post the URL to the site and create a download object to get the result.
		WWW getUser_post = new WWW(post_url);
		yield return getUser_post; // Wait until the download is done
		
		if (getUser_post.error != null)
			Debug.Log("There was an error posting the GetUser : " + getUser_post.error);

		Debug.Log(string.Format("WWW post: {0}", getUser_post.text));

		if(!string.IsNullOrEmpty(getUser_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getUser_post.text) as Dictionary<string, object>;

			user = new DBUser(int.Parse(data["id"].ToString()), 
			                  data["first_name"].ToString(),
			                  data["last_name"].ToString(),
			                  data["email"].ToString(),
			                  data["gender"].ToString(),
			                  data["token_for_business"].ToString());
		}

		dbUser = user;

		result(user);
	}
	
	public static IEnumerator CreateUser(string facebookID, string firstName, string lastName, string email, string gender)
	{
		Debug.Log("No user found... Creating new user");

		string post_url = createUserURL + "firstname=" + WWW.EscapeURL(firstName) +
										  "&lastname=" + WWW.EscapeURL(lastName) +
										  "&email=" + email +
										  "&gender=" + gender +
										  "&token_for_business=" + facebookID;
		
		Debug.Log(string.Format("CreateUser URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW createUserID_post = new WWW(post_url);
		yield return createUserID_post; // Wait until the download is done
	}

	public static IEnumerator GetGameID(string gameName, System.Action<int> result)
	{
		int gameID = -1;

		Debug.Log(string.Format("GetGameID({0})", gameName));

		string post_url = getGameIDURL + "gameName=" + WWW.EscapeURL(gameName);

		Debug.Log(string.Format("GetGameID URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW getGameID_post = new WWW(post_url);
		yield return getGameID_post; // Wait until the download is done
		
		if (getGameID_post.error != null)
			Debug.Log("There was an error posting the GetGameID : " + getGameID_post.error);
		
		Debug.Log(string.Format("WWW post: {0}", getGameID_post.text));
		
		if(!string.IsNullOrEmpty(getGameID_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getGameID_post.text) as Dictionary<string, object>;
			
			gameID = int.Parse(data["gameID"].ToString());
		}
		
		result(gameID);
	}

	public static IEnumerator GetUserRanking(int userID, int gameID, System.Action<int> result)
	{
		int position = -1;

		Debug.Log("DBHandler.GetUserRanking()");
		
		string post_url = getUserRankingURL + "userID=" + userID + 
									   		  "&gameID=" + gameID;

		Debug.Log(string.Format("GetUserRanking URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW getUserRanking_post = new WWW(post_url);
		yield return getUserRanking_post; // Wait until the download is done
		
		if (getUserRanking_post.error != null)
			Debug.Log("There was an error posting the GetUserRankingPosition : " + getUserRanking_post.error);
		
		Debug.Log(string.Format("WWW post: {0}", getUserRanking_post.text));
		
		if(!string.IsNullOrEmpty(getUserRanking_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getUserRanking_post.text) as Dictionary<string, object>;
			
			position = int.Parse(data["rank"].ToString());
		}	

		Debug.Log(string.Format("rank: {0}", position));
		
		result(position);
	}

	public static IEnumerator CreateUserRanking(int userID, int gameID)
	{
		Debug.Log("No ranking found... Creating new rank");
		
		string post_url = createUserRankingURL + "userID=" + userID + 
											     "&gameID=" + gameID;
		
		Debug.Log(string.Format("CreateUserRanking URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW createUserRanking_post = new WWW(post_url);
		yield return createUserRanking_post; // Wait until the download is done
	}

	public static IEnumerator GetUserScore(int userID, int gameID, System.Action<float> result)
	{
		float score = -1;
		
		Debug.Log("DBHandler.GetUserScore()");
		
		string post_url = getUserScoreURL + "userID=" + userID + 
											"&gameID=" + gameID;
		
		Debug.Log(string.Format("GetUserScore URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW getUserScore_post = new WWW(post_url);
		yield return getUserScore_post; // Wait until the download is done
		
		if (getUserScore_post.error != null)
			Debug.Log("There was an error posting the GetUserRankingPosition : " + getUserScore_post.error);
		
		Debug.Log(string.Format("WWW post: {0}", getUserScore_post.text));
		
		if(!string.IsNullOrEmpty(getUserScore_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getUserScore_post.text) as Dictionary<string, object>;
			
			score = float.Parse(data["score"].ToString());
		}	
		
		Debug.Log(string.Format("score: {0}", score));
		
		result(score);
	}

	public static IEnumerator UpdateUserScore(int userID, int gameID, float score)
	{
		Debug.Log("Updating user score");

		string post_url = updateUserScoreURL + "userID=" + userID + 
											   "&gameID=" + gameID + 
											   "&score=" + score;
		
		Debug.Log(string.Format("UpdateUserScore URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW updateUserScore_post = new WWW(post_url);
		yield return updateUserScore_post; // Wait until the download is done
	}


	#endif
}

public class DBUser 
{
	public int id;
	public string firstname;
	public string lastname;
	public string email;
	public string gender;
	public string facebookID;

	public DBUser()
	{

	}

	public DBUser(int id, string firstname, string lastname, string email, string gender, string facebookID)
	{
		this.id = id;
		this.firstname = firstname;
		this.lastname = lastname;
		this.email = email;
		this.gender = gender;
		this.facebookID = facebookID;
	}

	public override string ToString()
	{
		return 	"{id: " + id + "; " +
				"facebookID: " + facebookID + "; " +
				"First Name: " + firstname + "; " +
				"Last Name: " + lastname + "; " +
				"E-mail: " + email + "; " +
				"Gender: " + gender + "}";
	}
}