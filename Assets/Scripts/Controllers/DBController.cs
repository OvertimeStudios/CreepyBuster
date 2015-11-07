using UnityEngine;
using System.Collections;

public class DBController : MonoBehaviour 
{
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
		yield return StartCoroutine(DBHandler.CheckAndCreateGlobalUser(fbUser.tokenForBusiness, fbUser.firstname, fbUser.lastname, fbUser.email, fbUser.gender));
		#endregion

		#region Game User
		yield return StartCoroutine(DBHandler.CheckAndCreateGameUser(fbUser.id));
		#endregion

		#region Update Game Score
		if(DBHandler.User.score > Global.HighScore)
			Global.HighScore = (int)DBHandler.User.score;
		else if(Global.HighScore > DBHandler.User.score)
			yield return StartCoroutine(DBHandler.UpdateUserScore(DBHandler.User.id, (float)Global.HighScore));
		#endregion
	}
	#endif

}
