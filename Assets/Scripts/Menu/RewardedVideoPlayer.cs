using UnityEngine;
using System.Collections;

public class RewardedVideoPlayer : MonoBehaviour 
{
	public enum Rewards
	{
		Orbs,
		PlayAgain,
	}

	public Rewards reward;

	public int orbsToGive;

	public void Play()
	{
		#if UNITYADS_IMPLEMENTED
		UnityAdsHelper.ShowAd (null, GiveReward);
		#endif
	}

	private void GiveReward()
	{
		if (reward == Rewards.Orbs)
			GiveOrbs ();
		else if (reward == Rewards.PlayAgain)
			RevivePlayer ();
	}

	private void GiveOrbs()
	{
		Global.TotalOrbs += orbsToGive;
	}

	private void RevivePlayer()
	{

	}
}
