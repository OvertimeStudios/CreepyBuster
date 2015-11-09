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
	private const string getGlobalUserURL = "http://www.overtimestudios.com/server/global_GetUser.php?";
	private const string createGlobalUserURL = "http://www.overtimestudios.com/server/global_CreateUser.php?";

	private const string getGameUserURL = "http://www.overtimestudios.com/server/creepybuster_GetUser.php?";
	private const string createGameUserURL = "http://www.overtimestudios.com/server/creepybuster_CreateUser.php?";
	private const string updateUserScoreURL = "http://www.overtimestudios.com/server/creepybuster_UpdateScore.php?";
	private const string getUserGlobalRankingURL = "http://www.overtimestudios.com/server/creepybuster_GetUserGlobalRanking.php?";
	private const string getUserScoreURL = "http://www.overtimestudios.com/server/creepybuster_GetUserScore.php?";
	private const string getTopUsersURL = "http://www.overtimestudios.com/server/creepybuster_GetTopRankings.php?";

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

	#region GLOBAL
	public static IEnumerator CheckAndCreateGlobalUser(string token_for_business, string firstName, string lastName, string email, string gender, System.Action<int> result)
	{
		int globalID = 0;
		Debug.Log("Checking if Global User exists...");
		yield return Instance.StartCoroutine(GetGlobalUser(token_for_business, value => globalID = value));

		if(globalID == 0)
		{
			yield return Instance.StartCoroutine(CreateGlobalUser(token_for_business, firstName, lastName, email, gender));
			yield return Instance.StartCoroutine(GetGlobalUser(token_for_business, value => globalID = value));
		}
		else
		{
			Debug.Log("Global user already exists");
		}

		result(globalID);
	}

	private static IEnumerator GetGlobalUser(string token_for_business, System.Action<int> result)
	{
		Debug.Log("DBHandler.GetGlobalUser()");
		int globalID = 0;

		string post_url = getGlobalUserURL + "token_for_business=" + token_for_business;
		Debug.Log(string.Format("GetGlobalUser URL: {0}", post_url));

		// Post the URL to the site and create a download object to get the result.
		WWW getUser_post = new WWW(post_url);
		yield return getUser_post; // Wait until the download is done
		
		if (getUser_post.error != null)
			Debug.Log("There was an error posting the GetUser : " + getUser_post.error);

		Debug.Log(string.Format("WWW post: {0}", getUser_post.text));

		if(!string.IsNullOrEmpty(getUser_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getUser_post.text) as Dictionary<string, object>;
			
			globalID = int.Parse(data["id"].ToString());
		}

		result(globalID);
	}
	
	private static IEnumerator CreateGlobalUser(string token_for_business, string firstName, string lastName, string email, string gender)
	{
		Debug.Log("No Global user found... Creating new user");

		string post_url = createGlobalUserURL + "firstname=" + WWW.EscapeURL(firstName) +
										  "&lastname=" + WWW.EscapeURL(lastName) +
										  "&email=" + email +
										  "&gender=" + gender +
										  "&token_for_business=" + token_for_business;
		
		Debug.Log(string.Format("CreateGlobalUser URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW createUserID_post = new WWW(post_url);
		yield return createUserID_post; // Wait until the download is done
	}
	#endregion


	#region GAME
	public static IEnumerator CheckAndCreateGameUser(string facebookID, int globalID)
	{
		Debug.Log("Checking if Game User exists...");
		dbUser = null;
		yield return Instance.StartCoroutine(GetGameUser(facebookID));
		
		if(dbUser == null)
		{
			yield return Instance.StartCoroutine(CreateGameUser(facebookID, globalID));
			yield return Instance.StartCoroutine(GetGameUser(facebookID));
		}
		else
			Debug.Log("Game user already exists");
	}
	
	private static IEnumerator GetGameUser(string facebookID)
	{
		Debug.Log("DBHandler.GetGameUser()");
		string post_url = getGameUserURL + "facebookID=" + facebookID;
		Debug.Log(string.Format("GetGameUser URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW getUser_post = new WWW(post_url);
		yield return getUser_post; // Wait until the download is done
		
		if (getUser_post.error != null)
			Debug.Log("There was an error posting the GetUser : " + getUser_post.error);
		
		Debug.Log(string.Format("WWW post: {0}", getUser_post.text));
		
		if(!string.IsNullOrEmpty(getUser_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getUser_post.text) as Dictionary<string, object>;

			dbUser = new DBUser(int.Parse(data["id"].ToString()),
			                    data["facebookID"].ToString(),
			                    int.Parse(data["score"].ToString()));
		}
	}
	
	private static IEnumerator CreateGameUser(string facebookID, int globalID)
	{
		Debug.Log("No Game user found... Creating new user");
		
		string post_url = createGameUserURL + "facebookID=" + facebookID +
											  "&globalID=" + globalID;
		
		Debug.Log(string.Format("CreateGameUser URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW createUserID_post = new WWW(post_url);
		yield return createUserID_post; // Wait until the download is done
	}
	#endregion

	public static IEnumerator GetUserGlobalRanking(int userID, System.Action<int> result)
	{
		int position = -1;

		Debug.Log("DBHandler.GetUserRanking()");
		
		string post_url = getUserGlobalRankingURL + "userID=" + userID;

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

	public static IEnumerator UpdateUserScore(int userID, float score)
	{
		Debug.Log("Updating user score");

		string post_url = updateUserScoreURL + "userID=" + userID + 
											   "&score=" + score;
		
		Debug.Log(string.Format("UpdateUserScore URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW updateUserScore_post = new WWW(post_url);
		yield return updateUserScore_post; // Wait until the download is done
	}

	public static IEnumerator GetUserScore(string facebookID, System.Action<int> result)
	{
		int score = 0;
		
		Debug.Log("DBHandler.GetUserScore()");
		
		string post_url = getUserScoreURL + "facebookID=" + facebookID;
		
		Debug.Log(string.Format("GetUserScore URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW getUserScore_post = new WWW(post_url);
		yield return getUserScore_post; // Wait until the download is done
		
		if (getUserScore_post.error != null)
			Debug.Log("There was an error posting the GetUserRankingPosition : " + getUserScore_post.error);
		
		Debug.Log(string.Format("GetUserScore WWW post: {0}", getUserScore_post.text));
		
		if(!string.IsNullOrEmpty(getUserScore_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getUserScore_post.text) as Dictionary<string, object>;
			
			score = int.Parse(data["score"].ToString());
		}	
		
		Debug.Log(string.Format("score: {0}", score));
		
		result(score);
	}

	public static IEnumerator GetTopUsers(System.Action<List<FacebookFriend>> result)
	{
		Debug.Log("DBHandler.GetTopUsers()");
		
		string post_url = getTopUsersURL;
		
		Debug.Log(string.Format("GetTopUsers URL: {0}", post_url));
		
		// Post the URL to the site and create a download object to get the result.
		WWW getTopUsers_post = new WWW(post_url);
		yield return getTopUsers_post; // Wait until the download is done
		
		if (getTopUsers_post.error != null)
			Debug.Log("There was an error posting the GetUserRankingPosition : " + getTopUsers_post.error);
		
		Debug.Log(string.Format("GetTopUsers WWW post: {0}", getTopUsers_post.text));

		List<FacebookFriend> topUsers = new List<FacebookFriend>();

		if(!string.IsNullOrEmpty(getTopUsers_post.text))
		{
			Dictionary<string, object> data = Json.Deserialize(getTopUsers_post.text) as Dictionary<string, object>;

			List<System.Object> users = data["data"] as List<System.Object>;

			foreach(System.Object user in users)
			{
				Dictionary<string, object> userData = user as Dictionary<string, object>;

				topUsers.Add(new FacebookFriend(userData["facebookID"].ToString(), 
				                                userData["name"].ToString(), 
				                                int.Parse(userData["score"].ToString()),
				                                int.Parse(userData["rank"].ToString())));
			}
		}	

		result(topUsers);
	}
	#endif
}

public class DBUser
{
	public int id;
	public string facebookID;
	public int score;
	
	public DBUser()
	{
		
	}
	
	public DBUser(int id, string facebookID, int score)
	{
		this.id = id;
		this.facebookID = facebookID;
		this.score = score;
	}
	
	public override string ToString()
	{
		return 	"{id: " + id + "; " +
				"facebookID: " + facebookID + "; " + 
				"score: " + score + "}";
	}
}