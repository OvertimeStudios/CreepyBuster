using UnityEngine;
using System.Collections;
using System;

using UnityEngine.Advertisements;

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
	public static event Action OnFrozenCollected;
	public static event Action OnFrozenFade;
	public static event Action OnShowContinueScreen;
	public static event Action OnShowEndScreen;
	public static event Action OnReset;
	public static event Action OnFingerHit;

	public static bool isGameRunning = false;
	public static bool gameOver;
	private static bool slowedDown;
	private static bool frozen;
	private static bool invencible;
	private static int continues;

	public float timeInvencibleAfterDamage;
	public float timeToShowGameOverScreen;
	public int orbsToContinue;
	public int pointsPerOrb = 10;

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
	
	private static GameObject player;

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

	public static bool IsFrozen
	{
		get { return frozen; }
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

	public static bool IsTutorialRunning
	{
		get { return TutorialController.Instance.gameObject.activeInHierarchy; }
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
		RewardedVideoPlayer.OnRevivePlayer += VideoWatched;
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerUpEvent += OnFingerUp;
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= OnEnemyDied;
		MenuController.OnPanelOpening -= Reset;
		LevelDesign.OnPlayerLevelUp -= PlayerLevelUp;
		Item.OnCollected -= OnItemCollected;
		RewardedVideoPlayer.OnRevivePlayer -= VideoWatched;
		FingerDetector.OnFingerDownEvent -= OnFingerDown;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;

		TutorialController.Instance.gameObject.SetActive (false);
	}

	void Start()
	{
		instance = this;

		gameObject.SetActive(false);

		player = GameObject.FindWithTag ("Player");
	}

	void OnEnemyDied(GameObject enemy)
	{
		if(enemy.GetComponent<EnemyLife>().countAsKill)
		{
			score += enemy.GetComponent<EnemyLife>().score;

			if(OnScoreUpdated != null)
				OnScoreUpdated();

			//only count streak outside special
			if(enemy.GetComponent<EnemyLife>().countAsStreak)
			{
				RealStreakCount++;

				//call Action on set method
				if(!AttackTargets.IsSpecialActive)
					StreakCount++;
			}
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

		ScreenFeedback.ShowDamage (timeInvencibleAfterDamage);

		if (OnFingerHit != null)
			OnFingerHit ();

		if (LevelDesign.PlayerLevel > 0)
			LoseStacks ();
		else
			NoMoreLifes ();
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

	private void NoMoreLifes()
	{
		isGameRunning = false;

		StartCoroutine (ShowContinueScreen (timeToShowGameOverScreen));
	}

	public void GameOver()
	{
		isGameRunning = false;
		gameOver = true;

		MenuController.Instance.gameObject.SetActive(true);

		if (OnGameOver != null)
			OnGameOver ();
		
		gameObject.SetActive(false);
	}

	private IEnumerator ShowContinueScreen(float waitTime)
	{
		isGameRunning = false;
		gameOver = true;

		yield return new WaitForSeconds (waitTime);

		if (continues == 0 && Advertisement.IsReady ())
			Popup.ShowVideoNo("You got hit! \n \n Do you want to watch 1 video to continue playing?", null, ShowEndScreen, false);
		else
		{
			#if INFINITY_ORBS
			Popup.ShowYesNo("You got hit! \n \n But you have infinity orbs cheat. Do you want to continue, m'lord?", PayContinueOrbs, ShowEndScreen);
			#else
			float orbsToPay = (orbsToContinue * Mathf.Pow(2, continues));

			if(Global.TotalOrbs >= orbsToPay)
				Popup.ShowYesNo("You got hit! \n \n Do you want to spent " + orbsToPay + " orbs to continue playing? \n \n (You have " + Global.TotalOrbs + " orbs.)", PayContinueOrbs, ShowEndScreen);
			else
				Popup.ShowOk("You got hit! \n \n You don't have enough orbs to continue playing", ShowEndScreen);
			#endif
		}

		if (OnShowContinueScreen != null)
			OnShowContinueScreen ();
	}

	private void ShowEndScreen()
	{
		Popup.Hide ();

		Global.TotalOrbs += orbsCollected;

		HUDController.Instance.ShowEndScreen ();

		if (OnShowEndScreen != null)
			OnShowEndScreen ();
	}

	private void VideoWatched()
	{
		ContinuePlaying ();

		Popup.Hide ();
	}

	private void PayContinueOrbs()
	{
		#if !INFINITY_ORBS
		Global.TotalOrbs -= (int)(orbsToContinue * Mathf.Pow (2, continues));
		#endif
		ContinuePlaying ();
	}

	private void ContinuePlaying()
	{
		continues++;

		Debug.Log ("ContinuePlaying");

		StartCoroutine (WaitForFingerDown ());
	}

	private IEnumerator WaitForFingerDown()
	{
		Debug.Log ("WaitForFingerDown");

		while (FingerDetector.IsFingerDown)
			yield return null;

		while(!FingerDetector.IsFingerDown)
			yield return null;

		Debug.Log ("Continue");

		KillAllEnemies(false);

		isGameRunning = true;
		gameOver = false;
		player.SetActive (true);
	}

	public void StartGame()
	{
		Reset ();
		gameObject.SetActive (true);

		Debug.Log ("FingerDetector.IsFingerDown: " + FingerDetector.IsFingerDown);
		if (FingerDetector.IsFingerDown)
		{
			Debug.Log("Active Player");
			player.SetActive (true);
		}

		StartCoroutine (WaitForPlayer ());
	}

	private IEnumerator WaitForPlayer()
	{
		Debug.Log ("WaitForPlayer");
		isGameRunning = true;

		while (!player.activeSelf)
			yield return null;

		Debug.Log ("Player found");

		enemiesKillCount = 0;
		score = 0;
		specialStreak = 0;
		orbsCollected = 0;
		realStreakCount = 0;
		continues = 0;
		
		gameOver = false;
		
		if (OnGameStart != null)
			OnGameStart ();

		if (Global.RunTutorial)
			TutorialController.Instance.gameObject.SetActive (true);
	}

	private void Reset()
	{
		Debug.Log ("Reset");
		enemiesKillCount = 0;
		score = 0;
		StreakCount = 0;
		orbsCollected = 0;
		realStreakCount = 0;
		continues = 0;

		gameOver = false;

		if(OnScoreUpdated != null)
			OnScoreUpdated();

		if (OnReset != null)
			OnReset ();
	}

	void OnFingerDown(FingerDownEvent e)
	{
		if(isGameRunning)
			player.SetActive (true);
	}

	void OnFingerUp(FingerUpEvent e)
	{
		if(GameController.isGameRunning && !GameController.IsTutorialRunning)
			StartCoroutine (ShowContinueScreen (timeToShowGameOverScreen));
	}

	private void OnItemCollected(Item.Type itemType, GameObject gameObject)
	{
		switch(itemType)
		{
			case Item.Type.PlasmaOrb:
				orbsCollected += gameObject.GetComponent<PlasmaOrbItem>().orbs;
				Debug.Log("Orbs collected: " + gameObject.GetComponent<PlasmaOrbItem>().orbs + ". Total: " + orbsCollected);
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
				
				ScreenFeedback.ShowInvencibility(Invencible.Time);

				StopCoroutine("FadeInvencible");
				StartCoroutine("FadeInvencible");
			break;

			case Item.Type.DeathRay:
				KillAllEnemies(true);
			break;

			case Item.Type.Frozen:
				if(OnFrozenCollected != null)
					OnFrozenCollected();
				
				frozen = true;
				
				ScreenFeedback.ShowFrozen(Frozen.FrozenTime);

				StopCoroutine("FadeFrozen");
				StartCoroutine("FadeFrozen");
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

	private IEnumerator FadeFrozen()
	{
		yield return new WaitForSeconds(Frozen.FrozenTime);
		
		Debug.Log ("Frozen Faded");
		
		frozen = false;
		
		if(OnFrozenFade != null)
			OnFrozenFade ();
	}

	private IEnumerator FadeInvencible()
	{
		yield return new WaitForSeconds(Invencible.Time);

		Debug.Log ("Invencible Faded");

		invencible = false;
	}

	private void KillAllEnemies(bool countPoints)
	{
		for(int i = SpawnController.enemiesInGame.Count - 1; i >= 0; i--)
		{
			EnemyLife enemyLife = SpawnController.enemiesInGame[i].GetComponent<EnemyLife>();
			
			enemyLife.Dead(countPoints);
		}
	}
}
