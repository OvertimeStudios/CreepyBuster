using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

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

	[Header("Enemies Game Object")]
	public GameObject blu;
	public GameObject spiral;
	public GameObject ziggy;
	public GameObject charger;
	public GameObject legion;
	public GameObject follower;

	[Header("Bosses")]
	public GameObject bossMeteor;
	public GameObject bossTwins;
	public GameObject bossIllusion;

	[Header("Item")]
	public GameObject plasmaOrbPP;
	public GameObject plasmaOrbP;
	public GameObject plasmaOrbM;
	public GameObject plasmaOrbG;
	public GameObject deathRay;
	public GameObject invencibility;
	public GameObject frozen;
	public GameObject levelUp;

	[Header("Backgrounds")]
	public List<GameObject> backgrounds;

	[Header("Balance - Via XML")]
	public GameBalance gameBalance;

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
				return Instance.gameBalance.playerLevelUpCondition [LevelDesign.PlayerLevel + 1].killStreak - Instance.gameBalance.playerLevelUpCondition [LevelDesign.PlayerLevel].killStreak;
			}

			return Instance.gameBalance.specialAttributes.streak;
		}
	}

	/// <summary>
	/// Gets the streak necessary to unlock current level. (i.e. level1 = 15, level2 = 30, level3 = 45)
	/// </summary>
	public static int CurrentPlayerLevelUnlockStreak
	{
		get 
		{
			return Instance.gameBalance.playerLevelUpCondition [LevelDesign.PlayerLevel].killStreak + (Instance.gameBalance.specialAttributes.streak * GameController.specialStreak);
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
				return Instance.gameBalance.playerLevelUpCondition[Instance.gameBalance.playerLevelUpCondition.Length - 1].killStreak + (Instance.gameBalance.specialAttributes.streak * (GameController.specialStreak + 1));
			}

			return Instance.gameBalance.playerLevelUpCondition[LevelDesign.PlayerLevel + 1].killStreak ;
		}
	}

	public static int LastLevelPlayerStreak
	{
		get 
		{
			if(LevelDesign.PlayerLevel == 0)
				return 0;

			return Instance.gameBalance.playerLevelUpCondition[LevelDesign.PlayerLevel - 1].killStreak ;
		}
	}

	public static int MaxPlayerLevel
	{
		get { return Global.RayLevel + 1; }
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
		get { return Instance.gameBalance.enemiesTypesLevelUpCondition.Length - 1; }
	}

	/// <summary>
	/// Gets the next streak to enemy types level up.
	/// </summary>
	public static int NextStreakToEnemyTypesLevelUp
	{
		get 
		{
			if(!LevelDesign.IsEnemyTypesMaxLevel)
				return Instance.gameBalance.enemiesTypesLevelUpCondition[enemiesTypesLevel + 1].killStreak;
			
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
				return Instance.gameBalance.enemiesTypesLevelUpCondition[enemiesTypesLevel + 1].points;
			
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
		get { return Instance.gameBalance.enemiesSpawnLevelUpCondition.Length - 1; }
	}
	
	/// <summary>
	/// Gets the next streak to enemy spawn level up.
	/// </summary>
	public static int NextStreakToEnemySpawnLevelUp
	{
		get 
		{
			if(!LevelDesign.IsEnemySpawnMaxLevel)
				return Instance.gameBalance.enemiesSpawnLevelUpCondition[enemiesSpawnLevel + 1].killStreak;
			
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
				return Instance.gameBalance.enemiesSpawnLevelUpCondition[enemiesSpawnLevel + 1].points;
			
			return 0;
		}
	}
	#endregion

	#region enemies attributes
	/// <summary>
	/// Gets the next streak to enemy attributes level up.
	/// </summary>
	public static int NextScoreToEnemyAttributesLevelUp {
		get { return Instance.gameBalance.enemiesAttributesLevelUpCondition.points * (enemiesAttributeLevel + 1); }
	}

	#endregion

	#region Boss
	public static int NextKillToBossBattle
	{
		get 
		{
			if(bossLevel < Instance.gameBalance.bossLevelUpCondition.tiers.Length)
				return Instance.gameBalance.bossLevelUpCondition.tiers[bossLevel].kills;
			else
				return Instance.gameBalance.bossLevelUpCondition.tiers[Instance.gameBalance.bossLevelUpCondition.tiers.Length - 1].kills + ((bossLevel - (Instance.gameBalance.bossLevelUpCondition.tiers.Length - 1)) * Instance.gameBalance.bossLevelUpCondition.infinityTier.kills);
		}
	}

	public static GameObject CurrentBoss
	{
		get	
		{ 
			#if UNITY_WEBPLAYER
			return Instance.bossMeteor;
			#else

			BossTier currentLevelUpCondition = (bossLevel < Instance.gameBalance.bossLevelUpCondition.tiers.Length) ? Instance.gameBalance.bossLevelUpCondition.tiers[bossLevel] : Instance.gameBalance.bossLevelUpCondition.infinityTier;

			float maxPercent = currentLevelUpCondition.bossIllusion + currentLevelUpCondition.bossMeteor + currentLevelUpCondition.bossTwins;

			float rnd = UnityEngine.Random.Range(0f, maxPercent);

			if(rnd < currentLevelUpCondition.bossMeteor)
				return Instance.bossMeteor;
			else if(rnd < currentLevelUpCondition.bossTwins + currentLevelUpCondition.bossMeteor)
				return Instance.bossTwins;
			else
				return Instance.bossIllusion;
			#endif
			
		}
	}


	#endregion

	#region Item
	public static float ItemSpawnTime
	{
		get { return Instance.gameBalance.itemLevelUpCondition.spawnTime.Random (); }
	}

	/// <summary>
	/// The probability of spawning an item
	/// </summary>
	public static float SpawnItemPercent
	{
		get { return Instance.gameBalance.itemLevelUpCondition.itemTier[itemLevel].chanceToSpawn; }
	}

	public static List<ItemPercent> CurrentItens
	{
		get { return Instance.gameBalance.itemLevelUpCondition.itemTier[itemLevel].itens; }
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
		get { return Instance.gameBalance.itemLevelUpCondition.itemTier.Length - 1; }
	}
	
	/// <summary>
	/// Gets the next streak to item level up.
	/// </summary>
	public static int NextStreakToItemLevelUp
	{
		get 
		{
			if(!LevelDesign.IsItemMaxLevel)
				return Instance.gameBalance.itemLevelUpCondition.itemTier[itemLevel + 1].killStreak;
			
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
				return Instance.gameBalance.itemLevelUpCondition.itemTier[itemLevel + 1].points;
			
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

			return Instance.gameBalance.shopItens.damage[up];
		}
	}

	public static float CurrentRange
	{
		get
		{
			int up = 0;

			for(byte i = 0; i < Global.RangeLevel; i++)
				up++;
			
			return Instance.gameBalance.shopItens.range[up];
		}
	}

	#endregion

	#endregion

	public static List<EnemiesPercent> CurrentEnemies
	{
		get { return Instance.gameBalance.enemiesTypesLevelUpCondition[LevelDesign.EnemiesTypesLevel].enemies; }
	}

	public static float EnemiesSpawnTime
	{
		get { return Instance.gameBalance.enemiesSpawnLevelUpCondition[LevelDesign.EnemiesSpawnLevel].time; }
	}
	
	public static float EnemiesBonusLife
	{
		get { return Instance.gameBalance.enemiesAttributesLevelUpCondition.life * enemiesAttributeLevel; }
	}

	public static float EnemiesBonusVel
	{
		get { return Instance.gameBalance.enemiesAttributesLevelUpCondition.vel * enemiesAttributeLevel; }
	}

	public static int SpawnQuantity
	{
		get { return Instance.gameBalance.enemiesSpawnLevelUpCondition[LevelDesign.EnemiesSpawnLevel].quantity; }
	}

	public static int MaxRays
	{
		get { return Instance.gameBalance.playerLevelUpCondition[LevelDesign.PlayerLevel].maxRays + ((AttackTargets.IsSpecialActive) ? Instance.gameBalance.specialAttributes.bonusTarget : 0); }
	}

	public static Color CurrentColor
	{
		get 
		{
			if(AttackTargets.IsSpecialActive)
				return Instance.gameBalance.specialAttributes.color;
			else
				return Instance.gameBalance.playerLevelUpCondition [LevelDesign.PlayerLevel].color; 
		}
	}

	public static bool IsSpecialReady
	{
		get { return LevelDesign.PlayerLevel >= Instance.gameBalance.playerLevelUpCondition.Length - 1 && GameController.StreakCount >= NextStreakToPlayerLevelUp; }
	}

	public static bool IsPlayerFullyUpgraded
	{
		get { return Global.RayLevel + 2 == Instance.gameBalance.playerLevelUpCondition.Length; }
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
		GameController.OnRealStreakUpdated -= ItensLevelUp;
		GameController.OnKill -= KillCountUpdated;

		GameController.OnGameStart -= Reset;

		BossLife.OnBossDied -= BossLevelUp;
	}

	void Start()
	{
		//Load xml which contains all balance
		gameBalance = GameBalance.LoadFromText(((TextAsset) Resources.Load("Balance")).text);
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

				#if UNITYANALYTICS_IMPLEMENTED
				UnityAnalyticsHelper.OnLevelUp(enemiesTypesLevel);
				#endif

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
		if(enemiesAttributeLevel < gameBalance.enemiesAttributesLevelUpCondition.maxEnemiesAttributeLevel && GameController.Score >= LevelDesign.NextScoreToEnemyAttributesLevelUp)
		{
			enemiesAttributeLevel++;
			
			if(OnEnemiesAttributesLevelUp != null)
				OnEnemiesAttributesLevelUp();
		}
	}

	/*private void TierLevelUp()
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
	}*/

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

	public static void BossLevelUp(GameObject boss)
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
	public Color color { get { return GetColorFromHex(colorhex); } }

	//[HideInInspector]
	public string colorhex;

	private Color GetColorFromHex(string hex)
	{
		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte a = 255;//assume fully visible unless specified in hex
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		//Only use alpha if the string has enough characters
		if(hex.Length == 8){
			a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r,g,b,a);
	}
}

[System.Serializable, XmlRoot("balance")]
public class GameBalance
{
	[XmlArray("player"), XmlArrayItem("levelUpCondition")]
	public PlayerLevelUpCondition[] playerLevelUpCondition;

	[XmlElement("special")]
	public SpecialAttributes specialAttributes;

	[XmlArray("enemiesTypes"), XmlArrayItem("levelUpCondition")]
	public EnemiesTypesLevelUpCondition[] enemiesTypesLevelUpCondition;

	[XmlArray("enemiesSpawn"), XmlArrayItem("levelUpCondition")]
	public EnemiesSpawnLevelUpCondition[] enemiesSpawnLevelUpCondition;

	[XmlElement("enemiesAttributes")]
	public EnemiesAttributesLevelUpCondition enemiesAttributesLevelUpCondition;

	[XmlElement("boss")]
	public BossLevelUpCondition bossLevelUpCondition;

	[XmlElement("itens")]
	public ItemLevelUpCondition itemLevelUpCondition;

	[XmlElement("shop")]
	public ShopItens shopItens;

	public static GameBalance Load(string path)
	{
		var serializer = new XmlSerializer(typeof(GameBalance));
		using(var stream = new FileStream(path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as GameBalance;
		}
	}

	public static GameBalance LoadFromText(string text)
	{
		var serializer = new XmlSerializer(typeof(GameBalance));

		Debug.Log(text);
		return serializer.Deserialize(new StringReader(text)) as GameBalance;
	}
}

[System.Serializable]
public class SpecialAttributes
{
	public int streak = 20;
	public float duration = 5f;
	public float bonusDamage = 1f;
	public int bonusTarget = 1;
	public Color color { get { return GetColorFromHex(colorhex); } }

	//[HideInInspector]
	public string colorhex;

	private Color GetColorFromHex(string hex)
	{
		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte a = 255;//assume fully visible unless specified in hex
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		//Only use alpha if the string has enough characters
		if(hex.Length == 8){
			a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r,g,b,a);
	}
}

[System.Serializable]
public class EnemiesTypesLevelUpCondition : LevelUpCondition
{
	[XmlArray("EnemiesPercents"), XmlArrayItem("EnemiesPercent")]
	public List<EnemiesPercent> enemies = new List<EnemiesPercent>();
}

[System.Serializable]
public class EnemiesPercent
{
	public enum EnemyNames { Blu, Charger, Ziggy, Spiral, Follower, Legion }

	public GameObject enemy
	{
		get 
		{
			GameObject obj = null;

			switch(name)
			{
				case EnemyNames.Blu:
					obj = LevelDesign.Instance.blu;
					break;

				case EnemyNames.Ziggy:
					obj = LevelDesign.Instance.ziggy;
					break;

				case EnemyNames.Charger:
					obj = LevelDesign.Instance.charger;
					break;

				case EnemyNames.Spiral:
					obj = LevelDesign.Instance.spiral;
					break;

				case EnemyNames.Legion:
					obj = LevelDesign.Instance.legion;
					break;

				case EnemyNames.Follower:
					obj = LevelDesign.Instance.follower;
					break;
			}

			return obj;
		}
	}

	public EnemyNames name;
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
	public int maxEnemiesAttributeLevel;
}

[System.Serializable]
public class BossLevelUpCondition
{
	[XmlArray("tiers"), XmlArrayItem("tier")]
	public BossTier[] tiers;

	[XmlElement("infinityBoss")]
	public BossTier infinityTier;
}

[System.Serializable]
public class BossTier
{
	public int kills;
	public float bossMeteor;
	public float bossTwins;
	public float bossIllusion;
}

[System.Serializable]
public class ItemLevelUpCondition
{
	public float spawnTimeMin;
	public float spawnTimeMax;

	public RandomBetweenTwoConst spawnTime
	{
		get { return new RandomBetweenTwoConst(spawnTimeMin, spawnTimeMax); }
	}

	[XmlArray("tiers"), XmlArrayItem("tier")]
	public ItemTier[] itemTier;
}

[System.Serializable]
public class ItemTier : LevelUpCondition
{
	public float chanceToSpawn;

	[XmlArray("itensPercent"), XmlArrayItem("itemPercent")]
	public List<ItemPercent> itens;
}

[System.Serializable]
public class ItemPercent
{
	public enum ItensNames { PlasmaOrbPP, PlasmaOrbP, PlasmaOrbM, PlasmaOrbG, DeathRay, Frozen, Invencibility, LevelUp }

	public GameObject item
	{
		get
		{
			GameObject obj = null;

			switch(name)
			{
				case ItensNames.PlasmaOrbPP:
					obj = LevelDesign.Instance.plasmaOrbPP;
					break;

				case ItensNames.PlasmaOrbP:
					obj = LevelDesign.Instance.plasmaOrbP;
					break;

				case ItensNames.PlasmaOrbM:
					obj = LevelDesign.Instance.plasmaOrbM;
					break;

				case ItensNames.PlasmaOrbG:
					obj = LevelDesign.Instance.plasmaOrbG;
					break;

				case ItensNames.Frozen:
					obj = LevelDesign.Instance.frozen;
					break;

				case ItensNames.DeathRay:
					obj = LevelDesign.Instance.deathRay;
					break;

				case ItensNames.Invencibility:
					obj = LevelDesign.Instance.invencibility;
					break;

				case ItensNames.LevelUp:
					obj = LevelDesign.Instance.levelUp;
					break;
			}

			return obj;
		}
	}

	public ItensNames name;
	public float percent;
}

[System.Serializable]
public class LevelFloatList
{
	public float value;
}

[System.Serializable]
public class ShopItens
{
	[XmlArray("range"), XmlArrayItem("value")]	
	public List<float> range;

	[XmlArray("damage"), XmlArrayItem("value")]
	public List<float> damage;
}