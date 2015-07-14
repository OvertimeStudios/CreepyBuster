using UnityEngine;
using System.Collections;
using System;

public class GameController : MonoBehaviour 
{
	public static event Action OnGameStart;

	/// <summary>
	/// Occurs when on streak updated - before OnStreakUpdated
	/// </summary>
	public static event Action OnScoreUpdated;

	/// <summary>
	/// Occurs when on streak updated - after OnScoreUpdated
	/// </summary>
	public static event Action OnStreakUpdated;
	public static event Action OnRealStreakUpdated;
	public static event Action OnGameOver;
	public static event Action OnLoseStacks;
	public static event Action OnSlowDownCollected;
	public static event Action OnSlowDownFade;

	public static bool isGameRunning = false;
	public static bool gameOver;
	private static bool slowedDown;
	private static bool invencible;

	/// <summary>
	/// Total score for session
	/// </summary>
	private static int score;

	/// <summary>
	/// Current streak count (can be increased by items)
	/// </summary>
	private static int streakCount;

	/// <summary>
	/// Sometimes player increase streakCount by itens (i.e. Item.Type.LevelUp). This propertie count only kill streak
	/// </summary>
	private static int realStreakCount;

	/// <summary>
	/// Total enemies kill count
	/// </summary>
	public static int enemiesKillCount;

	/// <summary>
	/// How many times player used special without take damage.
	/// </summary>
	public static int specialStreak;

	public static int orbsCollected;

	#region get / set
	public static int StreakCount
	{
		get { return streakCount; }

		set
		{
			streakCount = value;

			if(OnStreakUpdated != null)
				OnStreakUpdated();
		}
	}

	public static int RealStreakCount
	{
		get { return realStreakCount; }

		set
		{
			realStreakCount = value;
			
			if(OnRealStreakUpdated != null)
				OnRealStreakUpdated();
		}
	}

	public static bool IsSlowedDown
	{
		get { return slowedDown; }
	}

	public static bool IsInvencible
	{
		get { return invencible; }
	}

	public static int Score
	{
		get { return score; }
		
		set
		{
			score = value;
			
			if (OnScoreUpdated != null)
				OnScoreUpdated ();
		}
	}
	#endregion

	#region singleton
	private static GameController instance;
	public static GameController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<GameController>();

			return instance;
		}
	}
	#endregion

	void OnEnable()
	{
		EnemyLife.OnDied += OnEnemyDied;
		MenuController.OnPanelOpening += Reset;
		LevelDesign.OnPlayerLevelUp += PlayerLevelUp;
		Item.OnCollected += OnItemCollected;
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= OnEnemyDied;
		MenuController.OnPanelOpening -= Reset;
		LevelDesign.OnPlayerLevelUp -= PlayerLevelUp;
		Item.OnCollected -= OnItemCollected;
	}

	void OnEnemyDied(GameObject enemy)
	{
		score += enemy.GetComponent<EnemyLife>().score;

		if(OnScoreUpdated != null)
			OnScoreUpdated();

		//only count streak outside special
		if(!AttackTargets.IsSpecialActive && enemy.GetComponent<EnemyLife>().countAsStreak)
		{
			//call Action on set method
			StreakCount++;

			RealStreakCount++;
		}
	}

	private void PlayerLevelUp()
	{
		if(LevelDesign.IsSpecialReady)
		{
			AttackTargets.Instance.UseSpecial();
		}
	}

	public void FingerHit()
	{
		if(IsInvencible) return;

		if (LevelDesign.PlayerLevel > 0)
			LoseStacks ();
		else
			GameOver ();
	}

	private void LoseStacks()
	{
		StreakCount = 0;
		LevelDesign.PlayerLevel = 0;
		realStreakCount = 0;

		AttackTargets.Instance.StopSpecial ();

		if (OnLoseStacks != null)
			OnLoseStacks ();
	}

	public void GameOver()
	{
		isGameRunning = false;
		gameOver = true;
		Global.TotalOrbs += orbsCollected;

		if (OnGameOver != null)
			OnGameOver ();
	}

	public void StartGame()
	{
		enemiesKillCount = 0;
		score = 0;
		specialStreak = 0;
		orbsCollected = 0;
		realStreakCount = 0;

		gameOver = false;
		isGameRunning = true;

		if (OnGameStart != null)
			OnGameStart ();
	}

	private void Reset()
	{
		enemiesKillCount = 0;
		score = 0;
		StreakCount = 0;
		orbsCollected = 0;
		realStreakCount = 0;

		gameOver = false;

		if(OnScoreUpdated != null)
			OnScoreUpdated();
	}

	void OnFingerUp(FingerUpEvent e)
	{
		GameOver ();
	}

	private void OnItemCollected(Item.Type itemType, GameObject gameObject)
	{
		switch(itemType)
		{
			case Item.Type.PlasmaOrb:
				orbsCollected += gameObject.GetComponent<PlasmaOrbItem>().orbs;
			break;

			case Item.Type.LevelUp:
				StreakCount = LevelDesign.NextStreakToPlayerLevelUp;
			break;

			case Item.Type.SlowDown:

				if(OnSlowDownCollected != null)
					OnSlowDownCollected();
				
				slowedDown = true;
				
				StopCoroutine("FadeSlowDown");
				StartCoroutine("FadeSlowDown");

			break;

			case Item.Type.Invecibility:
				invencible = true;

				StopCoroutine("FadeInvencible");
				StartCoroutine("FadeInvencible");
			break;

			case Item.Type.DeathRay:
				for(int i = SpawnController.enemiesInGame.Count - 1; i >= 0; i--)
				{
					EnemyLife enemyLife = SpawnController.enemiesInGame[i].GetComponent<EnemyLife>();
					
					enemyLife.Dead();
				}
			break;
		}

		Debug.Log("Collected " + itemType.ToString());
	}

	private IEnumerator FadeSlowDown()
	{
		yield return new WaitForSeconds(SlowDown.SlowTime);

		Debug.Log ("Slow Down Faded");

		slowedDown = false;

		if(OnSlowDownFade != null)
			OnSlowDownFade ();
	}

	private IEnumerator FadeInvencible()
	{
		yield return new WaitForSeconds(Invencible.Time);

		Debug.Log ("Invencible Faded");

		invencible = false;
	}
}
