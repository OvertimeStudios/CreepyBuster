using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnController : MonoBehaviour 
{
	public static event Action OnSpawn;
	public static event Action NoMoreEnemies;

	public static List<Transform> enemiesInGame;
	public static List<Transform> itensInGame;

	public List<GameObject> orbs;

	//viewport coordinates outside of screen
	private const float bottom = -0.2f;
	private const float left = -0.2f;
	private const float up = 1.2f;
	private const float right = 1.2f;

	private static float lastPosX = 0;
	private static float lastPosY = 0;

	private static GameObject boss;
	private static GameObject background;

	[Header("Only for developer")]
	public GameObject stars;
	public TweenAlpha starsFade;

	#region singleton
	private static SpawnController instance;

	public static SpawnController Instance
	{
		get 
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<SpawnController>();

			return instance;
		}
	}
	#endregion

	#region get / set
	private static bool CanSpawn
	{
		get 
		{
			return !TutorialController.IsRunning && !GameController.IsFrozen && GameController.isGameRunning && !GameController.IsBossTime && !ConsumablesController.IsUsingConsumables;
		}
	}

	public static int EnemiesInGame
	{
		get { return enemiesInGame.Count; }
	}
	#endregion

	void OnEnable()
	{
		MenuController.OnPanelOpening += SpawnBackground;
		MenuController.OnPanelClosing += DestroyBackground;
		MenuController.OnPanelClosed += Reset;
		GameController.OnGameStart += StartSpawn;
		GameController.OnGameOver += GameOver;
		EnemyMovement.OnOutOfScreen += RemoveEnemy;
		EnemyLife.OnDied += RemoveEnemy;
		Legion.OnMinionSpawned += AddEnemy;	
        BossLife.OnBossDied += BossDied;
	}

	void OnDisable()
	{
		MenuController.OnPanelOpening -= SpawnBackground;
		MenuController.OnPanelClosing -= DestroyBackground;
		MenuController.OnPanelClosed -= Reset;
		GameController.OnGameStart -= StartSpawn;
		GameController.OnGameOver -= GameOver;
		EnemyMovement.OnOutOfScreen -= RemoveEnemy;
		EnemyLife.OnDied -= RemoveEnemy;
		Legion.OnMinionSpawned -= AddEnemy;	
        BossLife.OnBossDied -= BossDied;
	}

	// Use this for initialization
	void Start () 
	{
		enemiesInGame = new List<Transform> ();
		itensInGame = new List<Transform> ();
	}

	public void StartSpawn()
	{
		if(GameController.gameMode == GameController.GameMode.Endless)
		{
			StartCoroutine ("SpawnEnemies", LevelDesign.EnemiesSpawnTime);
			StartCoroutine ("SpawnItens", LevelDesign.ItemSpawnTime);
		}
		else if(GameController.gameMode == GameController.GameMode.Story)
		{
			StartCoroutine("SpawnEnemiesStory");
		}
	}

	public void StopSpawn()
	{
		StopCoroutine ("SpawnEnemies");
		StopCoroutine ("SpawnItens");

		StopCoroutine("SpawnEnemiesStory");
	}

	private IEnumerator SpawnEnemiesStory()
	{
		List<EnemyWave> enemiesToSpawn = LevelDesignStoryMode.GetWave(GameController.world, GameController.level, GameController.wave);

        float totalSpawnTime = 0;
        float timeToNextSpawn = 0;
		while(enemiesToSpawn.Count > 0)
		{
            timeToNextSpawn = enemiesToSpawn[0].timeToSpawn - timeToNextSpawn;
            totalSpawnTime += timeToNextSpawn;

            yield return new WaitForSeconds(timeToNextSpawn);

			for(int i = 0; i < enemiesToSpawn.Count; i++)
			{
				EnemyWave enemyWave = enemiesToSpawn[i];
					
                //next group of enemies to spawn
                if(enemyWave.timeToSpawn - totalSpawnTime > 0)
				{
					break;
				}
				
				SpawnEnemy(GetEnemyObject(enemyWave.enemy));

                enemiesToSpawn.RemoveAt(i);

				i--;
			}
		}

		while(SpawnController.EnemiesInGame > 0)
			yield return null;

        bool isFinalWave = LevelDesignStoryMode.IsFinalWave(GameController.world, GameController.level, GameController.wave);
        bool isFinalLevel = LevelDesignStoryMode.IsFinalLevel(GameController.world, GameController.level);
        if(!isFinalWave)
        {
			GameController.wave++;
        }
        else if(!isFinalLevel)
        {
            Global.SetWorldLevelCompletion(GameController.world, GameController.level);

            GameController.wave = 0;
            GameController.level++;

        }

        if(isFinalWave && isFinalLevel)
        {
            Global.SetWorldLevelCompletion(GameController.world, GameController.level);
            Debug.Log("Spawn Boss!");
            SpawnBoss();
        }
        else
        {
            StartCoroutine("SpawnEnemiesStory");
        }
	}

    private void BossDied(GameObject gameObject)
    {
        if(GameController.gameMode == GameController.GameMode.Story)
        {
            Global.SetWorldLevelCompletion(GameController.world, 5);
        }
    }

	private GameObject GetEnemyObject(EnemiesPercent.EnemyNames enemyName)
	{
		GameObject obj = null;

		switch(enemyName)
		{
		case EnemiesPercent.EnemyNames.Blu:
			obj = LevelDesign.Instance.blu;
			break;

		case EnemiesPercent.EnemyNames.Ziggy:
			obj = LevelDesign.Instance.ziggy;
			break;

		case EnemiesPercent.EnemyNames.Charger:
			obj = LevelDesign.Instance.charger;
			break;

		case EnemiesPercent.EnemyNames.Spiral:
			obj = LevelDesign.Instance.spiral;
			break;

		case EnemiesPercent.EnemyNames.Legion:
			obj = LevelDesign.Instance.legion;
			break;

		case EnemiesPercent.EnemyNames.Follower:
			obj = LevelDesign.Instance.follower;
			break;
		}

		return obj;
	}
	
	private IEnumerator SpawnEnemies(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		SpawnEnemies ();
	}

	public static void SpawnBoss()
	{
		Vector3 pos = new Vector3 (0.5f, 1.3f, 0);
		pos = Camera.main.ViewportToWorldPoint (pos);
		pos.z = 0f;

        boss = Instantiate (LevelDesign.CurrentBoss, pos, Quaternion.identity) as GameObject; 

        SoundController.Instance.PlayMusic(SoundController.Musics.BossTheme);
	}

	public static void SpawnEnemy(GameObject enemy)
	{
		Vector3 pos = GetSpawnPosition();
		float rot = GetRotation(pos, enemy.name);

		GameObject e = Instantiate (enemy, pos, Quaternion.Euler(0, 0, rot)) as GameObject;
		
		AddEnemy(e);
		
		if(OnSpawn != null)
			OnSpawn();
	}

	private void SpawnEnemies()
	{
		if(CanSpawn)
		{
			//spawn some monsters according to LevelDesign (can be more than 1)
			for(byte i = 0; i < LevelDesign.SpawnQuantity; i++)
			{
				List<EnemiesPercent> monsters = LevelDesign.CurrentEnemies;

				//max %
				float maxPercent = 0f;
				foreach(EnemiesPercent ep in monsters)
					maxPercent += ep.percent;

				float rnd = UnityEngine.Random.Range(0f, maxPercent);

				float currentPercent = 0f;
				GameObject objToSpawn =  null;

				foreach(EnemiesPercent ep in monsters)
				{
					currentPercent += ep.percent;

					if(rnd <= currentPercent)
					{
						objToSpawn = ep.enemy;
						break;
					}
				}
				 
				SpawnEnemy(objToSpawn);
			}
		}

		StartCoroutine ("SpawnEnemies", LevelDesign.EnemiesSpawnTime);
	}

	public static Vector3 GetSpawnPosition()
	{
		float posX = 0f;
		float posY = 0f;

		#region Tier 1
		if(LevelDesign.TierLevel == 0)
		{
			//always on top
			posY = up;

			//random x
			do
			{
				posX = UnityEngine.Random.Range(0.1f, 0.9f);
			}
			while(Mathf.Abs(posX - lastPosX) < 0.1f);

			lastPosX = posX;

		}
		#endregion

		#region Tier 2
		if(LevelDesign.TierLevel == 1)
		{
			float rnd = UnityEngine.Random.Range (0f, 1f);

			if(rnd < 0.33f)//UP
			{
				posY = up;

				//random pos x
				do
				{
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);
				
				lastPosX = posX;
			}
			else if(rnd < 0.66f)//RIGHT
			{
				posX = right;

				//random only top 1/3 of pos Y
				do
				{
					posY = UnityEngine.Random.Range(0.7f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);
				
				lastPosY = posY;
			}
			else//LEFT
			{
				posX = left;

				//random only top 1/3 of pos Y
				do
				{
					posY = UnityEngine.Random.Range(0.7f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);
				
				lastPosY = posY;
			}
		}
		#endregion

		#region Tier 3
		if(LevelDesign.TierLevel == 2)
		{
			float rnd = UnityEngine.Random.Range (0f, 1f);
			
			if(rnd < 0.25f)//UP
			{
				posY = up;
				
				//random pos x
				do
				{
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posX - lastPosX) < 0.1f);
				
				lastPosX = posX;
			}
			else if(rnd < 0.5f)//RIGHT
			{
				posX = right;
				
				//random only top 1/2 of pos Y
				do
				{
					posY = UnityEngine.Random.Range(0.5f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);
				
				lastPosY = posY;
			}
			else if(rnd < 0.75f)//LEFT
			{
				posX = left;
				
				//random only top 1/2 of pos Y
				do
				{
					posY = UnityEngine.Random.Range(0.5f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);

				lastPosY = posY;
			}
			else//BOTTOM
			{
				posY = bottom;

				//random only center 1/2 of pos X
				do
				{
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posX - lastPosX) < 0.3f);
				
				lastPosX = posX;
			}
		}
		#endregion

		#region Tier 4
		if(LevelDesign.TierLevel == 3)
		{
			float rnd = UnityEngine.Random.Range (0f, 1f);
			
			if(rnd < 0.25f)//UP
			{
				posY = up;
				
				//random pos x
				do
				{
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posX - lastPosX) < 0.3f);
				
				lastPosX = posX;
			}
			else if(rnd < 0.5f)//RIGHT
			{
				posX = right;
				
				//random  pos Y
				do
				{
					posY = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);
				
				lastPosY = posY;
			}
			else if(rnd < 0.75f)//LEFT
			{
				posX = left;
				
				//random  pos Y
				do
				{
					posY = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posY - lastPosY) < 0.1f);

				lastPosY = posY;
			}
			else//BOTTOM
			{
				posY = bottom;
				
				//random pos x
				do
				{
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				while(Mathf.Abs(posX - lastPosX) < 0.3f);
				
				lastPosX = posX;
			}
		}
		#endregion
		
		Vector3 pos = Camera.main.ViewportToWorldPoint (new Vector3 (posX, posY, 0));
		pos.z = 0;

		return pos;
	}

	public static float GetRotation(Vector3 pos, string name)
	{
		Vector3 viewportPosition = Camera.main.WorldToViewportPoint (pos);

		float rot = 0f;

		if(!CanDiagonal(name))
		{
			if (viewportPosition.x < 0)//LEFT
				rot = 0f;
			if (viewportPosition.x > 1)//RIGHT
				rot = 180f;
			if (viewportPosition.y < 0)//DOWN
				rot = 90f;
			if(viewportPosition.y > 1)//UP
				rot = -90f;
		}
		else
		{
			Vector3 othersidePosition = Vector3.zero;

			if(viewportPosition.x < 0)
				othersidePosition = new Vector3(1, UnityEngine.Random.Range(0.1f, 0.9f), 0);
			if(viewportPosition.x > 1)
				othersidePosition = new Vector3(0, UnityEngine.Random.Range(0.1f, 0.9f), 0);
			if(viewportPosition.y < 0)
				othersidePosition = new Vector3(UnityEngine.Random.Range(0.1f, 0.9f), 1, 0);
			if(viewportPosition.y > 1)
				othersidePosition = new Vector3(UnityEngine.Random.Range(0.1f, 0.9f), 0, 0);

			rot = Mathf.Atan2(othersidePosition.y - viewportPosition.y, othersidePosition.x - viewportPosition.x) * Mathf.Rad2Deg;
		}

		return rot;
	}

	private static bool CanDiagonal(string objName)
	{
		if(objName.Contains("C1"))
			return true;

		return false;
	}

	public void SpawnBackground()
	{
		Debug.Log("SpawnBackground");
		//TODO: Spawn according to players choice

		StartCoroutine(FadeOutStars());
	}

	private IEnumerator FadeOutStars()
	{
		starsFade.ResetToBeginning();
		starsFade.from = 0;
		starsFade.to = 1;
		starsFade.PlayForward();

		yield return new WaitForSeconds(starsFade.duration);

		stars.SetActive(false);

		int rnd = (int)UnityEngine.Random.Range (0, LevelDesign.Instance.backgrounds.Count);
		
		background = Instantiate (LevelDesign.Instance.backgrounds [rnd]) as GameObject;

		yield return null;
	}

	public void DestroyBackground()
	{
		StartCoroutine(FadeInStars());
	}

	private IEnumerator FadeInStars()
	{
		TweenAlpha bgTweenAlpha = background.GetComponentInChildren<TweenAlpha>();

		bgTweenAlpha.ResetToBeginning();
		bgTweenAlpha.from = 0;
		bgTweenAlpha.to = 1;
		bgTweenAlpha.PlayForward();
		
		yield return new WaitForSeconds(bgTweenAlpha.duration);

		starsFade.ResetToBeginning();
		starsFade.from = 1;
		starsFade.to = 0;
		starsFade.PlayForward();

		stars.SetActive(true);

		Destroy(background);
		background = null;
	}

	private IEnumerator SpawnItens(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		
		SpawnItens ();
	}

	private void SpawnItens()
	{
		SpawnItens(false);
	}

	private void SpawnItens(bool stationary)
	{
		if(CanSpawn)
		{
			float rnd = UnityEngine.Random.Range (0f, 1f);

			if(rnd < LevelDesign.SpawnItemPercent)
			{
				//max %
				float maxPercent = 0f;
				foreach(ItemPercent ip in LevelDesign.CurrentItens)
					maxPercent += ip.percent;

				rnd = UnityEngine.Random.Range (0f, maxPercent);

				float currentPercent = 0f;
				GameObject objToSpawn =  null;
				
				foreach(ItemPercent ip in LevelDesign.CurrentItens)
				{
					currentPercent += ip.percent;
					
					if(rnd <= currentPercent)
					{
						objToSpawn = ip.item;
						break;
					}
				}

				//get any side to spawn
				rnd = UnityEngine.Random.Range (0f, 1f);
				float posX = 0f;
				float posY = 0f;
				
				if(rnd < 0.25f)//UP
				{
					posY = up;
					
					//random pos x
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				else if(rnd < 0.5f)//RIGHT
				{
					posX = right;
					
					//random  pos Y
					posY = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				else if(rnd < 0.75f)//LEFT
				{
					posX = left;
					
					//random  pos Y
					posY = UnityEngine.Random.Range(0.1f, 0.9f);
				}
				else//BOTTOM
				{
					posY = bottom;
					
					//random pos x
					posX = UnityEngine.Random.Range(0.1f, 0.9f);
				}

				Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(posX, posY));
				pos.z = 0;

				float rot = GetRotation (pos, objToSpawn.name);

				GameObject item = Instantiate (objToSpawn, pos, Quaternion.Euler(0, 0, rot)) as GameObject;

				if(stationary)
					item.GetComponent<Item>().vel = 0;

				itensInGame.Add(item.transform);
			}
		}

		StartCoroutine ("SpawnItens", LevelDesign.ItemSpawnTime);
	}

	public static void AddEnemy(GameObject obj)
	{
		enemiesInGame.Add (obj.transform);
	}

	private void RemoveEnemy(GameObject obj)
	{
		enemiesInGame.Remove (obj.transform);

		if(enemiesInGame.Count == 0)
		{
			if(NoMoreEnemies != null)
				NoMoreEnemies();
		}
	}

	private void GameOver()
	{
		StopSpawn ();
	}

	public void SpawnOrbs(int quantity, Vector3 position)
	{
		SpawnOrbs(quantity, position, 0, true);
	}

	public void SpawnOrbs(int quantity, Vector3 position, float spread)
	{
		SpawnOrbs(quantity, position, spread, true);
	}

	public void SpawnOrbs(int quantity, Vector3 position, float spread, bool stationary)
	{
		Debug.Log("Spawn Orbs");
		//preview verification anti inifinity-loop
		int minValue = 0;
		foreach(GameObject orb in orbs)
		{
			if(minValue == 0 || orb.GetComponent<PlasmaOrbItem>().orbs < minValue)
				minValue = orb.GetComponent<PlasmaOrbItem>().orbs;
		}
		
		//orbs can't sum up the quantity, correct it
		if(quantity % minValue != 0)
			quantity += minValue - (quantity % minValue);
		
		int quantitySpawned = 0;
		
		while(quantitySpawned < quantity)
		{
			GameObject orbToSpawn;
			int quantityToSpawn;
			
			do
			{
				orbToSpawn = orbs[UnityEngine.Random.Range(0, orbs.Count)];
				
				quantityToSpawn = orbToSpawn.GetComponent<PlasmaOrbItem>().orbs;
			}
			while(quantitySpawned + quantityToSpawn > quantity);

			float spawnAngle = UnityEngine.Random.Range(0, 2 * Mathf.PI);
			Vector3 spawnPosition = new Vector3(Mathf.Cos(spawnAngle), Mathf.Sin(spawnAngle)) * spread;

			GameObject orb = Instantiate(orbToSpawn, spawnPosition, Quaternion.identity) as GameObject;

			if(stationary)
				orb.GetComponent<Item>().vel = 0;

			itensInGame.Add(orb.transform);

			quantitySpawned += quantityToSpawn;

			Debug.Log("Orb spawned with value: " + quantityToSpawn + ". Sum: " + quantitySpawned);
		}
	}

	public void SpawnItem(Vector3 position, GameObject itemToSpawn)
	{
		GameObject item = Instantiate(itemToSpawn, position, Quaternion.identity) as GameObject;

		item.GetComponent<Item>().vel = 0;

		itensInGame.Add(item.transform);
	}

	private void Reset()
	{
		Debug.Log ("Clear all enemies");
		//Destroy all enemies by Destroy method
		/*foreach (Transform t in enemiesInGame)
		{
			if(t != null)
				Destroy (t.gameObject);
		}*/

		GameController.Instance.KillAllEnemies(false);

		foreach (Transform t in itensInGame)
		{
			if(t != null)
				Destroy (t.gameObject);
		}

		if(boss != null)
			Destroy(boss);

		itensInGame = new List<Transform> ();
		enemiesInGame = new List<Transform> ();
		boss = null;
	}
}