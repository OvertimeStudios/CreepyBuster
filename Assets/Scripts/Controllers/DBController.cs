using UnityEngine;
using System.Collections;

public class DBController : MonoBehaviour 
{
	public string gameName;

	public static int gameID;

	#region singleton
	private static DBController instance;
	public static DBController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<DBController>();

			return instance;
		}
	}
	#endregion

	#if FACEBOOK_IMPLEMENTED && DB_IMPLEMENTED
	void OnEnable()
	{
		FacebookController.OnLoggedIn += ConnectDB;
	}

	void OnDisable()
	{
		FacebookController.OnLoggedIn -= ConnectDB;
	}

	public void ConnectDB()
	{
		Debug.Log("ConnectDB()");

		StartCoroutine(GetAllInformation());
	}

	private IEnumerator GetAllInformation()
	{
		#region Game ID
		yield return StartCoroutine(DBHandler.GetGameID(gameName, value => gameID = value));
		Debug.Log(string.Format("Game ID: {0}", gameID));
		#endregion

		#region Get User
		FacebookUser fbUser = FacebookController.User;
		DBUser user = null;
		yield return StartCoroutine(DBHandler.GetUser(fbUser.tokenForBusiness, value => user = value));
		#endregion

		#region Create User
		if(user == null)
		{
			yield return StartCoroutine(DBHandler.CreateUser(fbUser.tokenForBusiness, fbUser.firstname, fbUser.lastname, fbUser.email, fbUser.gender));
			yield return StartCoroutine(DBHandler.GetUser(fbUser.tokenForBusiness, value => user = value));
		}
		#endregion

		#region Get User Ranking
		int ranking = -1;
		yield return StartCoroutine(DBHandler.GetUserRanking(user.id, gameID, value => ranking = value));
		#endregion

		#region Create User Score
		if(ranking == -1)
			yield return StartCoroutine(DBHandler.CreateUserRanking(user.id, gameID));
		#endregion

		#region Get User Score
		else
		{
			float score = -1;
			yield return StartCoroutine(DBHandler.GetUserScore(user.id, gameID, value => score = value));

			if(score > Global.HighScore)
				Global.HighScore = (int)score;
			else if(Global.HighScore > score)
				yield return StartCoroutine(DBHandler.UpdateUserScore(user.id, gameID, (float)Global.HighScore));
		}
		#endregion
	}
	#endif

}
