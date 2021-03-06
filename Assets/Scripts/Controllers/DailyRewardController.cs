﻿using UnityEngine;
using System;
using System.Collections;

public class DailyRewardController : MonoBehaviour 
{
	private const string DAILY_REWARD_DAY = "dailyRewardDay";
	private const string DAILY_REWARD_NEXT_TIME = "dailyRewardNextTime";

	public GameObject dailyRewardObject;
	public int[] orbsReward;

	private int orbsToCollect;

	private int rewardCooldown;
	private static DateTime rewardCooldownTime;

	#region get / set
	public static int DailyRewardDay
	{
		get
		{
			if(!PlayerPrefs.HasKey(DAILY_REWARD_DAY))
				return 0;

			return PlayerPrefs.GetInt(DAILY_REWARD_DAY);
		}
		set
		{
			PlayerPrefs.SetInt(DAILY_REWARD_DAY, value);
			PlayerPrefs.Save();
		}
	}

	public static string DailyRewardNextTime
	{
		get
		{
			if(!PlayerPrefs.HasKey(DAILY_REWARD_NEXT_TIME))
				return "";

			return PlayerPrefs.GetString(DAILY_REWARD_NEXT_TIME);
		}
		set
		{
			PlayerPrefs.SetString(DAILY_REWARD_NEXT_TIME, value);
			PlayerPrefs.Save();
		}
	}

	public DateTime RewardCooldownTime
	{
		get 
		{
			if (object.Equals(rewardCooldownTime,default(DateTime)))
			{
				if(!string.IsNullOrEmpty(DailyRewardNextTime))
					rewardCooldownTime = DateTime.Parse(DailyRewardNextTime);
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

	public static bool IsActive
	{
		get { return Instance.dailyRewardObject.activeInHierarchy; }
	}
	#endregion

	#region singleton
	private static DailyRewardController instance;
	public static DailyRewardController Instance
	{
		get 
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<DailyRewardController>();

			return instance;
		}
	}
	#endregion

	#region cooldown function
	public static void SetRewardCooldownTime (DateTime dateTime)
	{
		rewardCooldownTime = dateTime;
		DailyRewardNextTime = dateTime.ToString();
	}
	#endregion

	void Start()
	{
		CheckReward();
	}

	private void CheckReward() 
	{
		if(RewardCooldownLeft/3600f <= -24f)
		{
			DailyRewardDay = 0;
			SetRewardCooldownTime(DateTime.UtcNow);
		}
		
		if(IsReady)
			ShowReward();
	}

	private void ShowReward()
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
			
			if(i < DailyRewardDay)
				gray.SetActive(true);
			else if(i == DailyRewardDay)
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

		dailyRewardObject.SetActive(true);
	}

	public void CollectReward()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		Debug.Log(string.Format("Collecting {0} orbs from daily rewards.", orbsToCollect));

		Global.TotalOrbs += orbsToCollect;

		DailyRewardDay++;

		if(DailyRewardDay > 6)
			DailyRewardDay = 0;

		Debug.Log("CollectReward()");
		SetRewardCooldownTime(RewardCooldownTime.AddHours(24f));

		dailyRewardObject.SetActive(false);
	}
}
