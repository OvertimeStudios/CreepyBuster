using UnityEngine;
using System.Collections;

public class DBController : MonoBehaviour 
{
	public static bool allInformationLoaded;

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
		FacebookUser fbUser = FacebookController.User;

		#region Global User
		int globalID = 0;
		yield return StartCoroutine(DBHandler.CheckAndCreateGlobalUser(fbUser.tokenForBusiness, fbUser.firstname, fbUser.lastname, fbUser.email, fbUser.gender, value => globalID = value));
		#endregion

		#region Game User
		yield return StartCoroutine(DBHandler.CheckAndCreateGameUser(fbUser.id, globalID));
		#endregion

		#region Update Game Score
		//TODO: retrieve from cloud save
		/*if(DBHandler.User.score > Global.HighScore)
			Global.HighScore = (int)DBHandler.User.score;
		else if(Global.HighScore > DBHandler.User.score)
			yield return StartCoroutine(DBHandler.UpdateUserScore(DBHandler.User.id, (float)Global.HighScore));*/
		#endregion

		FacebookController.User.score = Global.HighScore;

		Debug.Log("Adding myself as friend");
		//HACK!!! Add myself as my friend for ranking
		FacebookController.User.AddFriend(new FacebookFriend(FacebookController.User.id, string.Format("{0} {1}", FacebookController.User.firstname, FacebookController.User.lastname), Global.HighScore));

		allInformationLoaded = true;
	}
	#endif

}
