using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DailyMissionObject : MonoBehaviour 
{
	#region Get  / Set
	private DailyMission CurrentMission
	{
		get { return missions[GetMissionID()]; }
	}

	public string Description
	{
		get { return CurrentMission.Description; }
	}

	public int Reward
	{
		get { return CurrentMission.reward; }
	}

	public int Value
	{
		get { return CurrentMission.value; }
	}

	public float Progress
	{
		get { return GetParameter(); }
	}

	public bool IsCompleted
	{
		get { return GetParameter() / CurrentMission.value > 1; }
	}
	#endregion

	public List<DailyMission> missions;

	private DailyMission currentMission;

	private int missionNumber = 0;
	private int MissionNumber
	{
		get 
		{ 
			if(missionNumber == 0)
				missionNumber = int.Parse(gameObject.name.Substring(gameObject.name.Length - 1, 1)); 

			return missionNumber;
		}
	}

	void Start()
	{
		LocalizationController.OnChanged += LanguageChanged;
		GameController.OnGameOver += VerifyUnlockment;
	}

	public void Sort()
	{
		int id = (int)UnityEngine.Random.Range(0, missions.Count);
		currentMission = missions[id];
		
		if(MissionNumber == 1)
		{
			Global.Mission1ID = id;
			Global.Mission1Completed = false;
		}
		else if(MissionNumber == 2)
		{
			Global.Mission2ID = id;
			Global.Mission2Completed = false;
		}
		else if(MissionNumber == 3)
		{
			Global.Mission3ID = id;
			Global.Mission3Completed = false;
		}
	}

	public void Build()
	{
		currentMission = missions[GetMissionID()];
		bool completed = GetMissionCompleted();

		//transform.FindChild("Completed").gameObject.SetActive(completed);
		//transform.FindChild("Normal").gameObject.SetActive(!completed);

		Debug.Log(currentMission.mission + " = " + GetParameter() + " / " + currentMission.value + " (" + Mathf.Min(GetParameter() / (float)currentMission.value, 1) + ")");

		transform.FindChild("Normal").FindChild("Description").GetComponent<UILabel>().text = (completed) ? Localization.Get("COMPLETED") : currentMission.Description;
		transform.FindChild("Normal").FindChild("Description").GetComponent<UILabel>().color = (completed) ? DailyMissionController.Instance.missionCompleteColor : DailyMissionController.Instance.countdownNormalColor;

		transform.FindChild("Normal").FindChild("reward").GetComponent<UILabel>().text = "+" + currentMission.reward.ToString();
		transform.FindChild("Normal").FindChild("bg-green").GetComponent<UISprite>().fillAmount = Mathf.Min(GetParameter() / (float)currentMission.value, 1);

		transform.FindChild("Normal").FindChild("reward").GetComponent<UILabel>().enabled = !completed;
		transform.FindChild("Normal").FindChild("orb icon").GetComponent<UISprite>().enabled = !completed;
	}

	private void VerifyUnlockment()
	{
		if(GetMissionCompleted()) return;

		float perc = Mathf.Min((float)GetParameter() / (float)currentMission.value, 1);
		
		if(perc == 1f)
			Unlock();
	}

	private void Unlock()
	{
		DailyMissionController.missionRecentUnlocked.Add(currentMission);

		if(MissionNumber == 1)
			Global.Mission1Completed = true;
		if(MissionNumber == 2)
			Global.Mission2Completed = true;
		if(MissionNumber == 3)
			Global.Mission3Completed = true;
	}

	private void LanguageChanged()
	{
		transform.FindChild("Normal").FindChild("Description").GetComponent<UILabel>().text = currentMission.Description;
	}

	private bool GetMissionCompleted()
	{
		bool completed = false;
		
		if(MissionNumber == 1)
			completed = Global.Mission1Completed;
		if(MissionNumber == 2)
			completed = Global.Mission2Completed;
		if(MissionNumber == 3)
			completed = Global.Mission3Completed;
		
		return completed;
	}
	
	private int GetMissionID()
	{
		int id = -1;
		
		if(MissionNumber == 1)
			id = Global.Mission1ID;
		if(MissionNumber == 2)
			id = Global.Mission2ID;
		if(MissionNumber == 3)
			id = Global.Mission3ID;

		return id;
	}

	private float GetParameter()
	{
		float parameter = 0;

		switch(currentMission.mission)
		{
			case DailyMission.Mission.HighScore:
				parameter = DailyMissionController.HighScore;
			break;

			case DailyMission.Mission.KillCreeps:
				parameter = DailyMissionController.BasicsKilled + DailyMissionController.BoomerangsKilled + 
							DailyMissionController.ZigZagsKilled + DailyMissionController.ChargersKilled + 
							DailyMissionController.LegionsKilled + DailyMissionController.FollowersKilled +
							DailyMissionController.Boss1Killed + DailyMissionController.Boss2Killed + 
							DailyMissionController.Boss3Killed;
			break;
			
			case DailyMission.Mission.KillBlue:
				parameter = DailyMissionController.BasicsKilled;
			break;

			case DailyMission.Mission.KillSpiral:
				parameter = DailyMissionController.BoomerangsKilled;
			break;

			case DailyMission.Mission.KillZigZag:
				parameter = DailyMissionController.ZigZagsKilled;
			break;
			
			case DailyMission.Mission.KillCharger:
				parameter = DailyMissionController.ChargersKilled;
			break;

			case DailyMission.Mission.KillLegion:
				parameter = DailyMissionController.LegionsKilled;
			break;
			
			case DailyMission.Mission.KillFollower:
				parameter = DailyMissionController.FollowersKilled;
			break;

			case DailyMission.Mission.KillMeteormite:
				parameter = DailyMissionController.Boss1Killed;
			break;
			
			case DailyMission.Mission.KillLegiworm:
				parameter = DailyMissionController.Boss2Killed;
			break;

			case DailyMission.Mission.KillPsyquor:
				parameter = DailyMissionController.Boss3Killed;
			break;
			
			case DailyMission.Mission.CollectDeathRay:
				parameter = DailyMissionController.DeathRayCollected;
			break;

			case DailyMission.Mission.CollectFrozen:
				parameter = DailyMissionController.FrozenCollected;
			break;
			
			case DailyMission.Mission.CollectInvincibility:
				parameter = DailyMissionController.InvincibilityCollected;
			break;

			case DailyMission.Mission.CollectLevelUp:
				parameter = DailyMissionController.LevelUpCollected;
			break;
		}

		return parameter;
	}
}

[System.Serializable]
public class DailyMission
{
	public enum Mission
	{
		HighScore,
		KillCreeps,
		KillBlue,
		KillSpiral,
		KillZigZag,
		KillCharger,
		KillLegion,
		KillFollower,
		KillMeteormite,
		KillLegiworm,
		KillPsyquor,
		CollectInvincibility,
		CollectDeathRay,
		CollectFrozen,
		CollectLevelUp,
	}
	
	public string Description
	{
		get
		{
			string descr = "";
			switch(mission)
			{
				case Mission.HighScore:
					descr = string.Format(Localization.Get("DAILYMISSION_POINTS"), value);
				break;

				case Mission.KillCreeps:
					descr = string.Format(Localization.Get("DAILYMISSION_CREEP"), value);
				break;
				
				case Mission.KillBlue:
					descr = string.Format(Localization.Get("DAILYMISSION_BLU"), value);
				break;

				case Mission.KillSpiral:
					descr = string.Format(Localization.Get("DAILYMISSION_BOOMERANG"), value);
				break;

				case Mission.KillZigZag:
					descr = string.Format(Localization.Get("DAILYMISSION_ZIGZAG"), value);
				break;
				
				case Mission.KillCharger:
					descr = string.Format(Localization.Get("DAILYMISSION_CHARGER"), value);
				break;

				case Mission.KillLegion:
					descr = string.Format(Localization.Get("DAILYMISSION_LEGION"), value);
				break;
				
				case Mission.KillFollower:
					descr = string.Format(Localization.Get("DAILYMISSION_FOLLOWER"), value);
				break;

				case Mission.KillMeteormite:
					descr = string.Format(Localization.Get("DAILYMISSION_BOSS1"), value);
				break;
				
				case Mission.KillLegiworm:
					descr = string.Format(Localization.Get("DAILYMISSION_BOSS2"), value);
				break;

				case Mission.KillPsyquor:
					descr = string.Format(Localization.Get("DAILYMISSION_BOSS3"), value);
				break;
				
				case Mission.CollectDeathRay:
					descr = string.Format(Localization.Get("DAILYMISSION_DEATHRAY"), value);
				break;

				case Mission.CollectFrozen:
					descr = string.Format(Localization.Get("DAILYMISSION_FROZEN"), value);
				break;
				
				case Mission.CollectInvincibility:
					descr = string.Format(Localization.Get("DAILYMISSION_INVINCIBILITY"), value);
				break;

				case Mission.CollectLevelUp:
					descr = string.Format(Localization.Get("DAILYMISSION_LEVELUP"), value);
				break;
			}
			return descr;
		}
	}
	
	public Mission mission;
	public int value;
	public int reward;
}