using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour 
{
	[Header("Shop Achievement")]
	public Achievement achievement;

	// Use this for initialization
	void Start () 
	{
		ItemShop.OnItemBought += VerifyAchievement;
	}

	private void VerifyAchievement()
	{
		Global.UpgradesBought++;

		if(!achievement.unlocked && Global.UpgradesBought == achievement.value)
		{
			achievement.Unlock();
			MenuController.Instance.ShowAchievements();
		}
	}
}
