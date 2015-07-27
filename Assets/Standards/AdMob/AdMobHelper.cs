using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if ADMOB_IMPLEMENTED
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif

public class AdMobHelper : MonoBehaviour 
{
	#region singleton
	private static AdMobHelper instance;
	private static AdMobHelper Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<AdMobHelper>();

			return instance;
		}
	}
	#endregion

	#region get / set
	public static bool IsBannerShowing
	{
		get { return bannerShowing; }
	}
	#endregion

	public bool isTest;
	public string[] testDeviceIDs;

	[Header("Preloads")]
	public bool preloadBanner = true;

	//banner properties
	private static BannerView bannerView;
	private static bool bannerShowing = false;

	#if ADMOB_IMPLEMENTED
	void Start()
	{
		if(preloadBanner)
			RequestBanner ();
	}

	#region Banner
	private void RequestBanner()
	{
		#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-7220323901199576/7752360046";
		#elif UNITY_IPHONE
		string adUnitId = "INSERT_IOS_BANNER_AD_UNIT_ID_HERE";
		#else
		string adUnitId = "unexpected_platform";
		#endif
		
		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

		bannerView.AdLoaded += HandleAdLoaded;
		bannerView.AdOpened += HandleAdOpened;
		bannerView.AdClosed += HandleAdClosed;
		bannerView.AdClosing += HandleAdClosed;
		bannerView.AdLeftApplication += HandleAdLeftApplication;
		bannerView.AdFailedToLoad += HandleAdFailedToLoad;

		// Load the banner with the request.
		bannerView.LoadAd(createAdRequest());
	}

	public static void ShowBanner()
	{
		if(bannerShowing || bannerView == null) return;

		bannerView.Show ();

		bannerShowing = true;
	}

	public static void HideBanner()
	{
		bannerView.Hide ();

		bannerShowing = false;
	}

	// Returns an ad request with custom ad targeting.
	private AdRequest createAdRequest()
	{
		// Create an empty ad request.
		AdRequest request;
		if(Debug.isDebugBuild && Instance.isTest)
		{
			Debug.Log("Test Banner Request");
			string devicesIDs = "";
			
			foreach(string testDeviceId in testDeviceIDs)
				devicesIDs += testDeviceId + ",";
			
			devicesIDs = devicesIDs.Substring(0, devicesIDs.Length - 1);
			
			request = new AdRequest.Builder()
					.AddTestDevice(AdRequest.TestDeviceSimulator)
					.AddTestDevice(devicesIDs)
					.AddKeyword("game")
					.SetGender(Gender.Male)
					.SetBirthday(new DateTime(1985, 1, 1))
					.TagForChildDirectedTreatment(false)
					.AddExtra("color_bg", "9B30FF")
					.Build();
		}
		else
		{
			Debug.Log("Normal Banner Request");
			request = new AdRequest.Builder().Build();
		}

		return request;
	}

	public void HandleAdLoaded(object sender, EventArgs args)
	{
		print("HandleAdLoaded event received.");
	}
	
	public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{
		print("HandleFailedToReceiveAd event received with message: " + args.Message);
	}
	
	public void HandleAdOpened(object sender, EventArgs args)
	{
		print("HandleAdOpened event received");
	}
	
	void HandleAdClosing(object sender, EventArgs args)
	{
		print("HandleAdClosing event received");
	}
	
	public void HandleAdClosed(object sender, EventArgs args)
	{
		print("HandleAdClosed event received");
	}
	
	public void HandleAdLeftApplication(object sender, EventArgs args)
	{
		print("HandleAdLeftApplication event received");
	}
	#endregion
	
	#endif
}
