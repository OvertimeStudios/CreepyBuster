using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ADMOB_IMPLEMENTED
using GoogleMobileAds.Api;
#endif

public class AdMobHelper : MonoBehaviour 
{
	#if ADMOB_IMPLEMENTED
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

	void Start()
	{
		if(preloadBanner)
			RequestBanner ();
	}

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
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request;
		if(Debug.isDebugBuild && Instance.isTest)
		{
			Debug.Log("Request test");
			string devicesIDs = "";

			foreach(string testDeviceId in testDeviceIDs)
				devicesIDs += testDeviceId + ",";

			devicesIDs = devicesIDs.Substring(0, devicesIDs.Length - 1);

			Debug.Log(devicesIDs);

			request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice(devicesIDs).Build();
		}
		else
			request = new AdRequest.Builder().Build();
		// Load the banner with the request.
		bannerView.LoadAd(request);
	}

	public static void ShowBanner()
	{
		if(bannerShowing) return;

		bannerView.Show ();

		bannerShowing = true;
	}

	public static void HideBanner()
	{
		bannerView.Hide ();

		bannerShowing = false;
	}
	#endif
}
