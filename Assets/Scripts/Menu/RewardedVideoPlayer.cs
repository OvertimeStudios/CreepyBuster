using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RewardedVideoPlayer : MonoBehaviour 
{
	#region Action
	public static event Action OnRevivePlayer;
	public static event Action OnDoubleOrbs;
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
			#if UNITYADS_IMPLEMENTED
			return RewardCooldownLeft <= 0;
			#else
			return false;
			#endif
		}
	}
	#endregion
	
	public enum Rewards
	{
		Orbs,
		PlayAgain,
		DoubleOrbs,
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

		#if UNITY_WEBPLAYER
		gameObject.SetActive(false);
		#endif
	}

	public void Play()
	{
		#if UNITYADS_IMPLEMENTED
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		if(IsReady)
		{
			if(reward == Rewards.PlayAgain)
				ShowAd();
			else
				Ask();
		}
		else if(!UnityAdsHelper.isSupported)
			Popup.ShowOk(Localization.Get("ADS_FAILED"));
		else if(!UnityAdsHelper.isInitialized)
			Popup.ShowOk(Localization.Get("ADS_FAILED"));
		else
			Popup.ShowOk(Localization.Get("ADS_FAILED"));
		#endif
	}

	private void Ask()
	{
		#if UNITYADS_IMPLEMENTED
		if(UnityAdsHelper.IsReady(UnityAdsHelper.REWARDED_VIDEO))
			Popup.ShowYesNo(string.Format(Localization.Get("VIDEO_TO_ORBS"), orbsToGive), ShowAd, null);
		else
			Popup.ShowBlank(Localization.Get("ADS_FAILED"), 2f);
		#else
		Popup.ShowBlank("Unity Ads not implemented", 2f);
		#endif
	}

	public void ShowAd()
	{
		#if UNITYADS_IMPLEMENTED
		UnityAdsHelper.ShowRewardedAd (GiveReward);
		#else
		Popup.ShowBlank("Unity Ads not implemented", 2f);
		#endif
	}

	public static void ShowAd(Action handleFinish)
	{
		#if UNITYADS_IMPLEMENTED
		if(UnityAdsHelper.IsReady(UnityAdsHelper.REWARDED_VIDEO))
			UnityAdsHelper.ShowRewardedAd(handleFinish);
		else
			Popup.ShowBlank(Localization.Get("ADS_FAILED"), 2f);
		#else
		Popup.ShowBlank("Unity Ads not implemented", 2f);
		#endif
	}
	
	private void GiveReward()
	{
		if (reward == Rewards.Orbs)
			GiveOrbs ();
		else if (reward == Rewards.PlayAgain)
			RevivePlayer ();
		else if(reward == Rewards.DoubleOrbs)
			DoubleOrbs();
		
		if(rewardCooldown > 0)
			SetRewardCooldownTime(DateTime.UtcNow.AddSeconds(rewardCooldown));
	}
	
	private void GiveOrbs()
	{
		Global.TotalOrbs += orbsToGive;
		
		Popup.ShowOk (string.Format(Localization.Get("YOU_RECEIVED"), orbsToGive));
	}
	
	private void RevivePlayer()
	{
		if (OnRevivePlayer != null)
			OnRevivePlayer ();
	}

	private void DoubleOrbs()
	{
		if(OnDoubleOrbs != null)
			OnDoubleOrbs();
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
