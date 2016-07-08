using UnityEngine;
using System;
using System.Collections;

public class AdsHelper : MonoBehaviour 
{
	public enum AdSize
	{
		Banner,
		MediumRectangle,
		IABBanner,
		Leaderboard,
		SmartBanner,
	}

	public enum AdPosition
	{
		Bottom,
		BottomLeft,
		BottomRight,
		Top,
		TopLeft,
		TopRight,
	}

	public static bool IsInterstitialReady
	{
		get
		{
			#if ADMOB_IMPLEMENTED
			return AdMobHelper.IsInterstitialReady;
			#else
			return false;
			#endif
		}
	}

	public static bool IsSimpleVideoReady
	{
		get
		{
			#if UNITYADS_IMPLEMENTED
			//check if any simple video ad is ready
			return UnityAdsHelper.IsReady(UnityAdsHelper.SIMPLE_VIDEO);
			#else
			return false;
			#endif
		}
	}

	public static bool IsRewardedVideoReady
	{
		get
		{
			#if UNITYADS_IMPLEMENTED || ADMOB_IMPLEMENTED
			//check if any rewarded video ad is ready
			return UnityAdsHelper.IsReady(UnityAdsHelper.REWARDED_VIDEO) || AdMobHelper.IsRewardedVideoReady;
			#else
			return false;
			#endif
		}
	}

	public static void ShowBannerAd(AdsHelper.AdSize adSize, AdsHelper.AdPosition adPosition)
	{
		AdMobHelper.ShowBanner(adSize, adPosition);
	}

	public static void HideBannerAd()
	{
		AdMobHelper.HideBanner();
	}

	public static void ShowInstertitialAd()
	{
		#if ADMOB_IMPLEMENTED
		if(AdMobHelper.IsInterstitialReady)
			AdMobHelper.ShowInterstitial();
		else
			Debug.LogError("No Instertitial Ad Ready. Please call IsInterstitialReady first to verify availability.");
		#endif
	}

	private static void ShowSimpleVideoAd()
	{
		#if UNITYADS_IMPLEMENTED
		if(UnityAdsHelper.IsReady(UnityAdsHelper.SIMPLE_VIDEO))
			UnityAdsHelper.ShowSimpleAd();
		else
			Debug.LogError("No Simple Video Ad Ready. Please call IsSimpleVideoReady first to verify availability.");
		#else
		Debug.LogError("No Ads Implemented");
		#endif
	}

	public static void ShowRewardedAd(Action onComplete)
	{
		#if UNITYADS_IMPLEMENTED || ADMOB_IMPLEMENTED
		if(UnityAdsHelper.IsReady(UnityAdsHelper.REWARDED_VIDEO))
			UnityAdsHelper.ShowRewardedAd(onComplete);
		else if(AdMobHelper.IsRewardedVideoReady)
			AdMobHelper.ShowRewardedVideo(onComplete);
		else
			Debug.LogError("No Rewarded Video Ad Ready. Please call IsRewardedVideoReady first to verify availability.");
		#else
		Debug.LogError("No Ads Implemented");
		#endif
	}
}
