using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LevelDesign : MonoBehaviour 
{
	#region Actions
	public static event Action OnPlayerLevelUp;
	public static event Action OnEnemiesTypesLevelUp;
	public static event Action OnEnemiesSpawnLevelUp;
	public static event Action OnEnemiesAttributesLevelUp;
	public static event Action OnTierLevelUp;
	public static event Action OnItensLevelUp;
	public static event Action OnBossReady;
	#endregion

	[Header("Player Level")]
	public PlayerLevelUpCondition[] playerLevelUpCondition;

	[Header("Special")]
	public int streakToSpecial;
	public float specialTime = 5f;
	public float specialBonusDamage = 1f;
	public int specialBonusTarget = 1;
	public Color specialColor = new Color(0, 0, 0, 0.3f);

	[Header("Enemies Level")]
	public EnemiesTypesLevelUpCondition[] enemiesTypesLevelUpCondition;

	public EnemiesSpawnLevelUpCondition[] enemiesSpawnLevelUpCondition;

	public EnemiesAttributesLevelUpCondition enemiesAttributesLevelUpCondition;
	
	[Header("Tier Level")]
	public LevelUpCondition[] tierLevelUpCondition;

	[Header("Boss Battle")]
	public GameObject bossMeteor;
	public GameObject bossTwins;
	public GameObject bossIllusion;
	public BossLevelUpCondition[] bossBattleCondition;
	public BossLevelUpCondition infinityBossBattleCondition;

	[Header("Backgrounds")]
	public List<GameObject> backgrounds;

	[Header("Item")]
	public RandomBetweenTwoConst itemSpawnTime;
	public ItemLevelUpCondition[] itensLevelUpCondition;

	[Header("Shop Itens")]
	public LevelFloatList[] rangeUpgrade;
	public LevelFloatList[] damageUpgrade;

	#region levels properties
	private static int playerLevel = 0;
	private static int enemiesSpawnLevel = 0;
	private static int enemiesTypesLevel = 0;
	private static int enemiesAttributeLevel = 0;
	private static int tierLevel = 0;
	private static int bossLevel = 0;
	private static int itemLevel = 0;
	#endregion

	#region get / set

	#region playerLevel
	/// <summary>
	/// Gets the streak difference to next player level. (i.e. level1 = 15, level2 = 15, level3 = 15)
	/// </summary>
	public static int StreakDifferenceToNextPlayerLevel
	{
		get 
		{
			if (!IsPlayerMaxLevel) 
			{
				return Instance.playerLevelUpCondition [LevelDesign.PlayerLevel + 1].killStreak - Instance.playerLevelUpCondition [LevelDesign.PlayerLevel].killStreak;
			}

			return Instance.streakToSpecial;
		}
	}

	/// <summary>
	/// Gets the streak necessary to unlock current level. (i.e. level1 = 15, level2 = 30, level3 = 45)
	/// </summary>
	public static int CurrentPlayerLevelUnlockStreak
	{
		get 
		{
			return Instance.playerLevelUpCondition [LevelDesign.PlayerLevel].killStreak + (Instance.streakToSpecial * GameController.specialStreak);
		}
	}

	/// <summary>
	/// Gets the next streak to player level up. (i.e. level2 = 30, level3 = 45, level4 = 60)
	/// </summary>
	public static int NextStreakToPlayerLevelUp
	{
		get 
		{
			if(IsPlayerMaxLevel)
			{
				//last streak + (streakToSpecial * how many specials it has been done so far)
				return Instance.playerLevelUpCondition[Instance.playerLevelUpCondition.Length - 1].killStreak + (Instance.streakToSpecial * (GameController.specialStreak + 1));
			}

			return Instance.playerLevelUpCondition[LevelDesign.PlayerLevel + 1].killStreak ;
		}
	}

	public static int LastLevelPlayerStreak
	{
		get 
		{
			if(LevelDesign.PlayerLevel == 0)
				return 0;

			return Instance.playerLevelUpCondition[LevelDesign.PlayerLevel - 1].killStreak ;
		}
	}

	public static int MaxPlayerLevel
	{
		get { return Global.RayLevel + 2; }
	}

	public static bool IsPlayerMaxLevel
	{
		get { return playerLevel == MaxPlayerLevel; }
	}
	#endregion

	#region enemies Level

	#region Type
	/// <summary>
	/// Gets a value indicating is enemy types max level.
	/// </summary>
	public static bool IsEnemyTypesMaxLevel
	{
		get { return LevelDesign.enemiesTypesLevel == MaxEnemyTypesLevel; }
	}

	/// <summary>
	/// Gets the max enemy types level.
	/// </summary>
	public static int MaxEnemyTypesLevel
	{
		get { return Instance.enemiesTypesLevelUpCondition.Length - 1; }
	}

	/// <summary>
	/// Gets the next streak to enemy types level up.
	/// </summary>
	public static int NextStreakToEnemyTypesLevelUp
	{
		get 
		{
			if(!LevelDesign.IsEnemyTypesMaxLevel)
				return Instance.enemiesTypesLevelUpCondition[enemiesTypesLevel + 1].killStreak;
			
			return 0;
		}
	}

	/// <summary>
	/// Gets the next score to enemy types level up.
	/// </summary>
	public static int NextScoreToEnemyTypesLevelUp
	{
		get 
		{
			if(!LevelDesign.IsEnemyTypesMaxLevel)
				return Instance.enemiesTypesLevelUpCondition[enemiesTypesLevel + 1].points;
			
			return 0;
		}
	}
	#endregion

	#region Spawn
	/// <summary>
	/// Gets a value indicating is enemy spawn max level.
	/// </summary>
	public static bool IsEnemySpawnMaxLevel
	{
		get { return LevelDesign.enemiesSpawnLevel == MaxEnemySpawnLevel; }
	}
	
	/// <summary>
	/// Gets the max enemy spawn level.
	/// </summary>
	public static int MaxEnemySpawnLevel
	{
		get { return Instance.enemiesSpawnLevelUpCondition.Length - 1; }
	}
	
	/// <summary>
	/// Gets the next streak to enemy spawn level up.
	/// </summary>
	public static int NextStreakToEnemySpawnLevelUp
	{
		get 
		{
			if(!LevelDesign.IsEnemySpawnMaxLevel)
				return Instance.enemiesSpawnLevelUpCondition[enemiesSpawnLevel + 1].killStreak;
			
			return 0;
		}
	}
	
	/// <summary>
	/// Gets the next score to enemy spawn level up.
	/// </summary>
	public static int NextScoreToEnemySpawnLevelUp
	{
		get 
		{
			if(!LevelDesign.IsEnemySpawnMaxLevel)
				return Instance.enemiesSpawnLevelUpCondition[enemiesSpawnLevel + 1].points;
			
			return 0;
		}
	}
	#endregion

	#region enemies attributes
	/// <summary>
	/// Gets the next streak to enemy attributes level up.
	/// </summary>
	public static int NextScoreToEnemyAttributesLevelUp {
		get { return Instance.enemiesAttributesLevelUpCondition.points * (enemiesAttributeLevel + 1); }
	}

	#endregion

	#region tier level
	/// <summary>
	/// Gets a value indicating if tier is max level.
	/// </summary>
	public static bool IsTierMaxLevel
	{
		get { return LevelDesign.tierLevel == MaxTierLevel; }
	}

	/// <summary>
	/// Gets the max tier level.
	/// </summary>
	public static int MaxTierLevel
	{
		get { return Instance.tierLevelUpCondition.Length - 1; }
	}

	public static int NextStreakToTierLevelUp
	{
		get 
		{
			if(!LevelDesign.IsTierMaxLevel)
				return Instance.tierLevelUpCondition[tierLevel + 1].killStreak;
			
			return 0;
		}
	}

	public static int NextScoreToTierLevelUp
	{
		get 
		{
			if(!LevelDesign.IsTierMaxLevel)
				return Instance.tierLevelUpCondition[tierLevel + 1].points;
			
			return 0;
		}
	}
	#endregion

	#region Boss
	public static int NextKillToBossBattle
	{
		get 
		{
			if(bossLevel < Instance.bossBattleCondition.Length)
				return Instance.bossBattleCondition[bossLevel].kills;
			else
				return Instance.bossBattleCondition[Instance.bossBattleCondition.Length - 1].kills + ((bossLevel - Instance.bossBattleCondition.Length - 1) * Instance.infinityBossBattleCondition.kills);
		}
	}

	public static GameObject CurrentBoss
	{
		get	
		{ 
			BossLevelUpCondition currentLevelUpCondition = (bossLevel < Instance.bossBattleCondition.Length) ? Instance.bossBattleCondition[bossLevel] : Instance.infinityBossBattleCondition;

			float maxPercent = currentLevelUpCondition.bossIllusion + currentLevelUpCondition.bossMeteor + currentLevelUpCondition.bossTwins;

			float rnd = UnityEngine.Random.Range(0f, maxPercent);

			if(rnd < currentLevelUpCondition.bossMeteor)
				return Instance.bossMeteor;
			else if(rnd < currentLevelUpCondition.bossTwins + currentLevelUpCondition.bossMeteor)
				return Instance.bossTwins;
			else
				return Instance.bossIllusion;
			
		}
	}


	#endregion

	#region Item
	public static float ItemSpawnTime
	{
		get { return Instance.itemSpawnTime.Random (); }
	}

	/// <summary>
	/// The probability of spawning an item
	/// </summary>
	public static float SpawnItemPercent
	{
		get { return Instance.itensLevelUpCondition[itemLevel].chanceToSpawn; }
	}

	public static List<ItemPercent> CurrentItens
	{
		get { return Instance.itensLevelUpCondition[itemLevel].itens; }
	}

	/// <summary>
	/// Gets a value indicating is item max level.
	/// </summary>
	public static bool IsItemMaxLevel
	{
		get { return LevelDesign.itemLevel == MaxItemLevel; }
	}
	
	/// <summary>
	/// Gets the max item level.
	/// </summary>
	public static int MaxItemLevel
	{
		get { return Instance.itensLevelUpCondition.Length - 1; }
	}
	
	/// <summary>
	/// Gets the next streak to item level up.
	/// </summary>
	public static int NextStreakToItemLevelUp
	{
		get 
		{
			if(!LevelDesign.IsItemMaxLevel)
				return Instance.itensLevelUpCondition[itemLevel + 1].killStreak;
			
			return 0;
		}
	}
	
	/// <summary>
	/// Gets the next score to item level up.
	/// </summary>
	public static int NextScoreToItemLevelUp
	{
		get 
		{
			if(!LevelDesign.IsItemMaxLevel)
				return Instance.itensLevelUpCondition[itemLevel + 1].points;
			
			return 0;
		}
	}
	#endregion

	#region Shop Upgrades
	public static float CurrentDamage
	{
		get
		{
			int up = 0;

			for(byte i = 0; i < Global.DamageLevel; i++)
				up++;

			return Instance.damageUpgrade[up].value;
		}
	}

	public static float CurrentRange
	{
		get
		{
			int up = 0;

			for(byte i = 0; i < Global.RangeLevel; i++)
				up++;
			
			return Instance.rangeUpgrade[up].value;
		}
	}

	#endregion

	#endregion

	public static List<EnemiesPercent> CurrentEnemies
	{
		get { return Instance.enemiesTypesLevelUpCondition[LevelDesign.EnemiesTypesLevel].enemies; }
	}

	public static float EnemiesSpawnTime
	{
		get { return Instance.enemiesSpawnLevelUpCondition[LevelDesign.EnemiesSpawnLevel].time; }
	}
	
	public static float EnemiesBonusLife
	{
		get { return Instance.enemiesAttributesLevelUpCondition.life * enemiesAttributeLevel; }
	}

	public static float EnemiesBonusVel
	{
		get { return Instance.enemiesAttributesLevelUpCondition.vel * enemiesAttributeLevel; }
	}

	public static int SpawnQuantity
	{
		get { return Instance.enemiesSpawnLevelUpCondition[LevelDesign.EnemiesSpawnLevel].quantity; }
	}

	public static int MaxRays
	{
		get { return Instance.playerLevelUpCondition[LevelDesign.PlayerLevel].maxRays + ((AttackTargets.IsSpecialActive) ? Instance.specialBonusTarget : 0); }
	}

	public static Color CurrentColor
	{
		get 
		{
			if(AttackTargets.IsSpecialActive)
				return Instance.specialColor;
			else
				return Instance.playerLevelUpCondition [LevelDesign.PlayerLevel].color; 
		}
	}

	public static bool IsSpecialReady
	{
		get { return LevelDesign.PlayerLevel >= Instance.playerLevelUpCondition.Length - 1 && GameController.StreakCount >= NextStreakToPlayerLevelUp; }
	}

	public static bool IsPlayerFullyUpgraded
	{
		get { return Global.RayLevel + 2 == Instance.playerLevelUpCondition.Length; }
	}

	public static int PlayerLevel
	{
		get { return playerLevel; }

		set 
		{ 
			bool levelUp = value >= playerLevel;

			playerLevel = value; 

			if(levelUp)
			{
				if(OnPlayerLevelUp != null)
					OnPlayerLevelUp();
			}
		}
	}

	public static int ItemLevel
	{
		get { return itemLevel; }
	}
	
	public static int EnemiesTypesLevel
	{
		get { return enemiesTypesLevel; }
	}
	
	public static int EnemiesSpawnLevel
	{
		get { return enemiesSpawnLevel; }
	}

	public static int EnemiesAttributesLevel
	{
		get { return enemiesAttributeLevel; }
	}
	
	public static int TierLevel
	{
		get { return tierLevel; }
	}

	public static int BossLevel
	{
		get { return bossLevel; }
	}

	#endregion

	#region singleton
	private static LevelDesign instance;
	public static LevelDesign Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<LevelDesign>();

			return instance;
		}
	}
	#endregion

	void OnEnable()
	{
		GameController.OnStreakUpdated += PlayerLevelUp;
		GameController.OnRealStreakUpdated += EnemiesTypesLevelUp;
		GameController.OnRealStreakUpdated += EnemiesSpawnLevelUp;
		GameController.OnRealStreakUpdated += EnemiesAttributesLevelUp;
		GameController.OnRealStreakUpdated += TierLevelUp;
		GameController.OnRealStreakUpdated += ItensLevelUp;
		GameController.OnKill += KillCountUpdated;

		GameController.OnGameStart += Reset;

		BossLife.OnBossDied += BossLevelUp;
	}

	void OnDisable()
	{
		GameController.OnStreakUpdated -= PlayerLevelUp;
		GameController.OnRealStreakUpdated -= EnemiesTypesLevelUp;
		GameController.OnRealStreakUpdated -= EnemiesSpawnLevelUp;
		GameController.OnRealStreakUpdated -= EnemiesAttributesLevelUp;
		GameController.OnRealStreakUpdated -= TierLevelUp;
		GameController.OnRealStreakUpdated -= ItensLevelUp;
		GameController.OnKill -= KillCountUpdated;

		GameController.OnGameStart -= Reset;

		BossLife.OnBossDied -= BossLevelUp;
	}

	private void PlayerLevelUp()
	{
		//don't level up anymore if it's not fully upgraded
		if(IsPlayerMaxLevel && !IsPlayerFullyUpgraded) return;

		if(GameController.StreakCount >= LevelDesign.NextStreakToPlayerLevelUp)
		{
			//call Action on set method
			PlayerLevel = Mathf.Clamp(PlayerLevel + 1, 0, MaxPlayerLevel);
		}
	}

	private void EnemiesTypesLevelUp()
	{
		if(!IsEnemyTypesMaxLevel)
		{
			if(GameController.Score >= LevelDesign.NextScoreToEnemyTypesLevelUp ||
			   GameController.RealStreakCount >= LevelDesign.NextStreakToEnemyTypesLevelUp)
			{
				enemiesTypesLevel++;

				if(OnEnemiesTypesLevelUp != null)
					OnEnemiesTypesLevelUp();
			}
		}
	}

	private void EnemiesSpawnLevelUp()
	{
		if(!IsEnemySpawnMaxLevel)
		{
			if(GameController.Score >= LevelDesign.NextScoreToEnemySpawnLevelUp ||
			   GameController.RealStreakCount >= LevelDesign.NextStreakToEnemySpawnLevelUp)
			{
				enemiesSpawnLevel++;
				
				if(OnEnemiesSpawnLevelUp != null)
					OnEnemiesSpawnLevelUp();
			}
		}
	}

	private void EnemiesAttributesLevelUp()
	{
		if(GameController.Score >= LevelDesign.NextScoreToEnemyAttributesLevelUp)
		{
			enemiesAttributeLevel++;
			
			if(OnEnemiesAttributesLevelUp != null)
				OnEnemiesAttributesLevelUp();
		}
	}

	private void TierLevelUp()
	{
		if(!IsTierMaxLevel)
		{
			if(GameController.Score >= LevelDesign.NextScoreToTierLevelUp ||
			   GameController.RealStreakCount >= LevelDesign.NextStreakToTierLevelUp)
			{
				tierLevel++;

				if(OnTierLevelUp != null)
					OnTierLevelUp();
			}
		}
	}

	private void ItensLevelUp()
	{
		if(!IsItemMaxLevel)
		{
			if(GameController.Score >= LevelDesign.NextScoreToItemLevelUp ||
			   GameController.RealStreakCount >= LevelDesign.NextStreakToItemLevelUp)
			{
				itemLevel++;
				
				if(OnItensLevelUp != null)
					OnItensLevelUp();
			}
		}
	}

	private void KillCountUpdated()
	{
		if(!GameController.IsBossTime && GameController.KillCount >= LevelDesign.NextKillToBossBattle)
		{
			if (OnBossReady != null)
				OnBossReady ();
		}
	}

	public static void BossLevelUp()
	{
		bossLevel++;
	}

	private void Reset()
	{
		playerLevel = 0;
		enemiesSpawnLevel = 0;
		enemiesTypesLevel = 0;
		enemiesAttributeLevel = 0;
		tierLevel = 0;
		bossLevel = 0;
		itemLevel = 0;
	}

	private void OnLoseStacks()
	{
		playerLevel = 0;
	}

	void OnGUI()
	{
		#if SHOW_GUI
		if(GameController.isGameRunning)
		{
			int w = Screen.width, h = Screen.height;

			GUIStyle style = new GUIStyle();
			
			Rect rect = new Rect(0, h * 10 / 100, w, h * 2 / 100);
			style.alignment = TextAnchor.UpperLeft;
			style.fontSize = h * 3 / 100;
			style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
			string text = "Enemy Type: " + (EnemiesTypesLevel + 1) + "\n" +
						  "Enemy Spawn: " + (EnemiesSpawnLevel + 1) + "\n" +
						  "Enemy Attr.: " + (EnemiesAttributesLevel + 1) + "\n" +
						  "Tier: " + (TierLevel + 1) + "\n" +
						  "Item: " + (ItemLevel + 1);
			GUI.Label(rect, text, style);
		}
		#endif 
	}
}

[System.Serializable]
public class LevelUpCondition
{
	public int points;
	public int killStreak;
}

[System.Serializable]
public class PlayerLevelUpCondition
{
	public int killStreak;
	public int maxRays;
	public Color color;
}

[System.Serializable]
public class EnemiesTypesLevelUpCondition : LevelUpCondition
{
	public List<EnemiesPercent> enemies;
}

[System.Serializable]
public class EnemiesPercent
{
	public GameObject enemy;
	public float percent;
}

[System.Serializable]
public class EnemiesSpawnLevelUpCondition : LevelUpCondition
{
	public float time;
	public int quantity;
}

[System.Serializable]
public class EnemiesAttributesLevelUpCondition
{
	public int points;
	public float vel;
	public float life;
}

[System.Serializable]
public class BossLevelUpCondition
{
	public int kills;
	[Range(0, 1f)]
	public float bossMeteor;
	[Range(0, 1f)]
	public float bossTwins;
	[Range(0, 1f)]
	public float bossIllusion;
}

[System.Serializable]
public class ItemLevelUpCondition : LevelUpCondition
{
	public float chanceToSpawn;
	public List<ItemPercent> itens;
}

[System.Serializable]
public class ItemPercent
{
	public GameObject item;
	public float percent;
}

[System.Serializable]
public class LevelFloatList
{
	public float value;
}