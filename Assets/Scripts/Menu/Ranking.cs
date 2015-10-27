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

		#if FACEBOOK_IMPLEMENTED
		if(FacebookController.IsLoggedIn)
			worldRank.text = "#" + DBHandler.GetUserRanking(DBHandler.User.id, DBController.gameID);
		#endif
	}
}
