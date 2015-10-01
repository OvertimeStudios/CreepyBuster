using UnityEngine;
using System.Collections;

public class Ranking : MonoBehaviour 
{
	public UILabel highScore;
	public UILabel worldRank;

	void OnEnable()
	{
		highScore.text = Global.HighScore.ToString();

		if(FacebookController.IsLoggedIn)
			worldRank.text = "#" + DBHandler.GetUserRanking(DBHandler.User.id, DBController.gameID);
		else
			worldRank.text = "Not logged in";
	}
}
