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
		float creepsKll = (float)(Global.BasicsKilled + Global.BoomerangsKilled + Global.ZigZagsKilled + 
		                          Global.ChargersKilled + Global.LegionsKilled + Global.FollowersKilled + 
		                          Global.Boss1Killed + Global.Boss2Killed + Global.Boss3Killed);

		creepsKilled.Find("Value").GetComponent<UILabel>().text = creepsKll.ToString();

		averageCreepsKilled.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0.00}",(creepsKll / (float)Global.GamesPlayed));

		basicKilled.Find("Value").GetComponent<UILabel>().text = Global.BasicsKilled.ToString();
		boomerangKilled.Find("Value").GetComponent<UILabel>().text = Global.BoomerangsKilled.ToString();
		zigzagKilled.Find("Value").GetComponent<UILabel>().text = Global.ZigZagsKilled.ToString();
		chargerKilled.Find("Value").GetComponent<UILabel>().text = Global.ChargersKilled.ToString();
		legionKilled.Find("Value").GetComponent<UILabel>().text = Global.LegionsKilled.ToString();
		followerKilled.Find("Value").GetComponent<UILabel>().text = Global.FollowersKilled.ToString();
		boss1Killed.Find("Value").GetComponent<UILabel>().text = Global.Boss1Killed.ToString();
		boss2Killed.Find("Value").GetComponent<UILabel>().text = Global.Boss2Killed.ToString();
		boss3Killed.Find("Value").GetComponent<UILabel>().text = Global.Boss3Killed.ToString();
		orbsCollected.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0,0}", Global.OrbsCollected);
		orbsSpent.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0,0}", Global.OrbsSpent);
		orbsMissed.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0,0}", Global.OrbsMissed);

		timeInGame.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimePlayed);

		float energy = Global.EnergySpent * 70023f;

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

		energySpent.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0.00} {1}", energy, unity);

		matchesPlayed.Find("Value").GetComponent<UILabel>().text = Global.GamesPlayed.ToString();
		bossEncounters.Find("Value").GetComponent<UILabel>().text = Global.BossEncounters.ToString();
		frozenCollected.Find("Value").GetComponent<UILabel>().text = Global.FrozenCollected.ToString();
		invencibilityCollected.Find("Value").GetComponent<UILabel>().text = Global.InvencibilityCollected.ToString();
		deathRayCollected.Find("Value").GetComponent<UILabel>().text = Global.DeathRayCollected.ToString();
		levelUpCollected.Find("Value").GetComponent<UILabel>().text = Global.LevelUpCollected.ToString();
		powerUpsCollected.Find("Value").GetComponent<UILabel>().text = (Global.FrozenCollected + Global.InvencibilityCollected + Global.LevelUpCollected + Global.DeathRayCollected).ToString();
		powerUpsMissed.Find("Value").GetComponent<UILabel>().text = Global.PowerUpsMissed.ToString();
		hitsByBasic.Find("Value").GetComponent<UILabel>().text = Global.HitsByBasic.ToString();
		hitsByBoomerang.Find("Value").GetComponent<UILabel>().text = Global.HitsByBoomerang.ToString();
		hitsByZigZag.Find("Value").GetComponent<UILabel>().text = Global.HitsByZigZag.ToString();
		hitsByCharger.Find("Value").GetComponent<UILabel>().text = Global.HitsByCharger.ToString();
		hitsByLegion.Find("Value").GetComponent<UILabel>().text = Global.HitsByLegion.ToString();
		hitsByFollower.Find("Value").GetComponent<UILabel>().text = Global.HitsByFollower.ToString();
		hitsByBoss1.Find("Value").GetComponent<UILabel>().text = Global.HitsByBoss1.ToString();
		hitsByBoss2.Find("Value").GetComponent<UILabel>().text = Global.HitsByBoss2.ToString();
		hitsByBoss3.Find("Value").GetComponent<UILabel>().text = Global.HitsByBoss3.ToString();

		hitsTaken.Find("Value").GetComponent<UILabel>().text = (Global.HitsByBasic + Global.HitsByBoomerang +
		                                                             Global.HitsByZigZag + Global.HitsByCharger + 
		                                                             Global.HitsByLegion + Global.HitsByFollower + 
		                                                             Global.HitsByBoss1 + Global.HitsByBoss2 + 
		                                                             Global.HitsByBoss3).ToString();

		longestMatch.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0.0} " + Localization.Get("SECONDS"), Global.LongestMatch);

		float averageMatch = (float)Global.TimePlayed / (float)Global.GamesPlayed;
		averageMatchTime.Find("Value").GetComponent<UILabel>().text = string.Format("{0:0.0} " + Localization.Get("SECONDS"), averageMatch);

		enemiesMissed.Find("Value").GetComponent<UILabel>().text = Global.EnemiesMissed.ToString();
		highScore.Find("Value").GetComponent<UILabel>().text = Global.HighScore.ToString();
		longestKillStreak.Find("Value").GetComponent<UILabel>().text = Global.MaxStreak.ToString();

		timeOnLeftSide.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnLeft);
		timeOnRightSide.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnRight);
		timeOnSpecial1.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial1);
		timeOnSpecial2.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial2);
		timeOnSpecial3.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial3);
		timeOnSpecial4.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial4);
		timeOnSpecial5.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial5);
		timeOnSpecial6.Find("Value").GetComponent<UILabel>().text = FormatHour(Global.TimeOnSpecial6);
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
