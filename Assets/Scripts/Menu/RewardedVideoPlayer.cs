using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RewardedVideoPlayer : MonoBehaviour 
{
	#region Action
	public static event Action OnRevivePlayer;
	#endregion
	
	#region get / set
	public DateTime RewardCooldownTime
	{
		get 
		{
			if (object.Equals(rewardCooldownTime,default(DateTime)))
			{
				if (PlayerPrefs.HasKey(Global.REWARDED_VIDEO_COOLDOWN))
					rewardCooldownTime = DateTime.Parse(PlayerPrefs.GetString(Global.REWARDED_VIDEO_COOLDOWN));
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
		get 
		{
			return RewardCooldownLeft <= 0 && UnityAdsHelper.IsReady();
		}
	}
	#endregion
	
	public enum Rewards
	{
		Orbs,
		PlayAgain,
	}
	
	public Rewards reward;
	
	public int orbsToGive;
	
	public int rewardCooldown;
	private static DateTime rewardCooldownTime;
	
	private UILabel countdown;
	
	void Start()
	{
		rewardCooldownTime = RewardCooldownTime;

		countdown = transform.FindChild ("Countdown").GetComponent<UILabel> ();
	}
	
	public void Play()
	{
		#if UNITYADS_IMPLEMENTED
		if(IsReady)
		{
			if(reward == Rewards.PlayAgain)
				ShowAd();
			else
				Ask();
		}
		else if(UnityAdsHelper.isSupported)
			Popup.ShowOk("Ads not supported");
		else if(UnityAdsHelper.isInitialized)
			Popup.ShowOk("Ads not loaded yet");
		else
			Popup.ShowOk("Ads failed");
		#endif
	}

	private void Ask()
	{
		Popup.ShowYesNo(Localization.Get("VIDEO_TO_ORBS") + " " + orbsToGive + " orbs?", ShowAd, null);
	}

	private void ShowAd()
	{
		UnityAdsHelper.ShowAd (null, GiveReward);
	}
	
	private void GiveReward()
	{
		if (reward == Rewards.Orbs)
			GiveOrbs ();
		else if (reward == Rewards.PlayAgain)
			RevivePlayer ();
		
		if(rewardCooldown > 0)
			SetRewardCooldownTime(DateTime.UtcNow.AddSeconds(rewardCooldown));
	}
	
	private void GiveOrbs()
	{
		Global.TotalOrbs += orbsToGive;
		
		Popup.ShowOk (Localization.Get("YOU_RECEIVED") + " " + orbsToGive + " orbs.");
	}
	
	private void RevivePlayer()
	{
		if (OnRevivePlayer != null)
			OnRevivePlayer ();
	}

	void Update()
	{
		if(rewardCooldown > 0)
		{
			foreach(UIButton button in GetComponents<UIButton>())
				button.isEnabled = RewardCooldownLeft <= 0;

			countdown.enabled = RewardCooldownLeft > 0;

			if(RewardCooldownLeft >= 0)
			{
				int seconds = (int)RewardCooldownLeft % 60;
				int minutes = (int)RewardCooldownLeft / 60;

				countdown.text = string.Format("{0:00}:{1:00}", minutes, seconds);
			}
		}
		else
		{
			foreach(UIButton button in GetComponents<UIButton>())
				button.isEnabled = true;

			countdown.enabled = false;
		}
	}

	//--- Reward Cooldown Methods
	
	public static DateTime GetRewardCooldownTime ()
	{
		if (object.Equals(rewardCooldownTime,default(DateTime)))
		{
			if (PlayerPrefs.HasKey(Global.REWARDED_VIDEO_COOLDOWN))
				rewardCooldownTime = DateTime.Parse(PlayerPrefs.GetString(Global.REWARDED_VIDEO_COOLDOWN));
			else 
				rewardCooldownTime = DateTime.UtcNow;
		}
		
		return rewardCooldownTime;
	}
	
	public static void SetRewardCooldownTime (DateTime dateTime)
	{
		rewardCooldownTime = dateTime;
		PlayerPrefs.SetString(Global.REWARDED_VIDEO_COOLDOWN,dateTime.ToString());
		PlayerPrefs.Save ();
	}
	
	public static void ResetRewardCooldownTime ()
	{
		SetRewardCooldownTime(DateTime.UtcNow);
	}
}
