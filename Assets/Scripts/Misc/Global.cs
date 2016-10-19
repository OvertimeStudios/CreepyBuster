using UnityEngine;
using System.Collections;
using System;

public class Global : MonoBehaviour 
{
	#region Action
	public static event Action OnOrbUpdated;
	public static event Action OnPurchasesCleared;
	public static event Action OnHighScoreUpdated;
	#endregion

	#region keys
	private const string FIRST_PLAY = "firstPlay";
	private const string RATED = "rated";
	public const string REWARDED_VIDEO_COOLDOWN = "rewardedVideoCooldown";
	private const string TOTAL_ORBS = "totalOrbs";
	private const string HIGH_SCORE = "highScore";
	private const string RAY_LEVEL = "rayLevel";
	private const string DAMAGE_LEVEL = "damageLevel";
	private const string RANGE_LEVEL = "rangeLevel";
	private const string MUSIC_ON = "musicOn";
	private const string SOUNDFX_ON = "soundFX";
	private const string LANGUAGE = "language";
	private const string VIBRATE = "vibrate";
	private const string FIRST_TIME_TUTORIAL = "firstTimeTutorial";
	private const string TUTORIAL_ENABLED = "tutorialEnabled";
	private const string ORBS_MULTIPLIER = "orbsMultiplier";
	private const string CREEP_UNLOCKED = "_unlocked";
	private const string ADS_FREE = "ads_free";
	private const string BASICS_KILLED = "basicsKilled";
	private const string BOOMERANGS_KILLED = "boomerangKilled";
	private const string CHARGERS_KILLED = "chargerKilled";
	private const string FOLLOWERS_KILLED = "followerKilled";
	private const string ZIGZAGS_KILLED = "zigzagKilled";
	private const string LEGIONS_KILLED = "legionKilled";
	private const string BOSS1_KILLED = "boss1Killed";
	private const string BOSS2_KILLED = "boss2Killed";
	private const string BOSS3_KILLED = "boss3Killed";
	private const string MAX_STREAK = "maxStreak";
	private const string GAMES_PLAYED = "gamesPlayed";
	private const string TIME_PLAYED = "timePlayed";
	private const string ENERGY_SPENT = "energySpent";
	private const string UPGRADES = "upgradesBought";
	private const string LEFT_RIGHT = "leftRight";
	private const string ORBS_COLLECTED = "orbsCollected";
	private const string ORBS_SPENT = "orbsSpent";
	private const string ORBS_MISSED = "orbsMissed";
	private const string BOSS_ENCOUNTERS = "bossEncounters";
	private const string LEVELUP_COLLECTED = "levelUpCollected";
	private const string FROZEN_COLLECTED = "frozenCollected";
	private const string INVENCIBILITY_COLLECTED = "invencibilityCollected";
	private const string DEATH_RAY_COLLECTED = "deathRayCollected";
	private const string POWER_UPS_MISSED = "powerUpsMissed";
	private const string HITS_BY_BASIC = "hitsByBasic";
	private const string HITS_BY_BOOMERANG = "hitsByBoomerang";
	private const string HITS_BY_ZIGZAG = "hitsByZigzag";
	private const string HITS_BY_CHARGER = "hitsByCharger";
	private const string HITS_BY_LEGION = "hitsByLegion";
	private const string HITS_BY_FOLLOWER = "hitsByFollower";
	private const string HITS_BY_BOSS1 = "hitsByBoss1";
	private const string HITS_BY_BOSS2 = "hitsByBoss2";
	private const string HITS_BY_BOSS3 = "hitsByBoss3";
	private const string ENEMIES_MISSED = "enemiesMissed";
	private const string LONGEST_MATCH = "longestMatch";
	private const string TIME_ON_LEFT = "timeOnLeft";
	private const string TIME_ON_RIGHT = "timeOnRight";
	private const string TIME_SPECIAL1 = "timeSpecial1";
	private const string TIME_SPECIAL2 = "timeSpecial2";
	private const string TIME_SPECIAL3 = "timeSpecial3";
	private const string TIME_SPECIAL4 = "timeSpecial4";
	private const string TIME_SPECIAL5 = "timeSpecial5";
	private const string TIME_SPECIAL6 = "timeSpecial6";
	private const string FACEBOOKID = "facebookID";
	private const string DEATHRAY_USABLE = "deathRayUsable";
	private const string INVENCIBLE_USABLE = "invencibleUsable";
	private const string LEVELUP_USABLE = "levelUpUsable";
	private const string FROZEN_USABLE = "frozenUsable";
	private const string SHIELD_USABLE = "shieldUsable";
	private const string DAILY_MISSION_TIME = "dailyRewardTime";
	private const string MISSION1_COMPLETED = "mission1Completed";
	private const string MISSION1_ID = "mission1ID";
	private const string MISSION2_COMPLETED = "mission2Completed";
	private const string MISSION2_ID = "mission2ID";
	private const string MISSION3_COMPLETED = "mission3Completed";
	private const string MISSION3_ID = "mission3ID";
	#endregion

	private static bool isLoaded;

	#region game variables
	private static int sessionsScore;
	#endregion

	#region session variables
	private static bool loggedIn = false;
	public static bool sentOnEnterMenu = false;
	public static bool sentOnEnterGame = false;
	#endregion

	#region get/set
	public static FacebookUser user
	{
		get 
		{ 
			#if FACEBOOK_IMPLEMENTED 
			return FacebookController.User; 
			#else
			return null;
			#endif
		}
	}

	public static bool IsLoaded
	{
		get 
		{
			return isLoaded;
		}
	}

	public static bool IsLoggedIn
	{
		get { return loggedIn; }
	}

	public static int HighScore
	{
		get
		{
			return DataCloudPrefs.GetInt(HIGH_SCORE);
		}

		set
		{
			DataCloudPrefs.SetInt(HIGH_SCORE, value);

			if(OnHighScoreUpdated != null)
				OnHighScoreUpdated();
		}
	}

	public static int SessionScore
	{
		get
		{
			return sessionsScore;
		}
		set
		{
			sessionsScore = value;
		}
	}

	/// <summary>
	/// Gets or sets the total orbs. Use this method to increment and Update orbs on game.
	/// </summary>
	/// <value>The total orbs of player.</value>
	public static int TotalOrbs
	{
		get
		{
			return DataCloudPrefs.GetInt(TOTAL_ORBS);
		}
		
		set
		{
			DataCloudPrefs.SetInt(TOTAL_ORBS, value);

			if(OnOrbUpdated != null)
				OnOrbUpdated();
		}
	}

	public static int RayLevel
	{
		get 
		{
			return DataCloudPrefs.GetInt(RAY_LEVEL);
		}
		set
		{
			DataCloudPrefs.SetInt(RAY_LEVEL, value);
		}
	}

	public static int RangeLevel
	{
		get 
		{
			return DataCloudPrefs.GetInt(RANGE_LEVEL);
		}
		set
		{
			DataCloudPrefs.SetInt(RANGE_LEVEL, value);
		}
	}

	public static int DamageLevel
	{
		get 
		{
			return DataCloudPrefs.GetInt(DAMAGE_LEVEL);
		}
		set
		{
			DataCloudPrefs.SetInt (DAMAGE_LEVEL, value);
		}
	}

	public static bool IsMusicOn
	{
		get
		{
			return DataCloudPrefs.GetInt(MUSIC_ON, 1) == 1;
		}

		set
		{
			DataCloudPrefs.SetInt (MUSIC_ON, (value == true) ? 1 : 0);
		}
	}

	public static bool IsSoundOn
	{
		get
		{
			return DataCloudPrefs.GetInt(SOUNDFX_ON, 1) == 1;
		}
		
		set
		{
			DataCloudPrefs.SetInt (SOUNDFX_ON, (value == true) ? 1 : 0);
		}
	}

	public static string Language
	{
		get
		{
			return DataCloudPrefs.HasKey(LANGUAGE) ? DataCloudPrefs.GetString(LANGUAGE) : "English";
		}

		set
		{
			DataCloudPrefs.SetString(LANGUAGE, value);
		}
	}

	public static bool CanVibrate
	{
		get
		{
			return DataCloudPrefs.GetInt(VIBRATE, 1) == 1;
		}
		
		set
		{
			DataCloudPrefs.SetInt (VIBRATE, (value == true) ? 1 : 0);
		}
	}

	public static bool FirstPlay
	{
		get 
		{
			return !DataCloudPrefs.HasKey (FIRST_PLAY); 
		}
	}

	public static bool Rated
	{
		get 
		{ 
			return DataCloudPrefs.HasKey (RATED); 
		}

		set
		{
			DataCloudPrefs.SetInt(RATED, 1);
		}
	}

	public static bool IsFirstTimeTutorial
	{
		get
		{
			return DataCloudPrefs.GetInt(FIRST_TIME_TUTORIAL) == 1;
		}

		set
		{
			DataCloudPrefs.SetInt (FIRST_TIME_TUTORIAL, (value == true) ? 1 : 0);
		}
	}

	public static bool IsTutorialEnabled
	{
		get
		{
			return DataCloudPrefs.GetInt(TUTORIAL_ENABLED) == 1;
		}
		
		set
		{
			Debug.Log("IsTutorialEnabled? " + IsTutorialEnabled);

			DataCloudPrefs.SetInt (TUTORIAL_ENABLED, (value == true) ? 1 : 0);
		}
	}

	public static int OrbsMultiplier
	{
		get
		{
			return Mathf.Max (1, DataCloudPrefs.GetInt(ORBS_MULTIPLIER));
		}
		
		set
		{
			DataCloudPrefs.SetInt (ORBS_MULTIPLIER, value);
		}
	}

	public static bool IsAdFree
	{
		get
		{
			if(!DataCloudPrefs.HasKey(ADS_FREE))
				return false;

			return DataCloudPrefs.GetInt(ADS_FREE) == 1;
		}

		set
		{
			DataCloudPrefs.SetInt(ADS_FREE, (value == true) ? 1 : 0);
		}
	}

	public static bool IsCreepUnlocked(CreepData.CreepType type)
	{
		return DataCloudPrefs.HasKey(type.ToString() + CREEP_UNLOCKED);
	}

	public static void UnlockCreep(CreepData.CreepType type)
	{
		DataCloudPrefs.SetInt(type.ToString() + CREEP_UNLOCKED, 1);
	}

	public static bool IsAchievementUnlocked(Achievement.Type type, int value)
	{
		return DataCloudPrefs.HasKey(type.ToString() + value);
	}
	
	public static void UnlockAchievement(Achievement.Type type, int value)
	{
		DataCloudPrefs.SetInt(type.ToString() + value, 1);
	}

	public static void LockAchievement(Achievement.Type type, int value)
	{
		DataCloudPrefs.DeleteKey(type.ToString() + value);
	}

	public static int BasicsKilled
	{
		get
		{
			return DataCloudPrefs.GetInt(BASICS_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(BASICS_KILLED, value);
		}
	}

	public static int BoomerangsKilled
	{
		get
		{
			return DataCloudPrefs.GetInt(BOOMERANGS_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(BOOMERANGS_KILLED, value);
		}
	}

	public static int ZigZagsKilled
	{
		get
		{
			return DataCloudPrefs.GetInt(ZIGZAGS_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(ZIGZAGS_KILLED, value);
		}
	}

	public static int ChargersKilled
	{
		get
		{
			return DataCloudPrefs.GetInt(CHARGERS_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(CHARGERS_KILLED, value);
		}
	}

	public static int LegionsKilled
	{
		get
		{
			return DataCloudPrefs.GetInt(LEGIONS_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(LEGIONS_KILLED, value);
		}
	}

	public static int FollowersKilled
	{
		get
		{
			return DataCloudPrefs.GetInt(FOLLOWERS_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(FOLLOWERS_KILLED, value);
		}
	}

	public static int Boss1Killed
	{
		get
		{
			return DataCloudPrefs.GetInt(BOSS1_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(BOSS1_KILLED, value);
		}
	}

	public static int Boss2Killed
	{
		get
		{
			return DataCloudPrefs.GetInt(BOSS2_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(BOSS2_KILLED, value);
		}
	}

	public static int Boss3Killed
	{
		get
		{
			return DataCloudPrefs.GetInt(BOSS3_KILLED);
		}
		set
		{
			DataCloudPrefs.SetInt(BOSS3_KILLED, value);
		}
	}

	public static int MaxStreak
	{
		get
		{
			return DataCloudPrefs.GetInt(MAX_STREAK);
		}
		set
		{
			DataCloudPrefs.SetInt(MAX_STREAK, value);
		}
	}
	
	public static int GamesPlayed
	{
		get
		{
			return DataCloudPrefs.GetInt(GAMES_PLAYED);
		}
		set
		{
			DataCloudPrefs.SetInt(GAMES_PLAYED, value);
		}
	}

	public static int TimePlayed
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_PLAYED);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_PLAYED, value);
		}
	}

	public static int EnergySpent
	{
		get
		{
			return DataCloudPrefs.GetInt(ENERGY_SPENT);
		}
		set
		{
			DataCloudPrefs.SetInt(ENERGY_SPENT, value);
		}
	}

	public static int UpgradesBought
	{
		get
		{
			return DataCloudPrefs.GetInt(UPGRADES);
		}
		set
		{
			DataCloudPrefs.SetInt(UPGRADES, value);
		}
	}

	public static int SideLeftRight
	{
		get
		{
			return DataCloudPrefs.GetInt(LEFT_RIGHT);
		}
		set
		{
			DataCloudPrefs.SetInt(LEFT_RIGHT, value);
		}
	}

	public static int OrbsCollected
	{
		get
		{
			return DataCloudPrefs.GetInt(ORBS_COLLECTED);
		}
		set
		{
			DataCloudPrefs.SetInt(ORBS_COLLECTED, value);
		}
	}

	public static int OrbsSpent
	{
		get
		{
			return DataCloudPrefs.GetInt(ORBS_SPENT);
		}
		set
		{
			DataCloudPrefs.SetInt(ORBS_SPENT, value);
		}
	}

	public static int OrbsMissed
	{
		get
		{
			return DataCloudPrefs.GetInt(ORBS_MISSED);
		}
		set
		{
			DataCloudPrefs.SetInt(ORBS_MISSED, value);
		}
	}

	public static int BossEncounters
	{
		get
		{
			return DataCloudPrefs.GetInt(BOSS_ENCOUNTERS);
		}
		set
		{
			DataCloudPrefs.SetInt(BOSS_ENCOUNTERS, value);
		}
	}

	public static int LevelUpCollected
	{
		get
		{
			return DataCloudPrefs.GetInt(LEVELUP_COLLECTED);
		}
		set
		{
			DataCloudPrefs.SetInt(LEVELUP_COLLECTED, value);
		}
	}

	public static int FrozenCollected
	{
		get
		{
			return DataCloudPrefs.GetInt(FROZEN_COLLECTED);
		}
		set
		{
			DataCloudPrefs.SetInt(FROZEN_COLLECTED, value);
		}
	}

	public static int InvencibilityCollected
	{
		get
		{
			return DataCloudPrefs.GetInt(INVENCIBILITY_COLLECTED);
		}
		set
		{
			DataCloudPrefs.SetInt(INVENCIBILITY_COLLECTED, value);
		}
	}

	public static int DeathRayCollected
	{
		get
		{
			return DataCloudPrefs.GetInt(DEATH_RAY_COLLECTED);
		}
		set
		{
			DataCloudPrefs.SetInt(DEATH_RAY_COLLECTED, value);
		}
	}

	public static int PowerUpsMissed
	{
		get
		{
			return DataCloudPrefs.GetInt(POWER_UPS_MISSED);
		}
		set
		{
			DataCloudPrefs.SetInt(POWER_UPS_MISSED, value);
		}
	}

	public static int HitsByBasic
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_BASIC);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_BASIC, value);
		}
	}

	public static int HitsByBoomerang
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_BOOMERANG);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_BOOMERANG, value);
		}
	}

	public static int HitsByZigZag
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_ZIGZAG);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_ZIGZAG, value);
		}
	}

	public static int HitsByCharger
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_CHARGER);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_CHARGER, value);
		}
	}

	public static int HitsByLegion
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_LEGION);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_LEGION, value);
		}
	}

	public static int HitsByFollower
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_FOLLOWER);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_FOLLOWER, value);
		}
	}

	public static int HitsByBoss1
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_BOSS1);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_BOSS1, value);
		}
	}

	public static int HitsByBoss2
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_BOSS2);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_BOSS2, value);
		}
	}

	public static int HitsByBoss3
	{
		get
		{
			return DataCloudPrefs.GetInt(HITS_BY_BOSS3);
		}
		set
		{
			DataCloudPrefs.SetInt(HITS_BY_BOSS3, value);
		}
	}

	public static int EnemiesMissed
	{
		get
		{
			return DataCloudPrefs.GetInt(ENEMIES_MISSED);
		}
		set
		{
			DataCloudPrefs.SetInt(ENEMIES_MISSED, value);
		}
	}

	public static int LongestMatch
	{
		get
		{
			return DataCloudPrefs.GetInt(LONGEST_MATCH);
		}
		set
		{
			DataCloudPrefs.SetInt(LONGEST_MATCH, value);
		}
	}

	public static int TimeOnLeft
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_ON_LEFT);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_ON_LEFT, value);
		}
	}

	public static int TimeOnRight
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_ON_RIGHT);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_ON_RIGHT, value);
		}
	}

	public static int TimeOnSpecial1
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_SPECIAL1);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_SPECIAL1, value);
		}
	}

	public static int TimeOnSpecial2
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_SPECIAL2);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_SPECIAL2, value);
		}
	}

	public static int TimeOnSpecial3
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_SPECIAL3);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_SPECIAL3, value);
		}
	}

	public static int TimeOnSpecial4
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_SPECIAL4);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_SPECIAL4, value);
		}
	}

	public static int TimeOnSpecial5
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_SPECIAL5);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_SPECIAL5, value);
		}
	}

	public static int TimeOnSpecial6
	{
		get
		{
			return DataCloudPrefs.GetInt(TIME_SPECIAL6);
		}
		set
		{
			DataCloudPrefs.SetInt(TIME_SPECIAL6, value);
		}
	}

	public static string FacebookID
	{
		get
		{
			if(!DataCloudPrefs.HasKey(FACEBOOKID))
				return "";

			return DataCloudPrefs.GetString(FACEBOOKID);
		}

		set
		{
			DataCloudPrefs.SetString(FACEBOOKID, value);
		}
	}

	public static int DeathRayConsumable
	{
		get
		{
			if(!DataCloudPrefs.HasKey(DEATHRAY_USABLE))
				return 0;

			return DataCloudPrefs.GetInt(DEATHRAY_USABLE);
		}
		set
		{
			DataCloudPrefs.SetInt(DEATHRAY_USABLE, value);
		}
	}

	public static int InvencibleConsumable
	{
		get
		{
			if(!DataCloudPrefs.HasKey(INVENCIBLE_USABLE))
				return 0;
			
			return DataCloudPrefs.GetInt(INVENCIBLE_USABLE);
		}
		set
		{
			DataCloudPrefs.SetInt(INVENCIBLE_USABLE, value);
		}
	}

	public static int FrozenConsumable
	{
		get
		{
			if(!DataCloudPrefs.HasKey(FROZEN_USABLE))
				return 0;
			
			return DataCloudPrefs.GetInt(FROZEN_USABLE);
		}
		set
		{
			DataCloudPrefs.SetInt(FROZEN_USABLE, value);
		}
	}

	public static int LevelUpConsumable
	{
		get
		{
			if(!DataCloudPrefs.HasKey(LEVELUP_USABLE))
				return 0;
			
			return DataCloudPrefs.GetInt(LEVELUP_USABLE);
		}
		set
		{
			DataCloudPrefs.SetInt(LEVELUP_USABLE, value);
		}
	}

	public static int ShieldConsumable
	{
		get
		{
			if(!DataCloudPrefs.HasKey(SHIELD_USABLE))
				return 0;
			
			return DataCloudPrefs.GetInt(SHIELD_USABLE);
		}
		set
		{
			DataCloudPrefs.SetInt(SHIELD_USABLE, value);
		}
	}

	public static string DailyMissionTime
	{
		get
		{
			if(!DataCloudPrefs.HasKey(DAILY_MISSION_TIME))
				return "";
			
			return DataCloudPrefs.GetString(DAILY_MISSION_TIME);
		}
		set
		{
			DataCloudPrefs.SetString(DAILY_MISSION_TIME, value);
		}
	}

	public static bool Mission1Completed
	{
		get
		{
			if(!DataCloudPrefs.HasKey(MISSION1_COMPLETED))
				return false;
			
			return DataCloudPrefs.GetInt(MISSION1_COMPLETED) == 1;
		}
		set
		{
			DataCloudPrefs.SetInt(MISSION1_COMPLETED, (value == true) ? 1 : 0);
		}
	}

	public static int Mission1ID
	{
		get
		{
			if(!DataCloudPrefs.HasKey(MISSION1_ID))
				return -1;
			
			return DataCloudPrefs.GetInt(MISSION1_ID);
		}
		set
		{
			DataCloudPrefs.SetInt(MISSION1_ID, value);
		}
	}

	public static bool Mission2Completed
	{
		get
		{
			if(!DataCloudPrefs.HasKey(MISSION2_COMPLETED))
				return false;

			return DataCloudPrefs.GetInt(MISSION2_COMPLETED) == 1;
		}
		set
		{
			DataCloudPrefs.SetInt(MISSION2_COMPLETED, (value == true) ? 1 : 0);
		}
	}
	
	public static int Mission2ID
	{
		get
		{
			if(!DataCloudPrefs.HasKey(MISSION2_ID))
				return -1;

			return DataCloudPrefs.GetInt(MISSION2_ID);
		}
		set
		{
			DataCloudPrefs.SetInt(MISSION2_ID, value);
		}
	}

	public static bool Mission3Completed
	{
		get
		{
			if(!DataCloudPrefs.HasKey(MISSION3_COMPLETED))
				return false;

			return DataCloudPrefs.GetInt(MISSION3_COMPLETED) == 1;
		}
		set
		{
			DataCloudPrefs.SetInt(MISSION3_COMPLETED, (value == true) ? 1 : 0);
		}
	}
	
	public static int Mission3ID
	{
		get
		{
			if(!DataCloudPrefs.HasKey(MISSION3_ID))
				return -1;

			return DataCloudPrefs.GetInt(MISSION3_ID);
		}
		set
		{
			DataCloudPrefs.SetInt(MISSION3_ID, value);
		}
	}
	#endregion

	public static void Reset()
	{
		DataCloudPrefs.DeleteAll ();

		Popup.ShowBlank("Player Prefs Reseted", 1f);
	}
}