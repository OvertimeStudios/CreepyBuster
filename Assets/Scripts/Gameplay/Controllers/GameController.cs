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
	public static event Action OnGameOver;
	public static event Action OnLoseStacks;

	public static bool isGameRunning = false;
	public static bool gameOver;

	/// <summary>
	/// Total score for session
	/// </summary>
	private static int score;

	/// <summary>
	/// Current streak count
	/// </summary>
	private static int streakCount;

	/// <summary>
	/// Total enemies kill count
	/// </summary>
	public static int enemiesKillCount;

	/// <summary>
	/// How many times player used special without take damage.
	/// </summary>
	public static int specialStreak;

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
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= OnEnemyDied;
		MenuController.OnPanelOpening -= Reset;
		LevelDesign.OnPlayerLevelUp -= PlayerLevelUp;
	}

	void OnEnemyDied(GameObject enemy)
	{
		score += enemy.GetComponent<EnemyLife>().score;

		if(OnScoreUpdated != null)
			OnScoreUpdated();

		//only count streak outside special
		if(!AttackTargets.IsSpecialActive && enemy.GetComponent<EnemyLife>().countAsStreak)
		{
			StreakCount++;

			if(OnStreakUpdated != null)
				OnStreakUpdated();
		}
	}

	private void PlayerLevelUp()
	{
		if(LevelDesign.IsSpecialReady)
		{
			AttackTargets.Instance.UseSpecial();
		}
	}

	private void EnemiesSpawnLevelUp ()
	{

	}

	private void EnemiesTypesLevelUp ()
	{
		
	}

	private void EnemiesAttributesLevelUp ()
	{
		
	}
	
	private void TierLevelUp ()
	{
		
	}

	public void FingerHit()
	{
		if (LevelDesign.PlayerLevel > 0)
			LoseStacks ();
		else
			GameOver ();
	}

	private void LoseStacks()
	{
		StreakCount = 0;
		LevelDesign.PlayerLevel = 0;

		AttackTargets.Instance.StopSpecial ();

		if (OnLoseStacks != null)
			OnLoseStacks ();
	}

	public void GameOver()
	{
		isGameRunning = false;
		gameOver = true;

		if (OnGameOver != null)
			OnGameOver ();
	}

	public void StartGame()
	{
		enemiesKillCount = 0;
		score = 0;
		specialStreak = 0;

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
		gameOver = false;

		if(OnScoreUpdated != null)
			OnScoreUpdated();
	}

	void OnFingerUp(FingerUpEvent e)
	{
		GameOver ();
	}
}
