using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ranking : MonoBehaviour 
{
	public UILabel highScore;
	public UILabel worldRank;
	public UILabel friendsRank;

	private bool listSorted = false;

	public GameObject general;
	public GameObject globalRanking;
	public GameObject friendsRanking;

	public GameObject globalInfo;
	public GameObject friendsInfo;

	void OnEnable()
	{
		highScore.text = Global.HighScore.ToString();
		worldRank.text = Localization.Get("NOT_LOGGED");
		friendsRank.text = Localization.Get("NOT_LOGGED");

		general.SetActive(true);
		globalRanking.SetActive(false);
		friendsRanking.SetActive(false);
		
		globalInfo.SetActive(false);
		friendsInfo.SetActive(false);

		#if FACEBOOK_IMPLEMENTED && DB_IMPLEMENTED
		if(FB.IsLoggedIn)
		{
			worldRank.text = Localization.Get("LOADING");
			friendsRank.text = Localization.Get("LOADING");
			StartCoroutine(GetGlobalRank());
			StartCoroutine(GetFriendsRank());
		}
		#endif
	}

	private IEnumerator GetGlobalRank()
	{
		Debug.Log("Getting Global ranking...");
		while(!DBController.allInformationLoaded)
			yield return null;

		int rank = 0;
		yield return StartCoroutine(DBHandler.GetUserGlobalRanking(DBHandler.User.id, value => rank = value));

		FacebookController.User.rank = rank;

		worldRank.text = "#" + rank;
		globalInfo.SetActive(true);
	}

	private IEnumerator GetFriendsRank()
	{
		Debug.Log("Getting Friends ranking...");

		if(!listSorted)
		{
			if(!FacebookController.User.friendsScoreLoaded)
				Debug.Log("Not all friends score are loaded");

			while(!FacebookController.User.friendsScoreLoaded || !DBController.allInformationLoaded)
				yield return null;

			string list = "Friends before sort";
			for(byte i = 0; i < FacebookController.User.friends.Count; i++)
			{
				FacebookFriend friend = FacebookController.User.friends[i];
				list += "\n" + friend.ToString();
			}

			Debug.Log(list);

			FacebookController.User.friends.Sort();
			FacebookController.User.friends.Reverse();

			for(byte i = 0; i < FacebookController.User.friends.Count; i++)
			{
				FacebookFriend friend = FacebookController.User.friends[i];

				friend.rank = (i + 1);

				if(i > 0)
				{
					FacebookFriend lastFriend = FacebookController.User.friends[i - 1];
					if(friend.score == lastFriend.score)
						friend.rank = lastFriend.rank;
				}
			}

			list = "Friends after sort";
			for(byte i = 0; i < FacebookController.User.friends.Count; i++)
			{
				FacebookFriend friend = FacebookController.User.friends[i];
				list += "\n" + friend.ToString();
			}
			
			Debug.Log(list);

			listSorted = true;
		}

		int rank = 0;
		foreach(FacebookFriend friend in FacebookController.User.friends)
		{
			if(friend.id == FacebookController.User.id)
				rank = friend.rank;
		}

		friendsRank.text = "#" + rank;
		friendsInfo.SetActive(true);
	}

	public void OpenGlobalRank()
	{
		general.SetActive(false);
		globalRanking.SetActive(true);
	}

	public void OpenFriendsRank()
	{
		general.SetActive(false);
		friendsRanking.SetActive(true);
	}

	public void CloseRank()
	{
		general.SetActive(true);
		globalRanking.SetActive(false);
		friendsRanking.SetActive(false);
	}
}
