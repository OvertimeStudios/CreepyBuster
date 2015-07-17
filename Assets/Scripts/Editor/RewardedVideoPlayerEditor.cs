using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RewardedVideoPlayer))]
public class RewardedVideoPlayerEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		RewardedVideoPlayer myScript = target as RewardedVideoPlayer;

		myScript.reward = (RewardedVideoPlayer.Rewards) EditorGUILayout.EnumPopup ("Reward", myScript.reward);

		if (myScript.reward == RewardedVideoPlayer.Rewards.Orbs)
			myScript.orbsToGive = EditorGUILayout.IntField ("Orbs to give", myScript.orbsToGive);
	}

}
