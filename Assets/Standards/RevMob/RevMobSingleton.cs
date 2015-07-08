using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RevMobAdType
{
	public const string Fullscreen = "Fullscreen";
}

public class RevMobSingleton : MonoBehaviour, IRevMobListener
{
	private static RevMob revmob;

	//RevMob Init parameters
	private static Dictionary<string, string> appIds;
	private static string gameObjectName;

	public static float timeToReconnect = 3.0f;

	//RevMob ads objects
	private static RevMobFullscreen fullscreen, video, rewardedVideo;
	private static RevMobBanner banner;

	//flags of ads ready
	//private static bool videoReceived = false;
	private static bool fullscreenReceived = false;
	private static bool bannerShowed = false;


	#region singleton
	private static RevMobSingleton instance;
	public static RevMobSingleton Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<RevMobSingleton>();

			return instance;
		}
	}
	#endregion

	#region get/set
	public static bool IsFullscreenReady
	{
		get { return fullscreenReceived; }
	}

	public static bool IsBannerShowing
	{
		get { return bannerShowed; }
	}
	#endregion

	void Awake()
	{
		//ensure will only have 1 singleton for the whole game
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			instance = this;
		}
		
		DontDestroyOnLoad (gameObject);
	}

	public static void Init(Dictionary<string, string> appIds)
	{
		Init (appIds, "RevMobSingleton");
	}

	private static void Init(Dictionary<string, string> appIds, string gameObjectName)
	{
		RevMobSingleton.appIds = appIds;
		RevMobSingleton.gameObjectName = gameObjectName;

		TryToConnect ();
	}

	private static IEnumerator TryToConnect(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);
		
		TryToConnect ();
	}
	
	private static void TryToConnect()
	{
		revmob = RevMob.Start (appIds, gameObjectName);
	}

	#region fulscreen ads
	/// <summary>
	/// Fullscreen ad unit can show either an interstitial or a video ad. It will always return the best campaign for your app. 
	/// Most commom ads usage
	/// *If you want to display only interstitial ads in your app, uncheck “Accepts Video” in the “ad units-actions”.
	/// </summary>
	public static void ShowFullscreen()
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		if(fullscreenReceived)
		{
			fullscreen.Show();
			fullscreenReceived = false;
		}
		else
			Debug.LogWarning("Fullscreen ads not loaded yet!");
		#endif
	}
	#endregion

	#region banner
	public static void ShowBanner(RevMob.Position position)
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		if(!bannerShowed)
		{
			banner = revmob.CreateBanner (position);
			banner.Show ();
			bannerShowed = true;
		}
		#endif
	}

	public static void HideBanner()
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		if(bannerShowed)
		{
			banner.Hide();
			banner.Release();
			banner = null;
			bannerShowed = false;
		}
		#endif
	}

	#endregion

	#region IRevMobListener implementation
	public void SessionIsStarted () 
	{
		Debug.Log("Session started.");

		//once session is started successfuly, we start to create all kind of desired ads

		//banner = revmob.CreateBanner();
		//video = revmob.CreateVideo ();
		fullscreen = revmob.CreateFullscreen ();
		//rewardedVideo = revmob.CreateRewardedVideo ();
	}
	
	public void SessionNotStarted (string revMobAdType) 
	{
		Debug.Log("Session not started.");
		
		//if not connected, try to reconnect
		StartCoroutine(TryToConnect (timeToReconnect));
	}
	
	public void RewardedVideoLoaded () 
	{
		Debug.Log("RewardedVideoLoaded.");
	}
	
	public void RewardedVideoNotCompletelyLoaded () 
	{
		Debug.Log("RewardedVideoNotCompletelyLoaded.");
	}
	
	public void RewardedVideoStarted () 
	{
		Debug.Log("RewardedVideoStarted.");
	}
	
	public void RewardedVideoFinished () 
	{
		Debug.Log("RewardedVideoFinished.");
	}
	
	public void RewardedVideoCompleted () 
	{
		Debug.Log("RewardedVideoCompleted.");
	}
	
	public void RewardedPreRollDisplayed () 
	{
		Debug.Log("RewardedPreRollDisplayed.");
	}
	
	public void VideoLoaded () 
	{
		Debug.Log("VideoLoaded.");
	}
	
	public void VideoNotCompletelyLoaded () 
	{
		Debug.Log("VideoNotCompletelyLoaded.");
	}
	
	public void VideoReceived()
	{
		Debug.Log("VideoReceived.");
		
		//videoReceived = true;
	}
	
	public void VideoStarted () 
	{
		Debug.Log("VideoStarted.");
	}
	
	public void VideoFinished () 
	{
		Debug.Log("VideoFinished.");
	}

	public void AdDidReceive (string revMobAdType) 
	{
		Debug.Log("Ad did receive: " + revMobAdType);

		if (revMobAdType == RevMobAdType.Fullscreen) 
			fullscreenReceived = true;
	}
	
	public void AdDidFail (string revMobAdType) 
	{
		Debug.Log("Ad did fail: " + revMobAdType);
	}
	
	public void AdDisplayed (string revMobAdType) 
	{
		Debug.Log("Ad displayed.");
	}
	
	public void UserClickedInTheAd (string revMobAdType) 
	{
		Debug.Log("Ad clicked: " + revMobAdType);

		if(revMobAdType == RevMobAdType.Fullscreen)
		{
			fullscreen.Release();
			fullscreen = revmob.CreateFullscreen();
		}
	}
	
	public void UserClosedTheAd (string revMobAdType) 
	{
		Debug.Log("Ad closed: " + revMobAdType);

		if (revMobAdType == RevMobAdType.Fullscreen) 
		{
			fullscreen.Release();
			fullscreen = revmob.CreateFullscreen();
		}
	}
	
	public void InstallDidReceive(string message) 
	{
		Debug.Log("Install received");
	}
	
	public void InstallDidFail(string message) 
	{
		Debug.Log("Install not received");
	}
	
	public void EulaIsShown() 
	{
		Debug.Log("Eula is displayed");
	}
	
	public void EulaAccepted() 
	{
		Debug.Log("Eula was accepted");
	}
	
	public void EulaRejected() 
	{
		Debug.Log("Eula was rejected");
	}
	#endregion
}
