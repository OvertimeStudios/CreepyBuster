using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Achievements : MonoBehaviour 
{
	public List<AchievementStats> achievementsList;

	public GameObject achievementPrefab;

	// Use this for initialization
	void Start () 
	{
		Transform grid = transform.FindChild ("Scroll View").FindChild("Grid");

		foreach(AchievementStats achievement in achievementsList)
		{
			Transform newAchievement = (Instantiate(achievementPrefab) as GameObject).transform;
			newAchievement.parent = grid;
			newAchievement.localScale = Vector3.one;

			newAchievement.FindChild("Title").GetComponent<UILabel>().text = (achievement.hidden && !achievement.unlocked) ? "???????" : achievement.title;
			newAchievement.FindChild("Description").GetComponent<UILabel>().text = (achievement.hidden && !achievement.unlocked) ? " ?????? \n ??????" : achievement.description;
			newAchievement.FindChild("Icon").GetComponent<UISprite>().spriteName = achievement.iconName;

			if(!achievement.unlocked)
				newAchievement.FindChild("Icon").GetComponent<UISprite>().color = Color.grey;
		}

		grid.GetComponent<UIGrid> ().Reposition ();
	}
}

[System.Serializable]
public class AchievementStats
{
	public string title;
	public string iconName = "Achievement";
	public string description;
	public bool unlocked;
	public bool hidden;
}