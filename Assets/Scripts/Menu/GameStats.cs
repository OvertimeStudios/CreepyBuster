using UnityEngine;
using System.Collections;

public class GameStats : MonoBehaviour 
{
	public Transform creepsKilled;
	public Transform averageCreepsKilled;
	public Transform basicKilled;
	public Transform boomerangKilled;
	public Transform zigzagKilled;
	public Transform chargerKilled;
	public Transform legionKilled;
	public Transform followerKilled;
	public Transform boss1Killed;
	public Transform boss2Killed;
	public Transform boss3Killed;
	public Transform orbsCollected;
	public Transform orbsSpent;
	public Transform orbsMissed;
	public Transform timeInGame;
	public Transform energySpent;
	public Transform matchesPlayed;
	public Transform bossEncounters;
	public Transform frozenCollected;
	public Transform levelUpCollected;
	public Transform invencibilityCollected;
	public Transform deathRayCollected;
	public Transform powerUpsCollected;
	public Transform powerUpsMissed;
	public Transform hitsByBasic;
	public Transform hitsByBoomerang;
	public Transform hitsByZigZag;
	public Transform hitsByCharger;
	public Transform hitsByLegion;
	public Transform hitsByFollower;
	public Transform hitsByBoss1;
	public Transform hitsByBoss2;
	public Transform hitsByBoss3;
	public Transform hitsTaken;
	public Transform enemiesMissed;
	public Transform longestMatch;
	public Transform averageMatchTime;
	public Transform highScore;
	public Transform longestKillStreak;
	public Transform timeOnLeftSide;
	public Transform timeOnRightSide;
	public Transform timeOnSpecial1;
	public Transform timeOnSpecial2;
	public Transform timeOnSpecial3;
	public Transform timeOnSpecial4;
	public Transform timeOnSpecial5;
	public Transform timeOnSpecial6;

	void OnEnable()
	{
		creepsKilled.FindChild("Value").GetComponent<UILabel>().text = (Global.BasicsKilled + Global.BoomerangsKilled + Global.ZigZagsKilled + 
																		Global.ChargersKilled + Global.LegionsKilled + Global.FollowersKilled + 
																		Global.Boss1Killed + Global.Boss2Killed + Global.Boss3Killed).ToString();

		averageCreepsKilled.FindChild("Value").GetComponent<UILabel>().text = string.Format("{0:0.0}",((float)(Global.BasicsKilled + Global.BoomerangsKilled + Global.ZigZagsKilled + 
		                                                                       Global.ChargersKilled + Global.LegionsKilled + Global.FollowersKilled + 
		                                                                       Global.Boss1Killed + Global.Boss2Killed + Global.Boss3Killed) / (float)Global.GamesPlayed).ToString());

		basicKilled.FindChild("Value").GetComponent<UILabel>().text = Global.BasicsKilled.ToString();
		boomerangKilled.FindChild("Value").GetComponent<UILabel>().text = Global.BoomerangsKilled.ToString();
		zigzagKilled.FindChild("Value").GetComponent<UILabel>().text = Global.ZigZagsKilled.ToString();
		chargerKilled.FindChild("Value").GetComponent<UILabel>().text = Global.ChargersKilled.ToString();
		legionKilled.FindChild("Value").GetComponent<UILabel>().text = Global.LegionsKilled.ToString();
		followerKilled.FindChild("Value").GetComponent<UILabel>().text = Global.FollowersKilled.ToString();
		boss1Killed.FindChild("Value").GetComponent<UILabel>().text = Global.Boss1Killed.ToString();
		boss2Killed.FindChild("Value").GetComponent<UILabel>().text = Global.Boss2Killed.ToString();
		boss3Killed.FindChild("Value").GetComponent<UILabel>().text = Global.Boss3Killed.ToString();
		orbsCollected.FindChild("Value").GetComponent<UILabel>().text = Global.OrbsCollected.ToString();
		orbsSpent.FindChild("Value").GetComponent<UILabel>().text = Global.OrbsSpent.ToString();
		orbsMissed.FindChild("Value").GetComponent<UILabel>().text = Global.OrbsMissed.ToString();

		timeInGame.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimePlayed);

		float energy = Global.EnergySpent * 70023f;
		Debug.Log(energy);
		string unity = "watt";
		if(energy > 1000000000f)
		{
			unity = "Giga" + unity;
			energy /= 1000000000f;
		}
		else if(energy > 1000000f)
		{
			unity = "Mega" + unity;
			energy /= 1000000f;
		}
		else if(energy > 1000f)
		{
			unity = "Kilo" + unity;
			energy /= 1000f;
		}
		else
			unity = unity.Substring(0, 1).ToUpper() + unity.Substring(1);

		energySpent.FindChild("Value").GetComponent<UILabel>().text = string.Format("{0:0.00} {1}", energy, unity);

		matchesPlayed.FindChild("Value").GetComponent<UILabel>().text = Global.GamesPlayed.ToString();
		bossEncounters.FindChild("Value").GetComponent<UILabel>().text = Global.BossEncounters.ToString();
		frozenCollected.FindChild("Value").GetComponent<UILabel>().text = Global.FrozenCollected.ToString();
		invencibilityCollected.FindChild("Value").GetComponent<UILabel>().text = Global.InvencibilityCollected.ToString();
		deathRayCollected.FindChild("Value").GetComponent<UILabel>().text = Global.DeathRayCollected.ToString();
		levelUpCollected.FindChild("Value").GetComponent<UILabel>().text = Global.LevelUpCollected.ToString();
		powerUpsCollected.FindChild("Value").GetComponent<UILabel>().text = (Global.FrozenCollected + Global.InvencibilityCollected + Global.LevelUpCollected).ToString();
		powerUpsMissed.FindChild("Value").GetComponent<UILabel>().text = Global.PowerUpsMissed.ToString();
		hitsByBasic.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByBasic.ToString();
		hitsByBoomerang.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByBoomerang.ToString();
		hitsByZigZag.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByZigZag.ToString();
		hitsByCharger.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByCharger.ToString();
		hitsByLegion.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByLegion.ToString();
		hitsByFollower.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByFollower.ToString();
		hitsByBoss1.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByBoss1.ToString();
		hitsByBoss2.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByBoss2.ToString();
		hitsByBoss3.FindChild("Value").GetComponent<UILabel>().text = Global.HitsByBoss3.ToString();

		hitsTaken.FindChild("Value").GetComponent<UILabel>().text = (Global.HitsByBasic + Global.HitsByBoomerang +
		                                                             Global.HitsByZigZag + Global.HitsByCharger + 
		                                                             Global.HitsByLegion + Global.HitsByZigZag + 
		                                                             Global.HitsByBoss1 + Global.HitsByBoss2 + 
		                                                             Global.HitsByBoss3).ToString();

		longestMatch.FindChild("Value").GetComponent<UILabel>().text = string.Format("{0} " + Localization.Get("SECONDS"), Global.LongestMatch);

		float averageMatch = (float)Global.TimePlayed / (float)Global.GamesPlayed;
		averageMatchTime.FindChild("Value").GetComponent<UILabel>().text = string.Format("{0} " + Localization.Get("SECONDS"), averageMatch);

		enemiesMissed.FindChild("Value").GetComponent<UILabel>().text = Global.EnemiesMissed.ToString();
		highScore.FindChild("Value").GetComponent<UILabel>().text = Global.HighScore.ToString();
		longestKillStreak.FindChild("Value").GetComponent<UILabel>().text = Global.MaxStreak.ToString();

		timeOnLeftSide.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnLeft);

		timeOnRightSide.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnRight);

		timeOnSpecial1.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial1);

		timeOnSpecial2.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial2);

		timeOnSpecial3.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial3);

		timeOnSpecial4.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial4);

		timeOnSpecial5.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial5);

		timeOnSpecial6.FindChild("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial6);
	}

	private static string FormatHour(float time)
	{
		int days = (int)Mathf.Floor(time / (3600f * 24f));
		time -= days * 3600f * 24f;
		int hours = (int)Mathf.Floor(time / 3600f);
		time -= hours * 3600f;
		int minutes = (int)Mathf.Floor(time / 60f);
		time -= minutes * 60f;
		int seconds = (int)Mathf.Floor(time);

		return string.Format("{0}D, {1}H, {2}M, {3}S", days, hours, minutes, seconds);
	}
}
