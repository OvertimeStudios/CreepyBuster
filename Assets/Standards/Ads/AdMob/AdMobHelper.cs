using UnityEngine;
using System;
using System.Collections;
#if ADMOB_IMPLEMENTED
using GoogleMobileAds.Api;
#endif

/// <summary>
/// Ad mob helper. It needs a prefab in game with this script attached to it
/// </summary>
public class AdMobHelper : MonoBehaviour 
{
	public DeviceID[] testDevices;

	public float timeout = 5f;

	[Header("Interstitial")]
	public string interstitialAndroid;
	public string interstitialIOS;

	[Header("Rewarded Video")]
	public string rewardedVideoAndroid;
	public string rewardedVideoIOS;

	private static Action _onComplete;

	#if ADMOB_IMPLEMENTED
	private static RewardBasedVideoAd rewardBasedVideo;
	private static InterstitialAd interstitial;
	#endif

	#region singleton
	private static AdMobHelper instance;
	public static AdMobHelper Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<AdMobHelper>();

			return instance;
		}
	}
	#endregion

	void OnDestroy()
	{
		#if ADMOB_IMPLEMENTED

		if(rewardBasedVideo != null)
		{
			rewardBasedVideo.OnAdLoaded -= HandleRewardBasedVideoLoaded;
			rewardBasedVideo.OnAdFailedToLoad -= HandleRewardBasedVideoFailedToLoad;
			rewardBasedVideo.OnAdOpening -= HandleRewardBasedVideoOpened;
			rewardBasedVideo.OnAdStarted -= HandleRewardBasedVideoStarted;
			rewardBasedVideo.OnAdRewarded -= HandleRewardBasedVideoRewarded;
			rewardBasedVideo.OnAdClosed -= HandleRewardBasedVideoClosed;
			rewardBasedVideo.OnAdLeavingApplication -= HandleRewardBasedVideoLeftApplication;
		}

		if(interstitial != null)
		{
			interstitial.OnAdLoaded += HandleOnInterstitialLoaded;
			interstitial.OnAdFailedToLoad += HandleOnInterstitialFailedToLoad;
			interstitial.OnAdOpening += HandleOnInterstitialOpened;
			interstitial.OnAdClosed += HandleOnInterstitialClosed;
			interstitial.OnAdLeavingApplication += HandleOnInterstitialLeavingApplication;
		}
		#endif
	}

	// Use this for initialization
	void Start () 
	{
		#if ADMOB_IMPLEMENTED

		#region REWARDED VIDEO
		//if user filled these fields, we should preload video into memory
		if(!string.IsNullOrEmpty(rewardedVideoAndroid) || !string.IsNullOrEmpty(rewardedVideoIOS))
		{
			rewardBasedVideo = RewardBasedVideoAd.Instance;
			

			CreateRewardedVideoRequest();

			// Ad event fired when the rewarded video ad
			// has been received.
			rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
			// has failed to load.
			rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
			// is opened.
			rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
			// has started playing.
			rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
			// has rewarded the user.
			rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
			// is closed.
			rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
			// is leaving the application.
			rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
		}
		#endregion

		#region INTERSTITIAL
		//if user filled these fields, we should preload video into memory
		if(!string.IsNullOrEmpty(interstitialAndroid) || !string.IsNullOrEmpty(interstitialAndroid))
		{
			CreateInterstitialRequest();

			// Called when an ad request has successfully loaded.
			interstitial.OnAdLoaded += HandleOnInterstitialLoaded;
			// Called when an ad request failed to load.
			interstitial.OnAdFailedToLoad += HandleOnInterstitialFailedToLoad;
			// Called when an ad is clicked.
			interstitial.OnAdOpening += HandleOnInterstitialOpened;
			// Called when the user returned from the app after an ad click.
			interstitial.OnAdClosed += HandleOnInterstitialClosed;
			// Called when the ad click caused the user to leave the application.
			interstitial.OnAdLeavingApplication += HandleOnInterstitialLeavingApplication;
		}

		#endregion

		#endif
	}

	#region INTERSTITIAL
	public static bool IsInterstitialReady
	{
		get 
		{
			#if ADMOB_IMPLEMENTED
			return interstitial != null && interstitial.IsLoaded(); 
			#else
			return false;
			#endif
		}
	}

	public static void ShowInterstitial()
	{
		#if ADMOB_IMPLEMENTED
		if(interstitial.IsLoaded())
			interstitial.Show();
		#endif
	}

	private static void CreateInterstitialRequest()
	{
		#if ADMOB_IMPLEMENTED

		#if UNITY_ANDROID
		string adUnitId = Instance.interstitialAndroid;
		#elif UNITY_IPHONE
		string adUnitId = Instance.interstitialIOS;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);
		// Create an empty ad request.
		AdRequest.Builder request = new AdRequest.Builder();

		//add test devices
		foreach(DeviceID device in Instance.testDevices)
			request.AddTestDevice(device.id);

		// Load the interstitial with the request.
		interstitial.LoadAd(request.Build());

		#endif
	}

	private static void HandleOnInterstitialLoaded(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Loaded Successfully");
		#endif
	}

	#if ADMOB_IMPLEMENTED
	private static void HandleOnInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{

		Debug.Log("Rewarded Video Failed to load: " + args.Message);
		// Handle the ad failed to load event.

		Instance.StartCoroutine(WaitAndCreateInterstitialRequest(Instance.timeout));

	}
	#endif

	private static IEnumerator WaitAndCreateInterstitialRequest(float waitTime)
	{
		#if ADMOB_IMPLEMENTED
		yield return new WaitForSeconds(waitTime);

		CreateInterstitialRequest();
		#else
		yield return null;
		#endif
	}

	private static void HandleOnInterstitialOpened(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Opened");
		#endif
	}

	private static void HandleOnInterstitialClosed(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Closed");
		CreateInterstitialRequest();
		#endif
	}

	private static void HandleOnInterstitialLeavingApplication(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Left Application");
		#endif
	}

	#endregion

	#region REWARDED VIDEO
	public static bool IsRewardedVideoReady
	{
		get 
		{
			#if ADMOB_IMPLEMENTED
			return rewardBasedVideo != null && rewardBasedVideo.IsLoaded(); 
			#else
			return false;
			#endif
		}
	}

	public static void ShowRewardedVideo(Action onComplete)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("rewardBasedVideo.IsLoaded() = " + rewardBasedVideo.IsLoaded());
		if(rewardBasedVideo.IsLoaded())
		{
			_onComplete = onComplete;

			rewardBasedVideo.Show();
		}
		#endif
	}
		
	private static void CreateRewardedVideoRequest()
	{
		#if ADMOB_IMPLEMENTED

		#if UNITY_EDITOR
		string adUnitId = "unused";
		#elif UNITY_ANDROID
		string adUnitId = Instance.rewardedVideoAndroid;
		#elif UNITY_IPHONE
		string adUnitId = Instance.rewardedVideoIOS;
		#else
		string adUnitId = "unexpected_platform";
		#endif

		//note: Rewarded Video Request is third party for AdMob and it's only hanlded by them. No test devices are permited. If you want rewarded video, set as test on the third party
		AdRequest.Builder request = new AdRequest.Builder();
		rewardBasedVideo.LoadAd(request.Build(), adUnitId);
		#endif
	}

	private static void HandleRewardBasedVideoLoaded(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Loaded Successfully");
		#endif
	}
	
	#if ADMOB_IMPLEMENTED
	private static void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
	{

		Debug.Log("Rewarded Video Failed to load: " + args.Message);
		// Handle the ad failed to load event.

		Instance.StartCoroutine(WaitAndCreateRewardedVideoRequest(Instance.timeout));

	}
	#endif

	private static IEnumerator WaitAndCreateRewardedVideoRequest(float waitTime)
	{
		#if ADMOB_IMPLEMENTED
		yield return new WaitForSeconds(waitTime);

		CreateRewardedVideoRequest();
		#else
		yield return null;
		#endif
	}

	private static void HandleRewardBasedVideoOpened(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Opened");
		#endif
	}

	private static void HandleRewardBasedVideoStarted(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Started");
		#endif
	}

	private static void HandleRewardBasedVideoRewarded(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Rewarded");

		if(_onComplete != null)
			_onComplete();
		#endif
	}

	private static void HandleRewardBasedVideoClosed(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Closed");
		CreateRewardedVideoRequest();
		#endif
	}

	private static void HandleRewardBasedVideoLeftApplication(object sender, EventArgs e)
	{
		#if ADMOB_IMPLEMENTED
		Debug.Log("Rewarded Video Left Application");
		#endif
	}
	#endregion
}

[System.Serializable]
public class DeviceID
{
	public string name;
	public string id;
}