using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpawnController : MonoBehaviour 
{
	public static event Action OnSpawn;

	public static List<Transform> enemiesInGame;
	public static List<Transform> itensInGame;

	//viewport coordinates outside of screen
	private const float bottom = -0.2f;
	private const float left = -0.2f;
	private const float up = 1.2f;
	private const float right = 1.2f;

	private static float lastPosX = 0;
	private static float lastPosY = 0;

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
			return !GameController.IsFrozen && !GameController.IsSlowedDown && GameController.isGameRunning && !GameController.IsTutorialRunning;
		}
	}
	#endregion

	void OnEnable()
	{
		MenuController.OnPanelClosed += Reset;
		GameController.OnGameStart += StartSpawn;
		GameController.OnGameOver += GameOver;
		EnemyMovement.OnOutOfScreen += RemoveEnemy;
		EnemyLife.OnDied += RemoveEnemy;
		Legion.OnMinionSpawned += AddEnemy;	
		//Legion.OnMinionReleased += AddEnemy;
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
		GameController.OnGameStart -= StartSpawn;
		GameController.OnGameOver -= GameOver;
		EnemyMovement.OnOutOfScreen -= RemoveEnemy;
		EnemyLife.OnDied -= RemoveEnemy;
		Legion.OnMinionSpawned -= AddEnemy;	
		//Legion.OnMinionReleased -= AddEnemy;
	}

	// Use this for initialization
	void Start () 
	{
		enemiesInGame = new List<Transform> ();
		itensInGame = new List<Transform> ();

		SpawnBackground ();
	}

	public void StartSpawn()
	{
		StartCoroutine ("SpawnEnemies", LevelDesign.EnemiesSpawnTime);
		StartCoroutine ("SpawnItens", LevelDesign.ItemSpawnTime);
	}

	public void StopSpawn()
	{
		StopCoroutine ("SpawnEnemies");
		StopCoroutine ("SpawnItens");
	}
	
	private IEnumerator SpawnEnemies(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		SpawnEnemies ();
	}

	public static void SpawnEnemy(GameObject enemy)
	{
		Debug.Log ("SpawnController.SpawnEnemy");

		Vector3 pos = GetSpawnPosition();
		float rot = GetRotation(pos);

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

	public static float GetRotation(Vector3 pos)
	{
		Vector3 viewportPosition = Camera.main.WorldToViewportPoint (pos);

		float rot = 0f;

		if (viewportPosition.x < 0)//LEFT
			rot = 0f;
		if (viewportPosition.x > 1)//RIGHT
			rot = 180f;
		if (viewportPosition.y < 0)//DOWN
			rot = 90f;
		if(viewportPosition.y > 1)//UP
			rot = -90f;

		return rot;
	}

	private void SpawnBackground()
	{
		//TODO: Spawn according to players choice

		int rnd = (int)UnityEngine.Random.Range (0, LevelDesign.Instance.backgrounds.Count);

		Instantiate (LevelDesign.Instance.backgrounds [rnd]);
	}

	private IEnumerator SpawnItens(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		
		SpawnItens ();
	}

	private void SpawnItens()
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

				float rot = GetRotation (pos);

				GameObject item = Instantiate (objToSpawn, pos, Quaternion.Euler(0, 0, rot)) as GameObject;

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
	}

	private void GameOver()
	{
		StopSpawn ();
	}

	private void Reset()
	{
		Debug.Log ("Clear all enemies");
		foreach (Transform t in enemiesInGame)
		{
			if(t != null)
				Destroy (t.gameObject);
		}

		foreach (Transform t in itensInGame)
		{
			if(t != null)
				Destroy (t.gameObject);
		}

		itensInGame = new List<Transform> ();
		enemiesInGame = new List<Transform> ();
	}
}