using System;
using UnityEngine;
using System.Collections;
#if UNITYADS_IMPLEMENTED
using UnityEngine.Advertisements;
#endif

public class UnityAdsHelper : MonoBehaviour 
{
	public const string SIMPLE_VIDEO = "video";
	public const string REWARDED_VIDEO = "rewardedVideo";

	#if UNITYADS_IMPLEMENTED
	public static bool isSupported { get { return Advertisement.isSupported; } }
	public static bool isInitialized { get { return Advertisement.isInitialized; } }
	
	public static bool IsReady() { return Advertisement.IsReady(); }
	public static bool IsReady(string zoneID) { return Advertisement.IsReady(zoneID); }

	private static Action _onComplete;
	#endif

	#region singleton
	private static UnityAdsHelper instance;
	public static UnityAdsHelper Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<UnityAdsHelper>();

			return instance;
		}
	}
	#endregion

	public static void ShowSimpleAd()
	{
		#if UNITYADS_IMPLEMENTED
		if(Advertisement.IsReady(SIMPLE_VIDEO))
		   Advertisement.Show(SIMPLE_VIDEO);
		#endif
	}
	
	public static void ShowRewardedAd(Action onComplete)
	{
		#if UNITYADS_IMPLEMENTED
		_onComplete = onComplete;
		Instance.ShowRwrdAd();
		#endif
	}

	private void ShowRwrdAd()
	{
		#if UNITYADS_IMPLEMENTED
		if (Advertisement.IsReady(REWARDED_VIDEO))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show(REWARDED_VIDEO, options);
		}
		#endif
	}

	#if UNITYADS_IMPLEMENTED
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
			case ShowResult.Finished:
				Debug.Log("The ad was successfully shown.");

				if(_onComplete != null)
					_onComplete();

				_onComplete = null;

				break;
			case ShowResult.Skipped:
				Debug.Log("The ad was skipped before reaching the end.");
				break;
			case ShowResult.Failed:
				Debug.LogError("The ad failed to be shown.");
				break;
		}
	}
	#endif
}
