using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DailyMissionController : MonoBehaviour 
{
	private int rewardCooldown;
	private static DateTime rewardCooldownTime;

	[Header("For Developer")]
	public UILabel countdown;
	public Color countdownNormalColor;
	public Transform missions;

	public static List<DailyMission> missionRecentUnlocked  = new List<DailyMission>();

	private DailyMissionObject[] missionObjects;
	private DailyMissionObject[] MissionObjects
	{
		get 
		{ 
			if(missionObjects == null)
				missionObjects = missions.GetComponentsInChildren<DailyMissionObject>();

			return missionObjects;
		}
	}

	#region get / set
	public DateTime RewardCooldownTime
	{
		get 
		{
			if (object.Equals(rewardCooldownTime,default(DateTime)))
			{
				if(!string.IsNullOrEmpty(Global.DailyMissionTime))
					rewardCooldownTime = DateTime.Parse(Global.DailyMissionTime);
				else 
					rewardCooldownTime = DateTime.UtcNow;
			}
			
			return rewardCooldownTime;
		}
	}
	
	public int RewardCooldownLeft
	{
		get
		{
			return (int)RewardCooldownTime.Subtract(DateTime.UtcNow).TotalSeconds;
		}
	}
	
	public bool IsReady
	{
		get { return RewardCooldownLeft <= 0; }
	}
	#endregion

	#region cooldown function
	public static void SetRewardCooldownTime (DateTime dateTime)
	{
		rewardCooldownTime = dateTime;
		Global.DailyMissionTime = dateTime.ToString();
	}
	#endregion

	#region daily stats
		#region const
		private const string DAILY_HIGHSCORE = "daily_highscore";
		private const string DAILY_BASIC = "daily_basic";
		private const string DAILY_BOOMERANG = "daily_boomerang";
		private const string DAILY_ZIGZAG = "daily_zigzag";
		private const string DAILY_CHARGER = "daily_charger";
		private const string DAILY_LEGION = "daily_legion";
		private const string DAILY_FOLLOWER = "daily_follower";
		private const string DAILY_BOSS1 = "daily_boss1";
		private const string DAILY_BOSS2 = "daily_boss2";
		private const string DAILY_BOSS3 = "daily_boss3";
		private const string DAILY_INVINCIBILITY = "daily_invincibility";
		private const string DAILY_LEVELUP = "daily_levelup";
		private const string DAILY_FROZEN = "daily_frozen";
		private const string DAILY_DEATHRAY = "daily_deathray";
		#endregion

		#region properties
		private static int highScore;

		private static int basicKilled;
		private static int boomerangKilled;
		private static int zigzagKilled;
		private static int chargerKilled;
		private static int legionKilled;
		private static int followerKilled;
		private static int boss1Killed;
		private static int boss2Killed;
		private static int boss3Killed;

		private static int invencibilityCollected;
		private static int levelUpCollected;
		private static int frozenCollected;
		private static int deathRayCollected;
		#endregion

		#region get / set for daily stats
		public static int HighScore
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_HIGHSCORE))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_HIGHSCORE);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_HIGHSCORE, value);
				PlayerPrefs.Save();
			}
		}

		public static int BasicsKilled
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_BASIC))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_BASIC);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_BASIC, value);
				PlayerPrefs.Save();
			}
		}

		public static int BoomerangsKilled
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_BOOMERANG))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_BOOMERANG);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_BOOMERANG, value);
				PlayerPrefs.Save();
			}
		}

		public static int ZigZagsKilled
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_ZIGZAG))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_ZIGZAG);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_ZIGZAG, value);
				PlayerPrefs.Save();
			}
		}

		public static int ChargersKilled
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_CHARGER))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_CHARGER);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_CHARGER, value);
				PlayerPrefs.Save();
			}
		}

		public static int LegionsKilled
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_LEGION))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_LEGION);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_LEGION, value);
				PlayerPrefs.Save();
			}
		}

		public static int FollowersKilled
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_FOLLOWER))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_FOLLOWER);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_FOLLOWER, value);
				PlayerPrefs.Save();
			}
		}

		public static int Boss1Killed
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_BOSS1))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_BOSS1);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_BOSS1, value);
				PlayerPrefs.Save();
			}
		}

		public static int Boss2Killed
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_BOSS2))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_BOSS2);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_BOSS2, value);
				PlayerPrefs.Save();
			}
		}

		public static int Boss3Killed
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_BOSS3))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_BOSS3);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_BOSS3, value);
				PlayerPrefs.Save();
			}
		}

		public static int InvincibilityCollected
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_INVINCIBILITY))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_INVINCIBILITY);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_INVINCIBILITY, value);
				PlayerPrefs.Save();
			}
		}

		public static int LevelUpCollected
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_LEVELUP))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_LEVELUP);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_LEVELUP, value);
				PlayerPrefs.Save();
			}
		}

		public static int FrozenCollected
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_FROZEN))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_FROZEN);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_FROZEN, value);
				PlayerPrefs.Save();
			}
		}

		public static int DeathRayCollected
		{
			get 
			{
				if(!PlayerPrefs.HasKey(DAILY_DEATHRAY))
					return 0;
				
				return PlayerPrefs.GetInt(DAILY_DEATHRAY);
			}

			set
			{
				PlayerPrefs.SetInt(DAILY_DEATHRAY, value);
				PlayerPrefs.Save();
			}
		}
		#endregion

	#endregion

	void OnEnable()
	{
		if(RewardCooldownLeft > 0)
			BuildMissions();
		else
		{
			while(RewardCooldownLeft <= 0)
				SortNewMissions();
		}
	}

	void Start()
	{
		MenuController.OnPanelClosed += BuildMissions;
	}

	// Update is called once per frame
	void Update () 
	{
		if(MenuController.CurrentMenu != MenuController.Menus.HUBConnection) return;

		if(RewardCooldownLeft >= 0)
		{
			int timeLeft = RewardCooldownLeft;

			int hours = (int)(timeLeft / 3600f);
			timeLeft -= (int)(hours * 3600f);

			int minutes = (int)(timeLeft / 60f);
			timeLeft -= (int)(minutes * 60f);

			int seconds = (int)timeLeft;

			countdown.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);

			if(RewardCooldownLeft < 3600)
				countdown.color = Color.red;
			else
				countdown.color = countdownNormalColor;
		}
		else
		{
			SortNewMissions();
		}
	}

	private void SortNewMissions()
	{
		foreach(DailyMissionObject mission in MissionObjects)
			mission.Sort();

		//added 24 more hours
		SetRewardCooldownTime(RewardCooldownTime.AddHours(24f));

		Reset();
		BuildMissions();
	}

	private void BuildMissions()
	{
		foreach(DailyMissionObject mission in MissionObjects)
			mission.Build();
	}

	private void Reset()
	{
		HighScore = 0;

		BasicsKilled = 0;
		BoomerangsKilled = 0;
		ZigZagsKilled = 0;
		ChargersKilled = 0;
		LegionsKilled = 0;
		FollowersKilled = 0;
		Boss1Killed = 0;
		Boss2Killed = 0;
		Boss3Killed = 0;

		InvincibilityCollected = 0;
		DeathRayCollected = 0;
		LevelUpCollected = 0;
		FrozenCollected = 0;
	}
}