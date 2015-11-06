using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Achievement : MonoBehaviour 
{
	private const string KILL_CREEPS = "ACHIEVEMENT_DESTROY_CREEPS";
	private const string KILL_BASIC = "ACHIEVEMENT_DESTROY_BASIC";
	private const string KILL_BOOMERANG = "ACHIEVEMENT_DESTROY_BOOMERANG";
	private const string KILL_ZIGZAG = "ACHIEVEMENT_DESTROY_ZIGZAG";
	private const string KILL_CHARGER = "ACHIEVEMENT_DESTROY_CHARGER";
	private const string KILL_LEGION = "ACHIEVEMENT_DESTROY_LEGION";
	private const string KILL_FOLLOWER = "ACHIEVEMENT_DESTROY_FOLLOWER";
	private const string KILL_BOSS1 = "ACHIEVEMENT_BOSS1";
	private const string KILL_BOSS2 = "ACHIEVEMENT_BOSS2";
	private const string KILL_BOSS3 = "ACHIEVEMENT_BOSS3";
	private const string STREAK = "ACHIEVEMENT_STREAK";
	private const string POINTS = "ACHIEVEMENT_POINTS";
	private const string HOUR = "ACHIEVEMENT_HOURS";
	private const string GAMES = "ACHIEVEMENT_PLAYS";
	private const string ENERGY = "ACHIEVEMENT_ENERGY";
	private const string UPGRADES = "ACHIEVEMENT_UPGRADES";
	private const string LEFT_RIGHT = "ACHIEVEMENT_LEFTRIGHT";
	private const string MENU = "ACHIEVEMENT_MENU";

	public enum Type
	{
		KillCreeps,
		KillBasic,
		KillBoomerang,
		KillZigZag,
		KillCharger,
		KillLegion,
		KillFollower,
		KillMeteormite,
		KillLegiworm,
		KillPsyquor,
		Streak,
		Points,
		Hours,
		Games,
		Special,
		EnergySpent,
		Upgrades,
		LeftRight,
		Menu,
	}

	public Type type;
	public int orbReward;
	public int value;
	public bool hidden = false;

	[Header("For Developer")]
	public UILabel titleLabel;
	public UILabel descriptionLabel;
	public UISprite fill;
	public UILabel percentLabel;
	public UILabel orbRewardLabel;

	public static List<AchievementUnlocked> achievementRecentUnlocked  = new List<AchievementUnlocked>();

	private float percent;
	[HideInInspector]
	public bool unlocked;

	void OnEnable()
	{
		percent = (unlocked) ? 1f : Mathf.Min((float)GetParameter() / (float)value, 1);

		percentLabel.text = (Mathf.Round(percent * 100f)).ToString() + "%";
		fill.fillAmount = percent;
	}

	// Use this for initialization
	void Start () 
	{
		LocalizationController.OnChanged += LanguageChanged;
		GameController.OnGameOver += VerifyUnlockment;

		unlocked = Global.IsAchievementUnlocked(type, value);

		titleLabel.text = GetTitle();
		descriptionLabel.text = GetDescription();

		orbRewardLabel.text = (hidden && !unlocked) ? "???" : orbReward.ToString();
	}

	private string GetTitle()
	{
		if(hidden && !unlocked)
			return "???";

		string title = GetPrefix() + "_TITLE";

		//add number if applyable
		string number = gameObject.name.Substring(gameObject.name.Length - 1);
		int n;

		if(int.TryParse(number,out n))
			title += number;

		return Localization.Get(title);
	}

	private string GetDescription()
	{
		if(hidden && !unlocked)
			return "???";

		string description = GetPrefix() + "_DESCRIPTION";

		if(type == Type.Hours)
			return string.Format(Localization.Get(description), (int)((float)value / 3600f));

		if(type == Type.Menu)
			return string.Format(Localization.Get(description), (int)((float)value / 60f));

		return string.Format(Localization.Get(description), value);
	}

	private string GetPrefix()
	{
		string prefix = "";

		switch(type)
		{
			case Type.KillCreeps:
				prefix = KILL_CREEPS;
				break;

			case Type.KillBasic:
				prefix = KILL_BASIC;
				break;

			case Type.KillBoomerang:
				prefix = KILL_BOOMERANG;
				break;

			case Type.KillCharger:
				prefix = KILL_CHARGER;
				break;

			case Type.KillFollower:
				prefix = KILL_FOLLOWER;
				break;

			case Type.KillLegion:
				prefix = KILL_LEGION;
				break;

			case Type.KillZigZag:
				prefix = KILL_ZIGZAG;
				break;

			case Type.KillMeteormite:
				prefix = KILL_BOSS1;
				break;
				
			case Type.KillLegiworm:
				prefix = KILL_BOSS2;
				break;
				
			case Type.KillPsyquor:
				prefix = KILL_BOSS3;
				break;
				
			case Type.EnergySpent:
				prefix = ENERGY;
				break;
			
			case Type.Hours:
				prefix = HOUR;
				break;

			case Type.Games:
				prefix = GAMES;
				break;
			case Type.Points:
				prefix = POINTS;
				break;

			case Type.Streak:
				prefix = STREAK;
				break;

			case Type.Upgrades:
				prefix = UPGRADES;
				break;

			case Type.LeftRight:
				prefix = LEFT_RIGHT;
				break;

			case Type.Menu:
				prefix = MENU;
				break;
		}

		return prefix;
	}

	private float GetParameter()
	{
		float parameter = 0;

		switch(type)
		{
			case Type.KillCreeps:
				parameter = Global.BasicsKilled + Global.BoomerangsKilled + Global.ZigZagsKilled + 
							Global.ChargersKilled + Global.LegionsKilled + Global.FollowersKilled + 
							Global.Boss1Killed + Global.Boss2Killed + Global.Boss3Killed;
				break;

			case Type.KillBasic:
				parameter = Global.BasicsKilled;
				break;

			case Type.KillBoomerang:
				parameter = Global.BoomerangsKilled;
				break;

			case Type.KillCharger:
				parameter = Global.ChargersKilled;
				break;

			case Type.KillFollower:
				parameter = Global.FollowersKilled;
				break;

			case Type.KillLegion:
				parameter = Global.LegionsKilled;
				break;

			case Type.KillZigZag:
				parameter = Global.ZigZagsKilled;
				break;

			case Type.KillMeteormite:
				parameter = Global.Boss1Killed;
				break;
				
			case Type.KillLegiworm:
				parameter = Global.Boss2Killed;
				break;
				
			case Type.KillPsyquor:
				parameter = Global.Boss3Killed;
				break;
				
			case Type.Streak:
				parameter = Global.MaxStreak;
				break;

			case Type.Points:
				parameter = Global.HighScore;
				break;

			case Type.Games:
				parameter = Global.GamesPlayed;
				break;

			case Type.Hours:
				parameter = Global.TimePlayed;
				break;

			case Type.EnergySpent:
				parameter = Global.EnergySpent;
				break;

			case Type.Menu:
				parameter = (int)MenuController.timeSpentOnMenu;
				break;

			case Type.Upgrades:
				parameter = Global.UpgradesBought;
				break;

			 case Type.LeftRight:
				parameter = Global.SideLeftRight;
				break;
		}

		return parameter;
	}

	private void VerifyUnlockment()
	{
		if(unlocked) return;
		
		float perc = Mathf.Min((float)GetParameter() / (float)value, 1);
		
		if(perc == 1f)
			Unlock();
	}

	public void Unlock()
	{
		unlocked = true;
		achievementRecentUnlocked.Add(new AchievementUnlocked(GetTitle(), GetDescription(), orbReward));
		Global.UnlockAchievement(type, value);

		titleLabel.text = GetTitle();
		descriptionLabel.text = GetDescription();
		
		orbRewardLabel.text = (hidden && !unlocked) ? "???" : orbReward.ToString();
	}

	private void LanguageChanged()
	{
		titleLabel.text = GetTitle();
		descriptionLabel.text = GetDescription();
	}
}

public class AchievementUnlocked
{
	public string title;
	public string description;
	public int orbReward;

	public AchievementUnlocked(string title, string description, int orbReward)
	{
		this.title = title;
		this.description = description;
		this.orbReward = orbReward;
	}
}
