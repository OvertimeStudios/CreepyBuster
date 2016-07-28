using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementController : MonoBehaviour 
{
	public List<AchievementItem> achievements;

	void OnDestroy()
	{
		GameController.OnGameOver -= UpdateAllAchievements;
	}

	void Start()
	{
		GameController.OnGameOver += UpdateAllAchievements;
	}

	public void UpdateAllAchievements()
	{
		foreach(AchievementItem achievement in achievements)
			achievement.Update();
	}
}

[System.Serializable]
public class AchievementItem
{
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

	public string identifier;
	public Type type;
	public int value;

	private float percent;

	public void Update()
	{
		percent = Mathf.Min((float)GetParameter() / (float)value, 1);

		Debug.Log(string.Format("Updating achievement {0} {1} ({2}): {3}% ({4}/{5})", type, value, identifier, (percent * 100), (float)GetParameter(), (float)value));
		AchievementsHelper.ReportAchievement(identifier, percent * 100f);
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
}