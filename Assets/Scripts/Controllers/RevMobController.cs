using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class RevMobController : MonoBehaviour
{
	#if REVMOB_IMPLEMENTED
	public float timeToReconnect = 5f;

	#region singleton
	private static RevMobController instance;
	public static RevMobController Instance
	{
		get
		{
			if (instance == null)
				instance = GameObject.FindObjectOfType<RevMobController>();

			return instance;
		}
	}
	#endregion

	private static readonly Dictionary<String, String> REVMOB_APP_IDS = new Dictionary<String, String>() 
	{
		{ "Android", "558073aa1f79ab5b1e1062d7"},
		{ "IOS", "558073401f79ab5b1e1062c9" }
	};
	
	//RevMobFullscreen fullscreen, video, rewardedVideo;
	//RevMobBanner banner;

	private bool videoReceived = false;

	void Start() 
	{
		RevMobSingleton.Init (REVMOB_APP_IDS);
	}

	#region video ads
	public void ShowVideo()
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		if(videoReceived)
		{
			//video = revmob.CreateVideo();
			videoReceived = false;
		}
		#endif
	}
	#endregion

	#region rewarded video
	void ShowRewardedVideo()
	{
		Debug.Log ("Show rewarded video ads!");
		
		#if UNITY_ANDROID || UNITY_IPHONE
		//rewardedVideo.ShowRewardedVideo();
		#endif
	}
	#endregion

	void OnEnable()
	{
		MenuController.OnPanelClosed += RevMobSingleton.ShowFullscreen;
		MenuController.OnPanelOpened += ShowBanner;
		MenuController.OnPanelClosing += RevMobSingleton.HideBanner;
	}
	
	void OnDisable()
	{
		MenuController.OnPanelClosed -= RevMobSingleton.ShowFullscreen;
		MenuController.OnPanelOpened -= ShowBanner;
		MenuController.OnPanelClosing -= RevMobSingleton.HideBanner;
	}

	void ShowBanner()
	{
		RevMobSingleton.ShowBanner (RevMob.Position.BOTTOM);
	}

	#endif
}