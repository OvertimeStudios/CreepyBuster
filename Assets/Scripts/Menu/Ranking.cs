using UnityEngine;
using System.Collections;

public class Ranking : MonoBehaviour 
{
	public UILabel highScore;
	public UILabel worldRank;

	void OnEnable()
	{
		highScore.text = Global.HighScore.ToString();
		worldRank.text = Localization.Get("NOT_LOGGED");

		#if FACEBOOK_IMPLEMENTED && DB_IMPLEMENTED
		if(FB.IsLoggedIn)
		{
			worldRank.text = Localization.Get("LOADING");
			StartCoroutine(GetRank());
		}
		#endif
	}

	private IEnumerator GetRank()
	{
		Debug.Log("Getting ranking...");
		int rank = 0;
		yield return StartCoroutine(DBHandler.GetUserGlobalRanking(DBHandler.User.id, value => rank = value));

		worldRank.text = "#" + rank;
	}
}
