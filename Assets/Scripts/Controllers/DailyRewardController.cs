using UnityEngine;
using System;
using System.Collections;

public class DailyRewardController : MonoBehaviour 
{
	public GameObject dailyRewardObject;
	public int[] orbsReward;

	private int orbsToCollect;

	public int rewardCooldown;
	private static DateTime rewardCooldownTime;

	#region get / set
	public DateTime RewardCooldownTime
	{
		get 
		{
			if (object.Equals(rewardCooldownTime,default(DateTime)))
			{
				if(!string.IsNullOrEmpty(Global.DailyRewardNextTime))
					rewardCooldownTime = DateTime.Parse(Global.DailyRewardNextTime);
				else 
					rewardCooldownTime = DateTime.UtcNow;
			}
			
			return rewardCooldownTime;
		}
	}

	public int RewardCooldownLeft
	{
		get
		{
			return (int)RewardCooldownTime.Subtract(DateTime.UtcNow).TotalSeconds;
		}
	}
	
	public bool IsReady
	{
		get { return RewardCooldownLeft <= 0; }
	}
	#endregion

	#region cooldown function
	public static void SetRewardCooldownTime (DateTime dateTime)
	{
		rewardCooldownTime = dateTime;
		Global.DailyRewardNextTime = dateTime.ToString();
	}
	#endregion

	// Use this for initialization
	void OnEnable () 
	{
		Transform rewards = dailyRewardObject.transform.FindChild("Rewards");

		for(byte i = 0; i < rewards.childCount; i++)
		{
			Transform t = rewards.GetChild(i);
			GameObject blue = t.FindChild("Blue").gameObject;
			GameObject green = t.FindChild("Green").gameObject;
			GameObject gray = t.FindChild("Gray").gameObject;
			UILabel day = t.FindChild("Day").GetComponent<UILabel>();
			UILabel orbs = t.FindChild("Orbs").GetComponent<UILabel>();
			TweenScale tween = t.GetComponent<TweenScale>();
			UIButton button = t.GetComponent<UIButton>();


			blue.SetActive(false);
			green.SetActive(false);
			gray.SetActive(false);

			if(i < Global.DailyRewardDay)
				gray.SetActive(true);
			else if(i == Global.DailyRewardDay)
			{
				orbsToCollect = orbsReward[i];
				tween.enabled = true;
				button.enabled = true;
				green.SetActive(true);
			}
			else
				blue.SetActive(true);

			day.text = string.Format("{0} {1}", Localization.Get("DAY"), i + 1);
			orbs.text = orbsReward[i] + "\n" + Localization.Get("ORBS");
		}
	}

	void Start()
	{
		CheckReward();
	}

	private void CheckReward() 
	{
		//Debug.Log(string.Format("Lefting {0} hours.", RewardCooldownLeft/3600f));

		if(RewardCooldownLeft/3600f < -24f)
			Global.DailyRewardDay = 0;

		if(IsReady)
			ShowReward();
	}

	private void ShowReward()
	{
		dailyRewardObject.SetActive(true);
	}

	public void CollectReward()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		Debug.Log(string.Format("Collecting {0} orbs from daily rewards.", orbsToCollect));

		Global.TotalOrbs += orbsToCollect;

		Global.DailyRewardDay++;

		if(Global.DailyRewardDay > 6)
			Global.DailyRewardDay = 0;

		SetRewardCooldownTime(DateTime.UtcNow.AddHours(24f));

		dailyRewardObject.SetActive(false);
	}
}
