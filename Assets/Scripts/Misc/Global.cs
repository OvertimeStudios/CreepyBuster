using UnityEngine;
using System.Collections;
using System;

public class Global : MonoBehaviour 
{
	#region Action
	public static event Action OnOrbUpdated;
	public static event Action OnPurchasesCleared;
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
	private const string DAILY_REWARD_DAY = "dailyRewardDay";
	private const string DAILY_REWARD_NEXT_TIME = "dailyRewardNextTime";
	#endregion

	private static bool isLoaded;

	#region game variables
	private static int highScore;
	private static int sessionsScore;
	private static int totalOrbs;
	private static int rayLevel;
	private static int damageLevel;
	private static int rangeLevel;
	private static int musicOn;
	private static int soundOn;
	private static int vibrate;
	private static int orbsMultiplier;

	private static int firstTimeTutorial;
	private static int tutorialEnabled;

	private static string language;
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
			if(!isLoaded)
				Load ();

			return PlayerPrefs.GetInt(HIGH_SCORE);
		}

		set
		{
			highScore = value;

			PlayerPrefs.SetInt (HIGH_SCORE, highScore);
			PlayerPrefs.Save ();
		}
	}

	public static int SessionScore
	{
		get
		{
			if(!isLoaded)
				Load ();

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
			if(!isLoaded)
				Load ();
			
			return totalOrbs;
		}
		
		set
		{
			totalOrbs = value;
			
			PlayerPrefs.SetInt (TOTAL_ORBS, totalOrbs);
			PlayerPrefs.Save ();

			if(OnOrbUpdated != null)
				OnOrbUpdated();
		}
	}

	public static int RayLevel
	{
		get 
		{
			if(!isLoaded)
				Load ();

			return rayLevel;
		}
		set
		{
			rayLevel = value;

			PlayerPrefs.SetInt (RAY_LEVEL, rayLevel);
			PlayerPrefs.Save ();
		}
	}

	public static int RangeLevel
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return rangeLevel;
		}
		set
		{
			rangeLevel = value;
			
			PlayerPrefs.SetInt (RANGE_LEVEL, rangeLevel);
			PlayerPrefs.Save ();
		}
	}

	public static int DamageLevel
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return damageLevel;
		}
		set
		{
			damageLevel = value;
			
			PlayerPrefs.SetInt (DAMAGE_LEVEL, damageLevel);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsMusicOn
	{
		get
		{
			if(!isLoaded)
				Load ();

			return musicOn == 1;
		}

		set
		{
			musicOn = (value == true) ? 1 : 0;

			PlayerPrefs.SetInt (MUSIC_ON, musicOn);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsSoundOn
	{
		get
		{
			if(!isLoaded)
				Load ();

			return soundOn == 1;
		}
		
		set
		{
			soundOn = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (SOUNDFX_ON, soundOn);
			PlayerPrefs.Save ();
		}
	}

	public static string Language
	{
		get
		{
			if(!isLoaded)
				Load();

			return language;
		}

		set
		{
			language = value;

			PlayerPrefs.SetString(LANGUAGE, language);
			PlayerPrefs.Save();
		}
	}

	public static bool CanVibrate
	{
		get
		{
			if(!isLoaded)
				Load ();
			
			return vibrate == 1;
		}
		
		set
		{
			vibrate = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (VIBRATE, vibrate);
			PlayerPrefs.Save ();
		}
	}

	public static bool FirstPlay
	{
		get { return !PlayerPrefs.HasKey (FIRST_PLAY); }
	}

	public static bool Rated
	{
		get { return PlayerPrefs.HasKey (RATED); }

		set
		{
			PlayerPrefs.SetInt(RATED, 1);
			PlayerPrefs.Save();
		}
	}

	public static bool IsFirstTimeTutorial
	{
		get
		{
			if(!isLoaded)
				Load ();

			return firstTimeTutorial == 1;
		}

		set
		{
			firstTimeTutorial = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (FIRST_TIME_TUTORIAL, firstTimeTutorial);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsTutorialEnabled
	{
		get
		{
			if(!isLoaded)
				Load ();
			
			return tutorialEnabled == 1;
		}
		
		set
		{
			tutorialEnabled = (value == true) ? 1 : 0;

			Debug.Log("IsTutorialEnabled? " + IsTutorialEnabled);

			PlayerPrefs.SetInt (TUTORIAL_ENABLED, tutorialEnabled);
			PlayerPrefs.Save ();
		}
	}

	public static int OrbsMultiplier
	{
		get
		{
			if(!isLoaded)
				Load ();
			
			return Mathf.Max (1, orbsMultiplier);
		}
		
		set
		{
			orbsMultiplier = value;
			
			PlayerPrefs.SetInt (ORBS_MULTIPLIER, orbsMultiplier);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsAdFree
	{
		get
		{
			if(!isLoaded)
				Load ();

			if(!PlayerPrefs.HasKey(ADS_FREE))
				return false;

			return PlayerPrefs.GetInt(ADS_FREE) == 1;
		}

		set
		{
			PlayerPrefs.SetInt(ADS_FREE, (value == true) ? 1 : 0);
			PlayerPrefs.Save();
		}
	}

	public static bool IsCreepUnlocked(CreepData.CreepType type)
	{
		if(!isLoaded)
			Load ();

		if(!PlayerPrefs.HasKey(type.ToString() + CREEP_UNLOCKED))
			return false;

		return true;
	}

	public static void UnlockCreep(CreepData.CreepType type)
	{
		PlayerPrefs.SetInt(type.ToString() + CREEP_UNLOCKED, 1);
		PlayerPrefs.Save();
	}

	public static bool IsAchievementUnlocked(Achievement.Type type, int value)
	{
		if(!isLoaded)
			Load ();
		
		if(!PlayerPrefs.HasKey(type.ToString() + value))
			return false;
		
		return true;
	}
	
	public static void UnlockAchievement(Achievement.Type type, int value)
	{
		PlayerPrefs.SetInt(type.ToString() + value, 1);
		PlayerPrefs.Save();
	}

	public static void LockAchievement(Achievement.Type type, int value)
	{
		PlayerPrefs.DeleteKey(type.ToString() + value);
		PlayerPrefs.Save();
	}

	public static int BasicsKilled
	{
		get
		{
			return PlayerPrefs.GetInt(BASICS_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(BASICS_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int BoomerangsKilled
	{
		get
		{
			return PlayerPrefs.GetInt(BOOMERANGS_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(BOOMERANGS_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int ZigZagsKilled
	{
		get
		{
			return PlayerPrefs.GetInt(ZIGZAGS_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(ZIGZAGS_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int ChargersKilled
	{
		get
		{
			return PlayerPrefs.GetInt(CHARGERS_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(CHARGERS_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int LegionsKilled
	{
		get
		{
			return PlayerPrefs.GetInt(LEGIONS_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(LEGIONS_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int FollowersKilled
	{
		get
		{
			return PlayerPrefs.GetInt(FOLLOWERS_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(FOLLOWERS_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int Boss1Killed
	{
		get
		{
			return PlayerPrefs.GetInt(BOSS1_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(BOSS1_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int Boss2Killed
	{
		get
		{
			return PlayerPrefs.GetInt(BOSS2_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(BOSS2_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int Boss3Killed
	{
		get
		{
			return PlayerPrefs.GetInt(BOSS3_KILLED);
		}
		set
		{
			PlayerPrefs.SetInt(BOSS3_KILLED, value);
			PlayerPrefs.Save();
		}
	}

	public static int MaxStreak
	{
		get
		{
			return PlayerPrefs.GetInt(MAX_STREAK);
		}
		set
		{
			PlayerPrefs.SetInt(MAX_STREAK, value);
			PlayerPrefs.Save();
		}
	}
	
	public static int GamesPlayed
	{
		get
		{
			return PlayerPrefs.GetInt(GAMES_PLAYED);
		}
		set
		{
			PlayerPrefs.SetInt(GAMES_PLAYED, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimePlayed
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_PLAYED);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_PLAYED, value);
			PlayerPrefs.Save();
		}
	}

	public static int EnergySpent
	{
		get
		{
			return PlayerPrefs.GetInt(ENERGY_SPENT);
		}
		set
		{
			PlayerPrefs.SetInt(ENERGY_SPENT, value);
			PlayerPrefs.Save();
		}
	}

	public static int UpgradesBought
	{
		get
		{
			return PlayerPrefs.GetInt(UPGRADES);
		}
		set
		{
			PlayerPrefs.SetInt(UPGRADES, value);
			PlayerPrefs.Save();
		}
	}

	public static int SideLeftRight
	{
		get
		{
			return PlayerPrefs.GetInt(LEFT_RIGHT);
		}
		set
		{
			PlayerPrefs.SetInt(LEFT_RIGHT, value);
			PlayerPrefs.Save();
		}
	}

	public static int OrbsCollected
	{
		get
		{
			return PlayerPrefs.GetInt(ORBS_COLLECTED);
		}
		set
		{
			PlayerPrefs.SetInt(ORBS_COLLECTED, value);
			PlayerPrefs.Save();
		}
	}

	public static int OrbsSpent
	{
		get
		{
			return PlayerPrefs.GetInt(ORBS_SPENT);
		}
		set
		{
			PlayerPrefs.SetInt(ORBS_SPENT, value);
			PlayerPrefs.Save();
		}
	}

	public static int OrbsMissed
	{
		get
		{
			return PlayerPrefs.GetInt(ORBS_MISSED);
		}
		set
		{
			PlayerPrefs.SetInt(ORBS_MISSED, value);
			PlayerPrefs.Save();
		}
	}

	public static int BossEncounters
	{
		get
		{
			return PlayerPrefs.GetInt(BOSS_ENCOUNTERS);
		}
		set
		{
			PlayerPrefs.SetInt(BOSS_ENCOUNTERS, value);
			PlayerPrefs.Save();
		}
	}

	public static int LevelUpCollected
	{
		get
		{
			return PlayerPrefs.GetInt(LEVELUP_COLLECTED);
		}
		set
		{
			PlayerPrefs.SetInt(LEVELUP_COLLECTED, value);
			PlayerPrefs.Save();
		}
	}

	public static int FrozenCollected
	{
		get
		{
			return PlayerPrefs.GetInt(FROZEN_COLLECTED);
		}
		set
		{
			PlayerPrefs.SetInt(FROZEN_COLLECTED, value);
			PlayerPrefs.Save();
		}
	}

	public static int InvencibilityCollected
	{
		get
		{
			return PlayerPrefs.GetInt(INVENCIBILITY_COLLECTED);
		}
		set
		{
			PlayerPrefs.SetInt(INVENCIBILITY_COLLECTED, value);
			PlayerPrefs.Save();
		}
	}

	public static int DeathRayCollected
	{
		get
		{
			return PlayerPrefs.GetInt(DEATH_RAY_COLLECTED);
		}
		set
		{
			PlayerPrefs.SetInt(DEATH_RAY_COLLECTED, value);
			PlayerPrefs.Save();
		}
	}

	public static int PowerUpsMissed
	{
		get
		{
			return PlayerPrefs.GetInt(POWER_UPS_MISSED);
		}
		set
		{
			PlayerPrefs.SetInt(POWER_UPS_MISSED, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByBasic
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_BASIC);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_BASIC, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByBoomerang
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_BOOMERANG);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_BOOMERANG, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByZigZag
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_ZIGZAG);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_ZIGZAG, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByCharger
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_CHARGER);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_CHARGER, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByLegion
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_LEGION);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_LEGION, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByFollower
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_FOLLOWER);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_FOLLOWER, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByBoss1
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_BOSS1);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_BOSS1, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByBoss2
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_BOSS2);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_BOSS2, value);
			PlayerPrefs.Save();
		}
	}

	public static int HitsByBoss3
	{
		get
		{
			return PlayerPrefs.GetInt(HITS_BY_BOSS3);
		}
		set
		{
			PlayerPrefs.SetInt(HITS_BY_BOSS3, value);
			PlayerPrefs.Save();
		}
	}

	public static int EnemiesMissed
	{
		get
		{
			return PlayerPrefs.GetInt(ENEMIES_MISSED);
		}
		set
		{
			PlayerPrefs.SetInt(ENEMIES_MISSED, value);
			PlayerPrefs.Save();
		}
	}

	public static int LongestMatch
	{
		get
		{
			return PlayerPrefs.GetInt(LONGEST_MATCH);
		}
		set
		{
			PlayerPrefs.SetInt(LONGEST_MATCH, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnLeft
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_ON_LEFT);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_ON_LEFT, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnRight
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_ON_RIGHT);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_ON_RIGHT, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnSpecial1
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_SPECIAL1);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_SPECIAL1, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnSpecial2
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_SPECIAL2);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_SPECIAL2, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnSpecial3
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_SPECIAL3);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_SPECIAL3, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnSpecial4
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_SPECIAL4);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_SPECIAL4, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnSpecial5
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_SPECIAL5);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_SPECIAL5, value);
			PlayerPrefs.Save();
		}
	}

	public static int TimeOnSpecial6
	{
		get
		{
			return PlayerPrefs.GetInt(TIME_SPECIAL6);
		}
		set
		{
			PlayerPrefs.SetInt(TIME_SPECIAL6, value);
			PlayerPrefs.Save();
		}
	}

	public static string FacebookID
	{
		get
		{
			if(!PlayerPrefs.HasKey(FACEBOOKID))
				return "";

			return PlayerPrefs.GetString(FACEBOOKID);
		}

		set
		{
			PlayerPrefs.SetString(FACEBOOKID, value);
			PlayerPrefs.Save();
		}
	}

	public static int DailyRewardDay
	{
		get
		{
			if(!PlayerPrefs.HasKey(DAILY_REWARD_DAY))
				return 0;

			return PlayerPrefs.GetInt(DAILY_REWARD_DAY);
		}
		set
		{
			PlayerPrefs.SetInt(DAILY_REWARD_DAY, value);
			PlayerPrefs.Save();
		}
	}

	public static string DailyRewardNextTime
	{
		get
		{
			if(!PlayerPrefs.HasKey(DAILY_REWARD_NEXT_TIME))
				return "";
			
			return PlayerPrefs.GetString(DAILY_REWARD_NEXT_TIME);
		}
		set
		{
			PlayerPrefs.SetString(DAILY_REWARD_NEXT_TIME, value);
			PlayerPrefs.Save();
		}
	}
	#endregion

	private static void Load()
	{
		if(isLoaded) return;

		sessionsScore = 0;

		if(FirstPlay)
		{
			PlayerPrefs.SetInt(FIRST_PLAY, 1);
			
			//initialize
			highScore = 0;
			totalOrbs = 0;
			rayLevel = 0;
			rangeLevel = 0;
			damageLevel = 0;
			musicOn = 1;
			soundOn = 1;
			vibrate = 1;
			firstTimeTutorial = 1;
			tutorialEnabled = 1;
			orbsMultiplier = 1;
			
			if(Application.systemLanguage == SystemLanguage.Portuguese)
				language = LocalizationController.Language.Portuguese.ToString();
			else
				language = LocalizationController.Language.English.ToString();
			
			SaveAll();
		}
		else
		{
			highScore = PlayerPrefs.GetInt(HIGH_SCORE);
			totalOrbs = PlayerPrefs.GetInt(TOTAL_ORBS);
			rayLevel = PlayerPrefs.GetInt(RAY_LEVEL);
			rangeLevel = PlayerPrefs.GetInt(RANGE_LEVEL);
			damageLevel = PlayerPrefs.GetInt(DAMAGE_LEVEL);
			musicOn = PlayerPrefs.GetInt(MUSIC_ON);
			soundOn = PlayerPrefs.GetInt(SOUNDFX_ON);
			vibrate = PlayerPrefs.GetInt(VIBRATE);
			firstTimeTutorial = PlayerPrefs.GetInt(FIRST_TIME_TUTORIAL);
			tutorialEnabled = PlayerPrefs.GetInt(TUTORIAL_ENABLED);
			orbsMultiplier = PlayerPrefs.GetInt(ORBS_MULTIPLIER);

			language = PlayerPrefs.GetString(LANGUAGE);
		}

		isLoaded = true;
	}

	public static void ClearPurchasedOnly()
	{
		damageLevel = 0;
		rangeLevel = 0;
		rayLevel = 0;

		SaveAll ();

		if(OnPurchasesCleared != null)
			OnPurchasesCleared();

		Popup.ShowBlank("Purchases Reseted", 1f);
	}

	private static void SaveAll()
	{
		PlayerPrefs.SetInt (HIGH_SCORE, highScore);
		PlayerPrefs.SetInt (TOTAL_ORBS, totalOrbs);
		PlayerPrefs.SetInt (RAY_LEVEL, rayLevel);
		PlayerPrefs.SetInt (RANGE_LEVEL, rangeLevel);
		PlayerPrefs.SetInt (DAMAGE_LEVEL, damageLevel);
		PlayerPrefs.SetInt (MUSIC_ON, musicOn);
		PlayerPrefs.SetInt (SOUNDFX_ON, soundOn);
		PlayerPrefs.SetString (LANGUAGE, language);
		PlayerPrefs.SetInt (VIBRATE, vibrate);
		PlayerPrefs.SetInt(FIRST_TIME_TUTORIAL, firstTimeTutorial);
		PlayerPrefs.SetInt(TUTORIAL_ENABLED, tutorialEnabled);
		PlayerPrefs.SetInt(ORBS_MULTIPLIER, orbsMultiplier);

		PlayerPrefs.Save ();
	}

	public static void Reset()
	{
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();

		Popup.ShowBlank("Player Prefs Reseted", 1f);
	}
}