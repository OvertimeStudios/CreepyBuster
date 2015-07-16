using UnityEngine;
using System.Collections;

public class RewardedVideoPlayer : MonoBehaviour 
{
	public int orbsToGive;

	public void Play()
	{
		#if UNITYADS_IMPLEMENTED
		UnityAdsHelper.ShowAd (null, GiveOrbs);
		#endif
	}

	private void GiveOrbs()
	{
		Global.TotalOrbs += orbsToGive;
	}
}
