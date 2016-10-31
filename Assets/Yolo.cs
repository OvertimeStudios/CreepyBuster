using UnityEngine;
using System.Collections;

public class Yolo : MonoBehaviour 
{
	public void WatchVideo()
	{
		if(AdMobHelper.IsRewardedVideoReady)
			AdMobHelper.ShowRewardedVideo(OnCompleteHandler);
	}

	private void OnCompleteHandler()
	{
		int orbs = PlayerPrefs.GetInt("YOLO", 0);
		PlayerPrefs.SetInt("YOLO", orbs + 100);

		Debug.Log("******** " + PlayerPrefs.GetInt("YOLO"));
	}
}
