using UnityEngine;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;

public class UnityAnalyticsHelper : MonoBehaviour 
{
	public enum CustomEvent
	{
		GameOver,
		Kills,
		KilledBy,
		WatchedRewardedVideoOrbs,
		EnterMenu,
		EnterGame,
		Level,
	}

	public enum NextScreen
	{
		Menu,
		Shop,
	}

	public static void GameOver(int score, int orbsCollected, float time, int continuesViaVideo, int continuesViaOrb, NextScreen nextScreen, bool watchedReplay, bool watchedVideoDoubleOrbs)
	{
		Dictionary<string, object> eventData = new Dictionary<string, object>();
		eventData.Add("score", score);
		eventData.Add("orbs", orbsCollected);
		eventData.Add("time", time);
		eventData.Add("continuesViaVideo", continuesViaVideo);
		eventData.Add("continuesViaOrbs", continuesViaOrb);
		eventData.Add("nextScreen", nextScreen.ToString());
		eventData.Add("watchedReplay", watchedReplay);
		eventData.Add("watchedVideoDoubleOrbs", watchedVideoDoubleOrbs);
		eventData.Add("gamesPlayed", 1);

		SendData(CustomEvent.GameOver, eventData);
	}

	public static void KilledBy(int basic, int boomerang, int zigzag, int charger, int legion, int follower, int boss1, int boss2, int boss3)
	{
		Dictionary<string, object> eventData = new Dictionary<string, object>();
		eventData.Add("killedByBasic", basic);
		eventData.Add("killedByBoomerang", boomerang);
		eventData.Add("killedByZigZag", zigzag);
		eventData.Add("killedByCharger", charger);
		eventData.Add("killedByLegion", legion);
		eventData.Add("killedByFollower", follower);
		eventData.Add("killedByBoss1", boss1);
		eventData.Add("killedByBoss2", boss2);
		eventData.Add("killedByBoss3", boss3);

		SendData(CustomEvent.KilledBy, eventData);
	}

	public static void Kills(int basic, int boomerang, int zigzag, int charger, int legion, int follower, int boss1, int boss2, int boss3)
	{
		Dictionary<string, object> eventData = new Dictionary<string, object>();
		eventData.Add("killBasic", basic);
		eventData.Add("killBoomerang", boomerang);
		eventData.Add("killZigZag", zigzag);
		eventData.Add("killCharger", charger);
		eventData.Add("killLegion", legion);
		eventData.Add("killFollower", follower);
		eventData.Add("killBoss1", boss1);
		eventData.Add("killBoss2", boss2);
		eventData.Add("killBoss3", boss3);
		
		SendData(CustomEvent.Kills, eventData);
	}

	public static void EnterOnMenu()
	{
		SendData(CustomEvent.EnterMenu, new Dictionary<string, object>());
	}

	public static void EnterOnGame()
	{
		SendData(CustomEvent.EnterGame, new Dictionary<string, object>());
	}

	public static void OnLevelUp(int level)
	{
		SendData(CustomEvent.Level.ToString + level, new Dictionary<string, object>());
	}

	private static void SendData(CustomEvent e, Dictionary<string, object> eventData)
	{
		SendData(e.ToString(), eventData);
	}

	private static void SendData(string e, Dictionary<string, object> eventData)
	{
		Analytics.CustomEvent(e, eventData);
	}
}
