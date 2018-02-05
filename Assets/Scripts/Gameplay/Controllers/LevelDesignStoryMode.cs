using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class LevelDesignStoryMode : MonoBehaviour 
{
	#region singleton
	private static LevelDesignStoryMode instance;
	public static LevelDesignStoryMode Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<LevelDesignStoryMode>();

			return instance;
		}
	}
	#endregion


	public static List<EnemyWave> GetWave(int world, int level, int wave)
	{
		return Instance.balance.worldBalance[world - 1].levels[level - 1].waves[wave - 1].enemies;
	}

	public StoryModeBalance balance;

	// Use this for initialization
	void Start () 
	{
		balance = StoryModeBalance.LoadFromText(((TextAsset) Resources.Load("BalanceStoryMode")).text);
	}
}

[System.Serializable, XmlRoot("balance")]
public class StoryModeBalance
{
	[XmlArray("tiers"), XmlArrayItem("tier"), HideInInspector]
	public WorldsSpawnBalanceXML[] worldsBalanceXML;

	public List<World> worldBalance = new List<World>();

	public static StoryModeBalance LoadFromText(string text)
	{
		var serializer = new XmlSerializer(typeof(StoryModeBalance));

		StoryModeBalance _gameBalance = serializer.Deserialize(new StringReader(text)) as StoryModeBalance;
		_gameBalance.ArrangeArray();

		return _gameBalance;
	}

	public void ArrangeArray()
	{
		World lastWorldCreated = null;
		Level lastLevelCreated = null;
		Wave lastWaveCreated = null;
		for(byte i = 0; i < worldsBalanceXML.Length; i++)
		{
			WorldsSpawnBalanceXML balanceXML = worldsBalanceXML[i];
			Debug.Log(balanceXML.ToString());

			if(balanceXML.world >= 0)
			{
				lastWorldCreated = new World();
				lastWorldCreated.number = balanceXML.world;

				worldBalance.Add(lastWorldCreated);
			}

			if(balanceXML.level >= 0)
			{
				lastLevelCreated = new Level();
				lastLevelCreated.number = balanceXML.level;

				lastWorldCreated.levels.Add(lastLevelCreated);
			}

			if(balanceXML.wave >= 0)
			{
				lastWaveCreated = new Wave();
				lastWaveCreated.number = balanceXML.wave;

				lastLevelCreated.waves.Add(lastWaveCreated);
			}

			EnemyWave enemyWave = new EnemyWave();
			enemyWave.enemy = balanceXML.enemy;
			enemyWave.timeToSpawn = balanceXML.time;

			lastWaveCreated.enemies.Add(enemyWave);
		}
	}
}

[System.Serializable]
public class WorldsSpawnBalanceXML
{
	public int world = -1;
	public int level = -1;
	public int wave = -1;
	public EnemiesPercent.EnemyNames enemy;
	public float time;

	public string ToString()
	{
		return string.Format("world: {0} \n level: {1} \n wave: {2} \n enemy: {3} \n time: {4}", world, level, wave, enemy, time);
	}
}

[System.Serializable]
public class World
{
	public int number;
	public List<Level> levels = new List<Level>();
}

[System.Serializable]
public class Level
{
	public int number;
	public List<Wave> waves = new List<Wave>();
}

[System.Serializable]
public class Wave
{
	public int number;
	public List<EnemyWave> enemies = new List<EnemyWave>();
}

[System.Serializable]
public class EnemyWave
{
	public EnemiesPercent.EnemyNames enemy;
	public float timeToSpawn;
}