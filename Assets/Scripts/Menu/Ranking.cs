using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if FACEBOOK_IMPLEMENTED
using Facebook.Unity;
#endif

public class Ranking : MonoBehaviour 
{
	public UILabel highScore;
	public UILabel worldRank;
	public UILabel friendsRank;

	private bool listSorted = false;

	public GameObject general;

	public GameObject globalInfo;
	public GameObject friendsInfo;

	public GameObject globalLogin;
	public GameObject friendsLogin;

	void OnEnable()
	{
		highScore.text = Global.HighScore.ToString();
		worldRank.text = "";//Localization.Get("NOT_LOGGED");
		friendsRank.text = "";//Localization.Get("NOT_LOGGED");

		general.SetActive(true);
		
		globalInfo.SetActive(FacebookHelper.IsUserLoggedIn);
		friendsInfo.SetActive(FacebookHelper.IsUserLoggedIn);

		#if LEADERBOARDS_IMPLEMENTED
		if(FacebookHelper.IsUserLoggedIn)
			GetRanks();
		#endif
	}

	void OnDestroy()
	{
		#if LEADERBOARDS_IMPLEMENTED
		FacebookController.OnJustLoggedIn -= GetRanks;
		#endif
	}

	void Start()
	{
		
	}

	public void AuthenticatePlayer()
	{
		#if LEADERBOARDS_IMPLEMENTED
		FacebookController.Instance.Login();
		#endif
	}

	private void GetRanks()
	{
		worldRank.text = Localization.Get("LOADING");
		friendsRank.text = Localization.Get("LOADING");

		globalLogin.SetActive(false);
		friendsLogin.SetActive(false);

		StartCoroutine(LoadRanks());
	}

	private IEnumerator LoadRanks()
	{
		//yield return StartCoroutine(GetGlobalRank());
		yield return StartCoroutine(GetFriendsRank());
	}

	private IEnumerator GetGlobalRank()
	{
		Debug.Log("Getting Global ranking...");

		#if LEADERBOARDS_IMPLEMENTED
		yield return null;
		#else
		yield return null;
		#endif
	}

	private IEnumerator GetFriendsRank()
	{
		Debug.Log("Getting Friends ranking...");

		#if LEADERBOARDS_IMPLEMENTED
		yield return StartCoroutine(FacebookHelper.GetAllTimeFriendsRank(SetFriendsRank));
		#else
		yield return null;
		#endif
	}

	private void SetGlobalRank(int rank)
	{
		Debug.Log(string.Format("Rankings.SetGlobalRank({0})", rank));
		//TODO: Localization
		worldRank.text = (rank <= 0) ? "Error" : "#" + rank;
		//worldRank.text += (maxRange > 0) ? " of " + maxRange : "";
	}

	private void SetFriendsRank(int rank)
	{
		Debug.Log(string.Format("Rankings.SetFriendsRank({0})", rank));
		//TODO: Localization
		friendsRank.text = (rank <= 0) ? "Error" : "#" + rank;
		//friendsRank.text += (maxRange > 0) ? " of " + maxRange : "";
	}

	//old methods
	/*private IEnumerator GetGlobalRank()
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
	}*/

	public void OpenGlobalRank()
	{
		#if LEADERBOARDS_IMPLEMENTED
		LeaderboardsHelper.OpenLeaderboards();
		#endif
		//general.SetActive(false);
		//globalRanking.SetActive(true);
	}

	public void OpenFriendsRank()
	{
		#if LEADERBOARDS_IMPLEMENTED
		LeaderboardsHelper.OpenLeaderboards();
		#endif
		//general.SetActive(false);
		//friendsRanking.SetActive(true);
	}

	public void CloseRank()
	{
		general.SetActive(true);
		//globalRanking.SetActive(false);
		//friendsRanking.SetActive(false);
	}
}
