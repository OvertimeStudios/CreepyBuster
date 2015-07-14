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
	#endregion

	[Header("Player Level")]
	public PlayerLevelUpCondition[] playerLevelUpCondition;
	public int streakToSpecial;
	public float specialTime = 5f;

	[Header("Enemies Level")]
	public EnemiesTypesLevelUpCondition[] enemiesTypesLevelUpCondition;

	public EnemiesSpawnLevelUpCondition[] enemiesSpawnLevelUpCondition;

	public EnemiesAttributesLevelUpCondition enemiesAttributesLevelUpCondition;
	
	[Header("Tier Level")]
	public LevelUpCondition[] tierLevelUpCondition;

	[Header("Boss Battle")]
	public BossLevelUpCondition bossBattleCondition;

	[Header("Backgrounds")]
	public List<GameObject> backgrounds;

	[Header("Item")]
	public RandomBetweenTwoConst itemSpawnTime;
	public ItemLevelUpCondition[] itensLevelUpCondition;
	
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

	public static int MaxPlayerLevel
	{
		get { return Instance.playerLevelUpCondition.Length - 1; }
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

	#region Item
	public static float ItemSpawnTime
	{
		get { return Instance.itemSpawnTime.Random (); }
	}

	public static float SpawnItemPercent
	{
		get { return Instance.itensLevelUpCondition[itemLevel].chanceToSpawn; }
	}

	public static List<ItemPercent> CurrentItens
	{
		get { return Instance.itensLevelUpCondition[itemLevel].itens; }
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
		get { return Instance.playerLevelUpCondition[LevelDesign.PlayerLevel].maxRays; }
	}

	public static Color CurrentColor
	{
		get { return Instance.playerLevelUpCondition [LevelDesign.PlayerLevel].color; }
	}

	public static bool IsSpecialReady
	{
		get { return LevelDesign.PlayerLevel >= Instance.playerLevelUpCondition.Length - 1 && GameController.StreakCount >= NextStreakToPlayerLevelUp; }
	}

	public static int PlayerLevel
	{
		get { return playerLevel; }
		set { playerLevel = value; }
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
		GameController.OnStreakUpdated += EnemiesTypesLevelUp;
		GameController.OnStreakUpdated += EnemiesSpawnLevelUp;
		GameController.OnStreakUpdated += EnemiesAttributesLevelUp;
		GameController.OnStreakUpdated += TierLevelUp;

		GameController.OnGameStart += Reset;
	}

	void OnDisable()
	{
		GameController.OnStreakUpdated -= PlayerLevelUp;
		GameController.OnStreakUpdated -= EnemiesTypesLevelUp;
		GameController.OnStreakUpdated -= EnemiesSpawnLevelUp;
		GameController.OnStreakUpdated -= EnemiesAttributesLevelUp;
		GameController.OnStreakUpdated -= TierLevelUp;

		GameController.OnGameStart -= Reset;
	}

	private void PlayerLevelUp()
	{
		if(GameController.StreakCount >= LevelDesign.NextStreakToPlayerLevelUp)
		{
			playerLevel = Mathf.Clamp(playerLevel + 1, 0, MaxPlayerLevel);

			if(OnPlayerLevelUp != null)
				OnPlayerLevelUp();
		}
	}

	private void EnemiesTypesLevelUp()
	{
		if(!IsEnemyTypesMaxLevel)
		{
			if(GameController.Score >= LevelDesign.NextScoreToEnemyTypesLevelUp ||
			   GameController.StreakCount >= LevelDesign.NextStreakToEnemyTypesLevelUp)
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
			   GameController.StreakCount >= LevelDesign.NextStreakToEnemySpawnLevelUp)
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
			   GameController.StreakCount >= LevelDesign.NextStreakToTierLevelUp)
			{
				tierLevel++;

				if(OnTierLevelUp != null)
					OnTierLevelUp();
			}
		}
	}

	private void Reset()
	{
		playerLevel = 0;
		enemiesSpawnLevel = 0;
		enemiesTypesLevel = 0;
		enemiesAttributeLevel = 0;
		tierLevel = 0;
		bossLevel = 0;
	}

	private void OnLoseStacks()
	{
		playerLevel = 0;
	}

	void OnGUI()
	{
		#if SHOW_GUI
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();
		
		Rect rect = new Rect(0, h * 10 / 100, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 3 / 100;
		style.normal.textColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
		string text = "Enemy Type: " + (EnemiesTypesLevel + 1) + "\n" +
					  "Enemy Spawn: " + (EnemiesSpawnLevel + 1) + "\n" +
					  "Enemy Attr.: " + (EnemiesAttributesLevel + 1) + "\n" +
					  "Tier: " + (TierLevel + 1);
		GUI.Label(rect, text, style);
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
public class BossLevelUpCondition : LevelUpCondition
{
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