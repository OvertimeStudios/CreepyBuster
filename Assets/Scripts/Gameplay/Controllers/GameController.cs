using UnityEngine;
using System.Collections;
using System;

using UnityEngine.Advertisements;

public class GameController : MonoBehaviour 
{
	enum CauseOfDeath
	{
		FingerOff,
		LifeOut,
	}

	public static event Action OnGameStart;

	/// <summary>
	/// Occurs when on streak updated - before OnStreakUpdated
	/// </summary>
	public static event Action OnScoreUpdated;

	/// <summary>
	/// Occurs when on streak updated - after OnScoreUpdated
	/// </summary>
	public static event Action OnKill;
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
	public static event Action OnGameEnding;
	public static event Action OnContinuePlaying;
	public static event Action OnPause;
	public static event Action OnResume;

	public static bool isGameRunning = false;
	public static bool gameOver;
	private static bool slowedDown;
	private static bool frozen;
	private static bool invencible;
	private static int continues;
	private static bool bossTime;
	private static float lastTimeScale;

	private static bool isPaused;

	public float timeInvencibleAfterDamage;
	public float timeToShowGameOverScreen;
	public int orbsToContinue;
	public int pointsPerOrb = 10;
	public float timeInvencibleAfterContinue = 2f;

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
	private static int enemiesKillCount;

	/// <summary>
	/// How many times player used special without take damage.
	/// </summary>
	public static int specialStreak;

	public static int orbsCollected;
	
	private static GameObject player;

	#region get / set
	public static bool IsGamePaused
	{
		get { return isPaused; }
	}

	public static bool IsBossTime
	{
		get { return bossTime; }
	}

	public static int KillCount
	{
		get { return enemiesKillCount; }
	}

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
		get { return invencible || ScreenFeedback.IsDamageActive; }
	}

	public static bool IsTakingDamage
	{
		get { return ScreenFeedback.IsDamageActive; }
	}

	public static bool IsInvencibleByItem
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
		get { return TutorialController.IsRunning; }
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
		FingerDetector.OnTapGesture += OnDoubleTap;
		LevelDesign.OnBossReady += BossIsReady;
		BossLife.OnBossDied += BossDied; 
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
		FingerDetector.OnTapGesture -= OnDoubleTap;
		LevelDesign.OnBossReady -= BossIsReady;
		BossLife.OnBossDied -= BossDied;

		TutorialController.Instance.gameObject.SetActive (false);
	}

	void Start()
	{
		instance = this;
		isPaused = false;

		gameObject.SetActive(false);

		player = GameObject.FindWithTag ("Player");
	}

	void OnEnemyDied(GameObject enemy)
	{
		if(enemy.GetComponent<EnemyLife>().countAsKill)
		{
			enemiesKillCount += 1;

			if(OnKill != null)
				OnKill();

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
		
		if (LevelDesign.PlayerLevel == 0)
			NoMoreLifes ();
		
		if (OnFingerHit != null)
			OnFingerHit ();

		LoseStacks ();
	}

	private void LoseStacks()
	{
		StreakCount = LevelDesign.LastLevelPlayerStreak;
		LevelDesign.PlayerLevel = Mathf.Max(LevelDesign.PlayerLevel - 1, 0);
		realStreakCount = 0;
		specialStreak = 0;

		AttackTargets.Instance.StopSpecial ();

		if (OnLoseStacks != null)
			OnLoseStacks ();
	}

	private void NoMoreLifes()
	{
		if(GameController.IsTutorialRunning)
			StartCoroutine (ShowEndScreen (timeToShowGameOverScreen));
		else
			StartCoroutine (ShowContinueScreen (timeToShowGameOverScreen, CauseOfDeath.LifeOut));
	}

	public void GoToShop()
	{
		GameOver();

		MenuController.goToShop = true;
	}

	public void GameOver()
	{
		isGameRunning = false;
		gameOver = true;
		player.SetActive (false);

		MenuController.Instance.gameObject.SetActive(true);

		if (OnGameOver != null)
			OnGameOver ();
		
		gameObject.SetActive(false);
	}

	private IEnumerator ShowEndScreen(float waitTime)
	{
		isGameRunning = false;
		gameOver = true;
		player.SetActive (false);

		//fade out TimeScale
		lastTimeScale = Time.timeScale;
		float fadeEndTime = Time.realtimeSinceStartup + waitTime;
		while(Time.realtimeSinceStartup < fadeEndTime)
		{
			Time.timeScale = ((fadeEndTime - Time.realtimeSinceStartup) / waitTime) * lastTimeScale;
			yield return null;
		}

		Time.timeScale = 0;

		ShowEndScreen ();
	}

	private IEnumerator ShowContinueScreen(float waitTime, CauseOfDeath causeOfDeath)
	{
		if(OnGameEnding != null)
			OnGameEnding();

		isGameRunning = false;
		gameOver = true;
		player.SetActive (false);

		//fade out TimeScale
		lastTimeScale = Time.timeScale;
		float fadeEndTime = Time.realtimeSinceStartup + waitTime;
		while(Time.realtimeSinceStartup < fadeEndTime)
		{
			Time.timeScale = ((fadeEndTime - Time.realtimeSinceStartup) / waitTime) * lastTimeScale;
			yield return null;
		}
		
		Time.timeScale = 0;

		if (continues == 0 && Advertisement.IsReady () && Advertisement.isInitialized && Advertisement.isSupported)
			Popup.ShowVideoNo(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.Get("VIDEO_TO_PLAY"), null, ShowEndScreen, false);
		else
		{
			#if INFINITY_ORBS
			Popup.ShowYesNo(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.Get("INFINITY_ORBS_TO_PLAY"), PayContinueOrbs, ShowEndScreen);
			#else
			float orbsToPay = (orbsToContinue * Mathf.Pow(2, continues));

			if(Global.TotalOrbs >= orbsToPay)
				Popup.ShowYesNo(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.GetType ("WANT_TO_SPEND") + " " + orbsToPay + " " + Localization.GetType ("CONTINUE_PLAYING") + "\n \n (" + Localization.GetType ("YOU_HAVE") + " " + Global.TotalOrbs + " orbs.)", PayContinueOrbs, ShowEndScreen);
			else
				Popup.ShowOk(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.GetType ("NOT_ENOUGH_ORBS"), ShowEndScreen);
			#endif
		}

		if (OnShowContinueScreen != null)
			OnShowContinueScreen ();
	}

	private void ShowEndScreen()
	{
		if(OnGameEnding != null)
			OnGameEnding();

		Popup.Hide ();

		//Global.TotalOrbs += orbsCollected;

		HUDController.Instance.ShowEndScreen ();

		if (OnShowEndScreen != null)
			OnShowEndScreen ();
	}

	private void VideoWatched()
	{
		ContinuePlaying ();
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

		isGameRunning = true;
		gameOver = false;

		Popup.Hide ();
		
		if(OnContinuePlaying != null)
			OnContinuePlaying();

		UseInvencibility(timeInvencibleAfterContinue);

		PauseGame(false);

		//Popup.ShowBlank (Localization.Get ("FINGER_ON_SCREEN"));

		//StartCoroutine (WaitForFingerDown ());
	}

	private IEnumerator WaitForFingerDown()
	{
		Debug.Log ("WaitForFingerDown");

		while (FingerDetector.IsFingerDown)
			yield return null;

		while(!FingerDetector.IsFingerDown)
			yield return null;

		Debug.Log ("Continue");

		//KillAllEnemies(false);

		isGameRunning = true;
		gameOver = false;
		player.SetActive (true);

		Popup.Hide ();

		if(OnContinuePlaying != null)
			OnContinuePlaying();

		//fade in TimeScale
		float fadeEndTime = Time.realtimeSinceStartup + timeToShowGameOverScreen;
		while(Time.realtimeSinceStartup < fadeEndTime)
		{
			Time.timeScale = (timeToShowGameOverScreen - (fadeEndTime - Time.realtimeSinceStartup)) / timeToShowGameOverScreen;
			yield return null;
		}
		
		Time.timeScale = 1;
	}

	public void StartGame()
	{
		SoundController.Instance.CrossFadeMusic(SoundController.Musics.GameTheme, 1f);

		Reset ();
		gameObject.SetActive (true);

		//if(Global.IsTutorialEnabled)
			//TutorialController.Instance.gameObject.SetActive(true);

		if (FingerDetector.IsFingerDown)
		{
			Debug.Log("Active Player");
			player.SetActive (true);
		}
		else
		{
			//Popup.ShowBlank (Localization.Get ("FINGER_ON_SCREEN"));
		}

		StartCoroutine (WaitForPlayer ());
	}

	private IEnumerator WaitForPlayer()
	{
		Debug.Log ("WaitForPlayer");
		isGameRunning = true;

		player.transform.position = Vector3.zero;

		if(!player.activeSelf)
			PauseGame();

		while (!player.activeSelf)
			yield return null;

		if(isPaused)
			ResumeGame();

		Debug.Log ("Player found");
		Popup.Hide ();

		if (OnGameStart != null)
			OnGameStart ();

		if (Global.IsTutorialEnabled)
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
		specialStreak = 0;
		continues = 0;
		isPaused = false;
		frozen = false;
		slowedDown = false;
		invencible = false;
		bossTime = false;

		gameOver = false;

		if(OnScoreUpdated != null)
			OnScoreUpdated();

		if (OnReset != null)
			OnReset ();
	}

	void OnFingerDown(FingerDownEvent e)
	{
		if(Popup.IsActive || !isGameRunning)return;

		if(!isPaused || (isPaused && e.Selection != null && e.Selection.layer == LayerMask.NameToLayer("Tap&Hold")))
			ResumeGame();
	}

	void OnFingerUp(FingerUpEvent e)
	{
		/*if(GameController.isGameRunning && !GameController.IsTutorialRunning)
		{
			ScreenFeedback.ShowDamage(timeInvencibleAfterDamage);
			StartCoroutine (ShowContinueScreen (timeToShowGameOverScreen, CauseOfDeath.FingerOff));

			if(OnGameEnding != null)
				OnGameEnding();
		}
		else if(GameController.IsTutorialRunning && TutorialController.CanLose)
		{
			ScreenFeedback.ShowDamage(timeInvencibleAfterDamage);
			StartCoroutine (ShowEndScreen (timeToShowGameOverScreen));

			if(OnGameEnding != null)
				OnGameEnding();
		}*/

		if(!isGameRunning) return;

		if(!isPaused)
			PauseGame();
	}

	private void OnDoubleTap(TapGesture gesture)
	{
		if(isPaused)
			Popup.ShowYesNo("Are you sure you wanna quit the game?", QuitGame, null, true);
	}

	private void QuitGame()
	{
		//GameOver();
		ResumeGame();
		StartCoroutine (ShowEndScreen (0f));
	}

	private void PauseGame()
	{
		PauseGame(true);
	}

	private void PauseGame(bool getNewTimeScale)
	{
		Debug.Log("Game Paused");

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Pause);

		isPaused = true;
		//Popup.ShowBlank (Localization.Get("FINGER_ON_SCREEN"));

		if(getNewTimeScale)
			lastTimeScale = Time.timeScale;
		Time.timeScale = 0f;

		if(OnPause != null)
			OnPause();
	}

	private void ResumeGame()
	{
		Debug.Log("Game Resumed");

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Resume);

		isPaused = false;
		player.SetActive (true);
		//Popup.Hide();
		Time.timeScale = lastTimeScale;

		if(OnResume != null)
			OnResume();
	}

	private void BossIsReady()
	{
		bossTime = true;

		StopCoroutine ("WaitForNoMoreEnemies");
		StartCoroutine ("WaitForNoMoreEnemies");
	}

	private IEnumerator WaitForNoMoreEnemies()
	{
		while (SpawnController.EnemiesInGame > 0)
			yield return null;

		SoundController.Instance.FadeOut(1f);

		yield return new WaitForSeconds(3f);

		SoundController.Instance.PlayMusic(SoundController.Musics.BossTheme);

		SpawnController.SpawnBoss ();
	}

	public static void BossDied()
	{
		SoundController.Instance.CrossFadeMusic(SoundController.Musics.GameTheme, 1f);
		bossTime = false;
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
				UseInvencibility(Invencible.Time);
			break;

			case Item.Type.DeathRay:
				KillAllEnemies(true);
			break;

			case Item.Type.Frozen:
				if(OnFrozenCollected != null)
					OnFrozenCollected();
				
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Freeze);

				frozen = true;
				
				ScreenFeedback.ShowFrozen(Frozen.FrozenTime);

				StopCoroutine("FadeFrozen");
				StartCoroutine("FadeFrozen");
			break;
		}

		Debug.Log("Collected " + itemType.ToString());
	}

	private void UseInvencibility(float invencibleTime)
	{
		invencible = true;
		
		ScreenFeedback.ShowInvencibility(invencibleTime);
		
		StopCoroutine("FadeInvencible");
		StartCoroutine("FadeInvencible");
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

	public void KillAllEnemies(bool countPoints)
	{
		for(int i = SpawnController.enemiesInGame.Count - 1; i >= 0; i--)
		{
			Transform enemy = SpawnController.enemiesInGame[i];
			
			if(enemy == null || enemy.gameObject.name.Contains("Minion")) continue;
			
			EnemyLife enemyLife = enemy.GetComponent<EnemyLife>();
			
			enemyLife.Dead(countPoints);
		}

		//kill minions after
		for(int i = SpawnController.enemiesInGame.Count - 1; i >= 0; i--)
		{
			Transform enemy = SpawnController.enemiesInGame[i];
			
			if(enemy == null) continue;
			
			EnemyLife enemyLife = enemy.GetComponent<EnemyLife>();
			
			enemyLife.Dead(countPoints);
		}

		SpawnController.enemiesInGame.Clear ();
	}
}
