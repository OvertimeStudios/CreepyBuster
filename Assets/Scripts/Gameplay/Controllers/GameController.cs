using UnityEngine;
using UnityEngine.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITYADS_IMPLEMENTED
using UnityEngine.Advertisements;
#endif

public class GameController : MonoBehaviour 
{
	private const string BASIC = "C1";
	private const string BOOMERANG = "C2";
	private const string FOLLOWER = "C7";
	private const string LEGION = "C5";
	private const string CHARGER = "C4";
	private const string ZIGZAG = "C3";

	private const string BOSS1 = "Meteoro";
	private const string BOSS2 = "Minhoco";
	private const string BOSS3 = "Illusion";

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
	#if UNITY_WEBGL
	public static event Action OnDemoOver;
	#endif

	public static bool isGameRunning = false;
	public static bool gameOver;
	private static bool slowedDown;
	private static bool frozen;
	private static bool invencible;
	private static bool shield;
	private static int continues;
	private static bool bossTime;
	private static float lastTimeScale;

	private static bool isPaused;

	public float timeInvencibleAfterDamage;
	public float timeToShowGameOverScreen;
	public int orbsToContinue;
	public int pointsPerOrb = 10;
	public float timeInvencibleAfterContinue = 5f;
	public float timeFrozen = 5f;

	#region game stats
	private static int creepsKilled;

	private static int basicsKilled;
	private static int boomerangsKilled;
	private static int chargersKilled;
	private static int zigzagsKilled;
	private static int legionsKilled;
	private static int followersKilled;

	public static int boss1Killed;
	public static int boss2Killed;
	public static int boss3Killed;

	public static int frozenCollected;
	public static int levelUpCollected;
	public static int invencibilityCollected;
	public static int deathRayCollected;

	public static int enemiesMissed;
	public static int orbsMissed;
	public static int powerUpsMissed;

	public static int orbsCollected;

	private static int bossEncounters;

	public static int maxStreak;

	public static float matchTime;
	public static float energySpent;

	public static int hitsByBasic;
	public static int hitsByBoomerang;
	public static int hitsByZigZag;
	public static int hitsByCharger;
	public static int hitsByLegion;
	public static int hitsByFollower;
	public static int hitsByBoss1;
	public static int hitsByBoss2;
	public static int hitsByBoss3;

	public static float leftTime;
	public static float rightTime;

	public static float timeOnSpecial1;
	public static float timeOnSpecial2;
	public static float timeOnSpecial3;
	public static float timeOnSpecial4;
	public static float timeOnSpecial5;
	public static float timeOnSpecial6;
	#endregion

	#region analytics variables
	private static int continuesVideo;
	private static int continuesOrbs;
	public static bool watchedReplay;
	public static bool watchedDoubleOrbs;
	#endregion

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

			if(realStreakCount > maxStreak)
				maxStreak = realStreakCount;

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

	public static bool IsShieldActive
	{
		get { return shield; }
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

	void Update()
	{
		if(isGameRunning)
			matchTime += Time.deltaTime;
	}

	void OnEnemyDied(GameObject enemy)
	{
		if(enemy.GetComponent<EnemyLife>().countAsKill)
		{
			enemiesKillCount++;

			//game stats
			creepsKilled++;

			EnemiesPercent.EnemyNames enemyName = enemy.GetComponent<EnemyLife>().type;
			if(enemyName == EnemiesPercent.EnemyNames.Blu)
			{
				Global.UnlockCreep(CreepData.CreepType.Basic);
				basicsKilled++;

				Debug.Log("BASIC KILLED: " + basicsKilled);
			}
			if(enemyName == EnemiesPercent.EnemyNames.Spiral)
			{
				Global.UnlockCreep(CreepData.CreepType.Boomerang);
				boomerangsKilled++;
			}
			if(enemyName == EnemiesPercent.EnemyNames.Ziggy)
			{
				Global.UnlockCreep(CreepData.CreepType.ZigZag);
				zigzagsKilled++;
			}
			if(enemyName == EnemiesPercent.EnemyNames.Charger)
			{
				Global.UnlockCreep(CreepData.CreepType.Charger);
				chargersKilled++;
			}
			if(enemyName == EnemiesPercent.EnemyNames.Legion)
			{
				Global.UnlockCreep(CreepData.CreepType.Legion);
				legionsKilled++;
			}
			if(enemyName == EnemiesPercent.EnemyNames.Follower)
			{
				Global.UnlockCreep(CreepData.CreepType.Follower);
				followersKilled++;
			}

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
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.LevelUp);

		if(LevelDesign.IsSpecialReady)
		{
			AttackTargets.Instance.UseSpecial();
		}
	}

	public void FingerHit(GameObject hit)
	{
		if(IsInvencible) return;

		//game stats
		string enemyName = hit.name;
		Debug.Log("Hit by: " + hit.name);
		if(enemyName.Contains(BASIC))
			hitsByBasic++;
		if(enemyName.Contains(BOOMERANG))
			hitsByBoomerang++;
		if(enemyName.Contains(ZIGZAG))
			hitsByZigZag++;
		if(enemyName.Contains(CHARGER))
			hitsByCharger++;
		if(enemyName.Contains(LEGION))
			hitsByLegion++;
		if(enemyName.Contains(FOLLOWER))
			hitsByFollower++;
		if(enemyName.Contains(BOSS1))
			hitsByBoss1++;
		if(enemyName.Contains(BOSS2))
			hitsByBoss2++;
		if(enemyName.Contains(BOSS3))
			hitsByBoss3++;
		
		if (OnFingerHit != null)
			OnFingerHit ();

		if(IsShieldActive)
		{
			shield = false;
			SoundController.Instance.PlaySoundFX(SoundController.SoundFX.DamageShield);
			ScreenFeedback.HideShield();
		}
		else
		{
			if (LevelDesign.PlayerLevel == 0)
				NoMoreLifes ();

			ScreenFeedback.ShowDamage (timeInvencibleAfterDamage);
			SoundController.Instance.PlaySoundFX(SoundController.SoundFX.PlayerDamage);

			LoseStacks ();
		}
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
		StartCoroutine (ShowContinueScreen (timeToShowGameOverScreen, CauseOfDeath.LifeOut));
	}

	public void GoToShop()
	{
		MenuController.goToShop = true;

		GameOver();
	}



	public void GameOver()
	{
		isGameRunning = false;
		gameOver = true;
		player.SetActive (false);

		MenuController.Instance.gameObject.SetActive(true);

		DataCloudPrefs.Save();

		if (OnGameOver != null)
			OnGameOver ();

		#if UNITYANALYTICS_IMPLEMENTED
		UnityAnalyticsHelper.NextScreen nextScreen;
		nextScreen = (MenuController.goToShop) ? UnityAnalyticsHelper.NextScreen.Shop : UnityAnalyticsHelper.NextScreen.Menu;

		UnityAnalyticsHelper.GameOver(Score, orbsCollected, matchTime, continuesVideo, continuesOrbs, nextScreen, watchedReplay, watchedDoubleOrbs);
		UnityAnalyticsHelper.Kills(basicsKilled, boomerangsKilled, zigzagsKilled, chargersKilled, legionsKilled, followersKilled, boss1Killed, boss2Killed, boss3Killed);
		UnityAnalyticsHelper.KilledBy(hitsByBasic, hitsByBoomerang, hitsByZigZag, hitsByCharger, hitsByLegion, hitsByFollower, hitsByBoss1, hitsByBoss2, hitsByBoss3);
		#endif

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

	private void UpdateGameStats()
	{
		Global.BasicsKilled += basicsKilled;
		Global.BoomerangsKilled += boomerangsKilled;
		Global.ZigZagsKilled += zigzagsKilled;
		Global.ChargersKilled += chargersKilled;
		Global.LegionsKilled += legionsKilled;
		Global.FollowersKilled += followersKilled;

		Global.Boss1Killed += boss1Killed;
		Global.Boss2Killed += boss2Killed;
		Global.Boss3Killed += boss3Killed;

		Global.FrozenCollected += frozenCollected;
		Global.InvencibilityCollected += invencibilityCollected;
		Global.LevelUpCollected += levelUpCollected;
		Global.DeathRayCollected += deathRayCollected;

		Global.OrbsMissed += orbsMissed;
		Global.PowerUpsMissed += powerUpsMissed;
		Global.EnemiesMissed += enemiesMissed;

		if(maxStreak > Global.MaxStreak)
			Global.MaxStreak = maxStreak;

		if (Score > Global.HighScore)
		{
			Global.HighScore = Score;

			/*
			#if FACEBOOK_IMPLEMENTED && DB_IMPLEMENTED
			if(FacebookController.IsLoggedIn)
				StartCoroutine(DBHandler.UpdateUserScore(DBHandler.User.id, Score));
			#endif
			*/

			#if LEADERBOARDS_IMPLEMENTED
			LeaderboardsHelper.SendScore(Score);
			#endif
		}
		
		if (Score > Global.SessionScore)
			Global.SessionScore = Score;

		Global.BossEncounters += bossEncounters;
		Global.TimePlayed += (int)matchTime;
		Global.EnergySpent += (int)energySpent;

		if(LightBehaviour.maxTimeOnSide > Global.SideLeftRight)
			Global.SideLeftRight = (int)LightBehaviour.maxTimeOnSide;

		if(matchTime > Global.LongestMatch)
			Global.LongestMatch = (int)matchTime;

		Global.HitsByBasic += hitsByBasic;
		Global.HitsByBoomerang += hitsByBoomerang;
		Global.HitsByZigZag += hitsByZigZag;
		Global.HitsByCharger += hitsByCharger;
		Global.HitsByLegion += hitsByLegion;
		Global.HitsByFollower += hitsByFollower;
		Global.HitsByBoss1 += hitsByBoss1;
		Global.HitsByBoss2 += hitsByBoss2;
		Global.HitsByBoss3 += hitsByBoss3;

		Global.TimeOnRight += (int)rightTime;
		Global.TimeOnLeft += (int)leftTime;

		Global.TimeOnSpecial1 += (int)timeOnSpecial1;
		Global.TimeOnSpecial2 += (int)timeOnSpecial2;
		Global.TimeOnSpecial3 += (int)timeOnSpecial3;
		Global.TimeOnSpecial4 += (int)timeOnSpecial4;
		Global.TimeOnSpecial5 += (int)timeOnSpecial5;
		Global.TimeOnSpecial6 += (int)timeOnSpecial6;
	}

	private void UpdateDailyMissionStats()
	{
		DailyMissionController.BasicsKilled += basicsKilled;
		DailyMissionController.BoomerangsKilled += boomerangsKilled;
		DailyMissionController.ZigZagsKilled += zigzagsKilled;
		DailyMissionController.ChargersKilled += chargersKilled;
		DailyMissionController.LegionsKilled += legionsKilled;
		DailyMissionController.FollowersKilled += followersKilled;
		
		DailyMissionController.Boss1Killed += boss1Killed;
		DailyMissionController.Boss2Killed += boss2Killed;
		DailyMissionController.Boss3Killed += boss3Killed;
		
		DailyMissionController.FrozenCollected += frozenCollected;
		DailyMissionController.InvincibilityCollected += invencibilityCollected;
		DailyMissionController.LevelUpCollected += levelUpCollected;
		DailyMissionController.DeathRayCollected += deathRayCollected;

		if (Score > DailyMissionController.HighScore)
			DailyMissionController.HighScore = Score;
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

		Debug.Log("ShowContinueScreen()");

		#if ADMOB_IMPLEMENTED
		Debug.Log(string.Format("AdMobHelper.IsRewardedVideoReady? {0}",AdMobHelper.IsRewardedVideoReady));
		if (continues == 0 && AdMobHelper.IsRewardedVideoReady)
			Popup.ShowVideoNo(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.Get("VIDEO_TO_PLAY"), ShowAdToContinue, ShowEndScreen, false);
		else
		{
		#endif
			#if INFINITY_ORBS
			Popup.ShowYesNo(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.Get("INFINITY_ORBS_TO_PLAY"), PayContinueOrbs, ShowEndScreen);
			#else
			float orbsToPay = (orbsToContinue * Mathf.Pow(2, continues));
			Debug.Log("orbsToPay: " + orbsToPay);
			if(Global.TotalOrbs >= orbsToPay)
				Popup.ShowYesNo(Localization.Get(causeOfDeath.ToString()) + "\n \n" + string.Format(Localization.Get ("WANT_TO_SPEND"), orbsToPay) + "\n \n (" + string.Format(Localization.Get ("YOU_HAVE"), Global.TotalOrbs) + ")", PayContinueOrbs, ShowEndScreen);
			else
				Popup.ShowOk(Localization.Get(causeOfDeath.ToString()) + "\n \n" + Localization.Get ("NOT_ENOUGH_ORBS"), ShowEndScreen);
			#endif
		#if ADMOB_IMPLEMENTED
		}
		#endif

		if (OnShowContinueScreen != null)
			OnShowContinueScreen ();
	}

	private void ShowAdToContinue()
	{
		AdMobHelper.ShowRewardedVideo(ContinuePlaying, SoundController.Instance.MuteForAds, SoundController.Instance.UnmuteForAds);
	}

	private void ShowEndScreen()
	{
		if(OnGameEnding != null)
			OnGameEnding();

		UpdateGameStats();
		UpdateDailyMissionStats();

		Popup.Hide ();

		if (OnShowEndScreen != null)
			OnShowEndScreen ();

		#if UNITY_WEBGL
		if(IsBossTime)
			EndDemo();
		#endif
	}

	private void VideoWatched()
	{
		continuesVideo++;
	}

	private void PayContinueOrbs()
	{
		continuesOrbs++;
		#if !INFINITY_ORBS
		Global.TotalOrbs -= (int)(orbsToContinue * Mathf.Pow (2, continues));
		#endif
		ContinuePlaying ();
	}

	private void ContinuePlaying()
	{
		continues++;

		Debug.Log ("ContinuePlaying. continues used: " + continues);

		ReachMaxLevel();

		isGameRunning = true;
		gameOver = false;

		Popup.Hide ();
		
		if(OnContinuePlaying != null)
			OnContinuePlaying();

		UseInvencibility(timeInvencibleAfterContinue);

		PauseGame(false);
	}

	private void ReachMaxLevel()
	{
		while(!LevelDesign.IsPlayerMaxLevel)
			StreakCount = LevelDesign.NextStreakToPlayerLevelUp;
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
		#if UNITYANALYTICS_IMPLEMENTED
		if(!Global.sentOnEnterGame)
		{
			UnityAnalyticsHelper.EnterOnGame();
			Global.sentOnEnterGame = true;
		}
		#endif

		SoundController.Instance.CrossFadeMusic(SoundController.Musics.GameTheme, 1f);

		isGameRunning = true;
		Reset ();
		gameObject.SetActive (true);

		//if(Global.IsTutorialEnabled)
		TutorialController.Instance.gameObject.SetActive(true);

		if (FingerDetector.IsFingerDown)
		{
			Debug.Log("Active Player");
			player.SetActive (true);
		}
		else
		{
			//Popup.ShowBlank (Localization.Get ("FINGER_ON_SCREEN"));
		}

		Global.GamesPlayed++;

		StartCoroutine (WaitForPlayer ());
	}

	private IEnumerator WaitForPlayer()
	{
		Debug.Log ("WaitForPlayer");

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

		//if (Global.IsTutorialEnabled)
			//TutorialController.Instance.gameObject.SetActive (true);
	}

	private void Reset()
	{
		Debug.Log ("Reset");
		enemiesKillCount = 0;
		score = 0;
		StreakCount = 0;
		realStreakCount = 0;
		specialStreak = 0;
		continues = 0;
		isPaused = false;
		frozen = false;
		slowedDown = false;
		invencible = false;
		bossTime = false;

		#region game stats
		creepsKilled = 0;

		basicsKilled = 0;
		boomerangsKilled = 0;
		chargersKilled = 0;
		zigzagsKilled = 0;
		legionsKilled = 0;
		followersKilled = 0;
		
		boss1Killed = 0;
		boss2Killed = 0;
		boss3Killed = 0;

		frozenCollected = 0;
		levelUpCollected = 0;
		invencibilityCollected = 0;
		deathRayCollected = 0;

		enemiesMissed = 0;
		orbsMissed = 0;
		powerUpsMissed = 0;

		orbsCollected = 0;
		bossEncounters = 0;

		maxStreak = 0;
		matchTime = 0;
		energySpent = 0;

		hitsByBasic = 0;
		hitsByBoomerang = 0;
		hitsByZigZag = 0;
		hitsByCharger = 0;
		hitsByLegion = 0;
		hitsByFollower = 0;
		hitsByBoss1 = 0;
		hitsByBoss2 = 0;
		hitsByBoss3 = 0;

		leftTime = 0;
		rightTime = 0;

		timeOnSpecial1 = 0;
		timeOnSpecial2 = 0;
		timeOnSpecial3 = 0;
		timeOnSpecial4 = 0;
		timeOnSpecial5 = 0;
		timeOnSpecial6 = 0;
		#endregion

		#region analytics
		continuesVideo = 0;
		continuesOrbs = 0;
		watchedReplay = false;
		watchedDoubleOrbs = false;
		#endregion

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
		if(!isGameRunning) return;

		if(!isPaused)
			PauseGame();
	}

	private void OnDoubleTap(TapGesture gesture)
	{
		if(isPaused)
			Popup.ShowYesNo(Localization.Get("QUIT_GAME"), QuitGame, null, true);
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

		bossEncounters++;

		SpawnController.SpawnBoss ();
	}

	public static void BossDied(GameObject boss)
	{
		SoundController.Instance.CrossFadeMusic(SoundController.Musics.GameTheme, 1f);
		bossTime = false;

		string bossName = boss.name;
		if(bossName.Contains(BOSS1))
		{
			Global.UnlockCreep(CreepData.CreepType.Meteor);
			boss1Killed++;
		}
		if(bossName.Contains(BOSS2))
		{
			Global.UnlockCreep(CreepData.CreepType.Twins);
			boss2Killed++;
		}
		if(bossName.Contains(BOSS3))
		{
			Global.UnlockCreep(CreepData.CreepType.Illusion);
			boss3Killed++;
		}

		#if UNITY_WEBGL
		EndDemo();
		#endif
	}

	private void OnItemCollected(Item.Type itemType, GameObject gameObject)
	{
		switch(itemType)
		{
			case Item.Type.PlasmaOrb:
				orbsCollected += gameObject.GetComponent<PlasmaOrbItem>().orbs;
			break;

			case Item.Type.LevelUp:
				levelUpCollected++;
			break;

			case Item.Type.SlowDown:

			break;

			case Item.Type.Invencibility:
				invencibilityCollected++;
			break;

			case Item.Type.DeathRay:
				deathRayCollected++;
			break;

			case Item.Type.Frozen:
				frozenCollected++;
			break;
		}

		UseItem(itemType);

		Debug.Log("Collected " + itemType.ToString());
	}

	public void SpawnItem(Item.Type itemType)
	{
		GameObject prefab = null;

		switch(itemType)
		{
			case Item.Type.LevelUp:
				prefab = ConsumablesController.Instance.levelUpSpinPrefab;
				break;

			case Item.Type.Invencibility:
				prefab = ConsumablesController.Instance.InvincibilitySpinPrefab;
				break;

			case Item.Type.DeathRay:
				prefab = ConsumablesController.Instance.deathRaySpinPrefab;
				break;

			case Item.Type.Frozen:
				prefab = ConsumablesController.Instance.frozenSpinPrefab;
				break;

			case Item.Type.Shield:
				prefab = ConsumablesController.Instance.shieldSpinPrefab;
				break;
		}

		(Instantiate(prefab, player.transform.position, Quaternion.identity) as GameObject).transform.parent = player.transform.FindChild("power-ups");

	}

	public void UseItem(Item.Type itemType)
	{
		switch(itemType)
		{
			case Item.Type.LevelUp:
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.PowerUpCollected, SoundController.SoundFX.LevelUp);
				StreakCount = LevelDesign.NextStreakToPlayerLevelUp;
			break;
				
			case Item.Type.Invencibility:
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.PowerUpCollected);
				UseInvencibility(Invencible.Time);
			break;
				
			case Item.Type.DeathRay:
				KillAllEnemies(true);
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.PowerUpCollected, SoundController.SoundFX.DeathRay);
			break;

			case Item.Type.SlowDown:

				if(OnSlowDownCollected != null)
					OnSlowDownCollected();
				
				slowedDown = true;
				
				StopCoroutine("FadeSlowDown");
				StartCoroutine("FadeSlowDown");

			break;

			case Item.Type.Frozen:
				if(OnFrozenCollected != null)
					OnFrozenCollected();
				
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.PowerUpCollected, SoundController.SoundFX.Freeze);
				
				frozen = true;
				
				ScreenFeedback.ShowFrozen(Frozen.FrozenTime);
				
				StopCoroutine("FadeFrozen");
				StartCoroutine("FadeFrozen");
			break;

			case Item.Type.Shield:
				shield = true;
				ScreenFeedback.ShowShield();
			break;
		}

		//remove form list if it is consumable
		ConsumablesController.ItemUsed(itemType);

		//ended
		if(!ConsumablesController.IsUsingConsumables)
		{}
	}

	private void UseInvencibility(float invencibleTime)
	{
		invencible = true;
		
		ScreenFeedback.ShowInvencibility(invencibleTime);
		
		StopCoroutine("FadeInvencible");
		StartCoroutine("FadeInvencible", invencibleTime);
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

	private IEnumerator FadeInvencible(float invencibleTime)
	{
		yield return new WaitForSeconds(invencibleTime);

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

	#if UNITY_WEBGL
	private static void EndDemo()
	{
		Time.timeScale = 0;

		if(OnDemoOver != null)
			OnDemoOver();
	}

	public void ShowAndroidGamePage()
	{
		Application.OpenURL("https://play.google.com/store/apps/details?id=com.overtimestudios.creepybuster");
	}

	public void ShowIOSGamePage()
	{
		Application.OpenURL("https://itunes.apple.com/app/id1060148248");
	}
	#endif
}
