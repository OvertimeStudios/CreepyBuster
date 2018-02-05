using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendsRanking : MonoBehaviour 
{
	enum Leaderboard
	{
		AllTime,
		Daily,
		None,
	}

	private List<FacebookFriend> topUsers;

	public GameObject goldRank;
	public GameObject silverRank;
	public GameObject cooperRank;
	public GameObject normalRank;

	private Leaderboard activeLeaderboard = Leaderboard.None;
	private bool isLoaded = true;

	public UILabel allTimeButton;
	public UILabel dailyButton;
	public Color activeColor;
	public Color inactiveColor;

	void OnEnable()
	{
		activeLeaderboard = Leaderboard.None;
		isLoaded = true;

		StartCoroutine(BuildAllTimeRank());
	}

	private IEnumerator BuildAllTimeRank()
	{
		if(activeLeaderboard == Leaderboard.AllTime || !isLoaded) yield break;

		activeLeaderboard = Leaderboard.AllTime;
		isLoaded = false;

		allTimeButton.color = activeColor;
		dailyButton.color = inactiveColor;

		UIGrid grid = GetComponentInChildren<UIGrid>();
		grid.transform.DestroyChildren();

		transform.Find("Loading").gameObject.SetActive(true);

		yield return StartCoroutine(GameSparksController.GetAllTimeFriendsList((list) => topUsers = list));

		for(byte i = 0; i < topUsers.Count; i++)
		{
			FacebookFriend user = topUsers[i];

			GameObject rank;
			if(user.rank == 1)
				rank = Instantiate(goldRank) as GameObject;
			else if(user.rank == 2)
				rank = Instantiate(silverRank) as GameObject;
			else if(user.rank == 3)
				rank = Instantiate(cooperRank) as GameObject;
			else
				rank = Instantiate(normalRank) as GameObject;

			UILabel nomeLabel = rank.transform.Find("Name").GetComponent<UILabel>();
			UILabel scoreLabel = rank.transform.Find("Score").GetComponent<UILabel>();
			UILabel rankLabel = rank.transform.Find("Rank").GetComponent<UILabel>();
			UITexture picture = rank.transform.Find("Picture").GetComponent<UITexture>();

			nomeLabel.text = user.name;
			scoreLabel.text = Localization.Get("SCORE") + ": " + user.score;
			rankLabel.text = "Rank #" + user.rank;
			StartCoroutine(LoadProfilePicture(user, picture));

			rank.transform.parent = grid.transform;
			rank.transform.localScale = Vector3.one;
		}

		grid.GetComponent<UIGrid>().Reposition();

		transform.Find("Loading").gameObject.SetActive(false);

		isLoaded = true;
	}

	private IEnumerator BuildDailyRank()
	{
		if(activeLeaderboard == Leaderboard.Daily || !isLoaded) yield break;

		activeLeaderboard = Leaderboard.Daily;
		isLoaded = false;

		allTimeButton.color = inactiveColor;
		dailyButton.color = activeColor;

		UIGrid grid = GetComponentInChildren<UIGrid>();
		grid.transform.DestroyChildren();

		transform.Find("Loading").gameObject.SetActive(true);

		yield return StartCoroutine(GameSparksController.GetDailyFriendsList((list) => topUsers = list));

		for(byte i = 0; i < topUsers.Count; i++)
		{
			FacebookFriend user = topUsers[i];

			GameObject rank;
			if(user.rank == 1)
				rank = Instantiate(goldRank) as GameObject;
			else if(user.rank == 2)
				rank = Instantiate(silverRank) as GameObject;
			else if(user.rank == 3)
				rank = Instantiate(cooperRank) as GameObject;
			else
				rank = Instantiate(normalRank) as GameObject;

			UILabel nomeLabel = rank.transform.Find("Name").GetComponent<UILabel>();
			UILabel scoreLabel = rank.transform.Find("Score").GetComponent<UILabel>();
			UILabel rankLabel = rank.transform.Find("Rank").GetComponent<UILabel>();
			UITexture picture = rank.transform.Find("Picture").GetComponent<UITexture>();

			nomeLabel.text = user.name;
			scoreLabel.text = Localization.Get("SCORE") + ": " + user.score;
			rankLabel.text = "Rank #" + user.rank;
			StartCoroutine(LoadProfilePicture(user, picture));

			rank.transform.parent = grid.transform;
			rank.transform.localScale = Vector3.one;
		}

		grid.GetComponent<UIGrid>().Reposition();

		transform.Find("Loading").gameObject.SetActive(false);

		isLoaded = true;
	}

	private IEnumerator LoadProfilePicture(FacebookFriend user, UITexture texture)
	{
		Debug.Log("Getting profile picture for friend " + user.name);
		Texture profilePicture = null;
		yield return StartCoroutine(user.GetProfilePicture(value => profilePicture = value));

		Debug.Log("Finished loading profile picture for friend " + user.name);

		texture.mainTexture = profilePicture;
	}

	public void AllTimeClicked()
	{
		StartCoroutine(BuildAllTimeRank());
	}

	public void DailyClicked()
	{
		StartCoroutine(BuildDailyRank());
	}
}
