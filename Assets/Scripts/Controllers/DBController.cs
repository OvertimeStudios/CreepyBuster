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
		Debug.Log("DBHandler.Connect()");
		DBHandler.Connect();

		//Get game id
		gameID = DBHandler.GetGameID(gameName);
		Debug.Log("Game ID: " + gameID);

		FacebookUser fbUser = FacebookController.User;

		DBUser user = DBHandler.GetUser(fbUser.tokenForBusiness);

		if(user == null)//no user was found
			user = DBHandler.CreateUser(fbUser.tokenForBusiness, fbUser.firstname, fbUser.lastname, fbUser.email, fbUser.gender);

		if(DBHandler.GetUserRanking(user.id, gameID) == -1)
			DBHandler.CreateUserScore(user.id, gameID);
		
		Debug.Log(user.ToString());
	}
	#endif

}
