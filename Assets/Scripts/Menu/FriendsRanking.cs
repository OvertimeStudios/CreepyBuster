using UnityEngine;
using System.Collections;

public class FriendsRanking : MonoBehaviour 
{
	public GameObject goldRank;
	public GameObject silverRank;
	public GameObject cooperRank;
	public GameObject normalRank;

	void OnEnable()
	{
		UIGrid grid = GetComponentInChildren<UIGrid>();
		grid.transform.DestroyChildren();

		StartCoroutine(BuildRank());
	}
	
	IEnumerator BuildRank()
	{
		FacebookController.User.friendsScoreLoaded = false;

		foreach(FacebookFriend friend in FacebookController.User.friends)
			StartCoroutine(friend.UpdateScore());

		while(!FacebookController.User.friendsScoreLoaded)
			yield return null;

		UIGrid grid = GetComponentInChildren<UIGrid>();

		for(byte i = 0; i < FacebookController.User.friends.Count; i++)
		{
			FacebookFriend friend = FacebookController.User.friends[i];
			
			GameObject rank;
			if(friend.rank == 1)
				rank = Instantiate(goldRank) as GameObject;
			else if(friend.rank == 2)
				rank = Instantiate(silverRank) as GameObject;
			else if(friend.rank == 3)
				rank = Instantiate(cooperRank) as GameObject;
			else
				rank = Instantiate(normalRank) as GameObject;

			UILabel nomeLabel = rank.transform.FindChild("Name").GetComponent<UILabel>();
			UILabel scoreLabel = rank.transform.FindChild("Score").GetComponent<UILabel>();
			UILabel rankLabel = rank.transform.FindChild("Rank").GetComponent<UILabel>();
			UITexture picture = rank.transform.FindChild("Picture").GetComponent<UITexture>();

			nomeLabel.text = friend.name;
			scoreLabel.text = Localization.Get("SCORE") + ": " + friend.score;
			rankLabel.text = "Rank #" + friend.rank;
			StartCoroutine(LoadProfilePicture(friend, picture));

			rank.transform.parent = grid.transform;
			rank.transform.localScale = Vector3.one;
		}
		
		grid.GetComponent<UIGrid>().Reposition();
	}

	private IEnumerator LoadProfilePicture(FacebookFriend friend, UITexture texture)
	{
		Debug.Log("Getting profile picture for friend " + friend.name);
		Texture profilePicture = null;
		yield return StartCoroutine(friend.GetProfilePicture(value => profilePicture = value));

		Debug.Log("Finished loading profile picture for friend " + friend.name);

		texture.mainTexture = profilePicture;
	}
}
