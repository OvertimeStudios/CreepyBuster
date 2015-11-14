using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalRanking : MonoBehaviour 
{
	private List<FacebookFriend> topUsers;

	public GameObject goldRank;
	public GameObject silverRank;
	public GameObject cooperRank;
	public GameObject normalRank;
	
	private bool loaded;
	
	void OnEnable()
	{
		UIGrid grid = GetComponentInChildren<UIGrid>();
		grid.transform.DestroyChildren();

		StartCoroutine(BuildRank());
	}
	
	private IEnumerator BuildRank()
	{
		transform.FindChild("Loading").gameObject.SetActive(true);

		yield return StartCoroutine(DBHandler.GetTopUsers(value => topUsers = value));

		UIGrid grid = GetComponentInChildren<UIGrid>();
		
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
			
			UILabel nomeLabel = rank.transform.FindChild("Name").GetComponent<UILabel>();
			UILabel scoreLabel = rank.transform.FindChild("Score").GetComponent<UILabel>();
			UILabel rankLabel = rank.transform.FindChild("Rank").GetComponent<UILabel>();
			UITexture picture = rank.transform.FindChild("Picture").GetComponent<UITexture>();
			
			nomeLabel.text = user.name;
			scoreLabel.text = Localization.Get("SCORE") + ": " + user.score;
			rankLabel.text = "Rank #" + user.rank;
			StartCoroutine(LoadProfilePicture(user, picture));
			
			rank.transform.parent = grid.transform;
			rank.transform.localScale = Vector3.one;
		}

		bool isInTop10 = false;
		foreach(FacebookFriend topUser in topUsers)
		{
			if(topUser.id == FacebookController.User.id)
			{
				isInTop10 = true;
				break;
			}
		}

		if(!isInTop10)
		{
			FacebookFriend player = new FacebookFriend(FacebookController.User.id,
			                                           FacebookController.User.firstname + " " + FacebookController.User.lastname,
			                                           FacebookController.User.score,
			                                           FacebookController.User.rank);

			GameObject rank = Instantiate(normalRank) as GameObject;
			
			UILabel nomeLabel = rank.transform.FindChild("Name").GetComponent<UILabel>();
			UILabel scoreLabel = rank.transform.FindChild("Score").GetComponent<UILabel>();
			UILabel rankLabel = rank.transform.FindChild("Rank").GetComponent<UILabel>();
			UITexture picture = rank.transform.FindChild("Picture").GetComponent<UITexture>();
			
			nomeLabel.text = player.name;
			scoreLabel.text = Localization.Get("SCORE") + ": " + player.score;
			rankLabel.text = "Rank #" + player.rank;
			StartCoroutine(LoadProfilePicture(player, picture));
			
			rank.transform.parent = grid.transform;
			rank.transform.localScale = Vector3.one;
		}
		
		grid.GetComponent<UIGrid>().Reposition();

		transform.FindChild("Loading").gameObject.SetActive(false);
	}
	
	private IEnumerator LoadProfilePicture(FacebookFriend user, UITexture texture)
	{
		Debug.Log("Getting profile picture for friend " + user.name);
		Texture profilePicture = null;
		yield return StartCoroutine(user.GetProfilePicture(value => profilePicture = value));
		
		Debug.Log("Finished loading profile picture for friend " + user.name);
		
		texture.mainTexture = profilePicture;
	}
}
