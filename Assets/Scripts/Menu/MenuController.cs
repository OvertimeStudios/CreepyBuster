using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;
using UnityEngine.Advertisements;

public class MenuController : MonoBehaviour 
{
	#region Action
	public static Action OnMenuAchievementUnlocked;
	#endregion

	public enum Menus
	{
		None,
		Main,
		Shop,
		Settings,
		Credits,
		HUBConnection,
		Achievements,
		Creepypedia,
		GameStats,
		HowToPlay,
	}
	
	public static Menus activeMenu;
	private static Menus lastMenu;

	private static GameObject lastScreen;
	private static GameObject activeScreen;

	/// <summary>
	/// Occurs when on panel fully open.
	/// </summary>
	public static event Action OnPanelOpened;

	/// <summary>
	/// Occurs when on panel starts to open.
	/// </summary>
	public static event Action OnPanelOpening;

	/// <summary>
	/// Occurs when on panel is fully closed.
	/// </summary>
	public static event Action OnPanelClosed;

	/// <summary>
	/// Occurs when on panel starts to close.
	/// </summary>
	public static event Action OnPanelClosing;

	public GameObject menu;
	public GameObject hud;
	private TweenPosition wallTop;
	private TweenPosition wallBottom;
	private UILabel highScore;

	public static bool goToShop = false;
	private static bool isMenuActive = true;

	private int achievementOrbsToGive;
	private int dailyMissionOrbsToGive;

	//ADS
	public int gamesToShowAd;
	private int gamesCount;

	//Rate
	public int gamesToShowRate;
	
	public static float timeSpentOnMenu;

	[Header("Telas Tween")]
	public TweenPosition menuTween;
	public Transform mainScreen;
	public Transform shopScreen;
	public Transform settingsScreen;
	public Transform creditsScreen;
	public Transform hubConnectionScreen;
	public Transform creepypediaScreen;
	public Transform gameStatsScreen;
	public Transform howToPlayScreen;

	[Header("Menu Achievement")]
	public int menuAchievementValue = 600;

	[Header("Web")]
	public GameObject playPlayFun;
	public GameObject shopButton;
	public GameObject overtimeHubButton;
	public GameObject highScoreMenu;
	public GameObject shopGO;
	public GameObject overtimeHubGO;

	#region singleton
	private static MenuController instance;
	public static MenuController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<MenuController>();

			return instance;
		}
	}
	#endregion

	#region get / set
	public GameObject ActiveScreen
	{
		get { return activeScreen; }

		set
		{
			lastScreen = activeScreen;

			activeScreen = value;
		}
	}

	public static Menus CurrentMenu
	{
		get { return activeMenu; }
	}

	public static bool IsMenuActive
	{
		get { return isMenuActive; }
	}
	#endregion

	void OnEnable()
	{
		timeSpentOnMenu = 0;

		GameController.OnGameOver += ClosePanel;
		GameController.OnGameOver += UpdateScore;
		MenuController.OnPanelClosed += ShowAds;
		MenuController.OnPanelClosed += ShowRate;

		Global.OnHighScoreUpdated += UpdateScore;

		LeaderboardsHelper.OnPlayerAuthenticated += SendFirstScore;
	}

	void OnDisable()
	{
		timeSpentOnMenu = 0;

		GameController.OnGameOver -= ClosePanel;
		GameController.OnGameOver -= UpdateScore;
		MenuController.OnPanelClosed -= ShowAds;
		MenuController.OnPanelClosed -= ShowRate;

		Global.OnHighScoreUpdated -= UpdateScore;

		LeaderboardsHelper.OnPlayerAuthenticated -= SendFirstScore;
	}

	private void SendFirstScore()
	{
		//HACK: the first time player enter game, he doesn't have any registered score on leaderboard. So, entry a 0 value.
		LeaderboardsHelper.SendScore(Global.HighScore);

		//StartCoroutine(GetUserScore());
	}

	private IEnumerator GetUserScore()
	{
		long score = 0;
		yield return StartCoroutine(LeaderboardsHelper.GetUserScore(value => score = value));

		if((int)score > Global.HighScore)
			Global.HighScore = (int)score;
	}

	// Use this for initialization
	IEnumerator Start ()
	{
		#if !UNITY_WEBPLAYER || !PLAYPLAYFUN
		playPlayFun.SetActive(false);
		#endif

		#if UNITY_WEBPLAYER
		shopButton.SetActive(false);
		overtimeHubButton.SetActive(false);
		highScoreMenu.SetActive(false);
		shopGO.SetActive(false);
		overtimeHubGO.SetActive(false);
		#endif

		instance = this;

		activeMenu = Menus.Main;
		lastMenu = Menus.None;

		activeScreen = mainScreen.gameObject;

		//hide all others menus
		settingsScreen.gameObject.SetActive (false);
		creditsScreen.gameObject.SetActive (false);
		creepypediaScreen.gameObject.SetActive (false);
		gameStatsScreen.gameObject.SetActive (false);
		howToPlayScreen.gameObject.SetActive(false);

		wallTop = mainScreen.FindChild ("WallTop").GetComponent<TweenPosition> ();
		wallBottom = mainScreen.FindChild ("WallBottom").GetComponent<TweenPosition> ();
		highScore = wallTop.transform.FindChild ("High Score").FindChild ("Score").GetComponent<UILabel> ();

		hud.SetActive (false);
		UpdateScore ();

		#if UNITYANALYTICS_IMPLEMENTED
		if(!Global.sentOnEnterMenu)
		{
			UnityAnalyticsHelper.EnterOnMenu();
			Global.sentOnEnterMenu = true;
		}
		#endif

		yield return new WaitForSeconds(0.3f);

		hubConnectionScreen.gameObject.SetActive (false);
		shopScreen.gameObject.SetActive (false);
	}

	void Update()
	{
		if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Escape))
		{
			Global.Reset();
		}

		if(!Global.IsAchievementUnlocked(Achievement.Type.Menu, menuAchievementValue))
		{
			//game stats
			timeSpentOnMenu += Time.deltaTime;

			if(timeSpentOnMenu >= menuAchievementValue)
			{
				if(OnMenuAchievementUnlocked != null)
					OnMenuAchievementUnlocked();

				Global.UnlockAchievement(Achievement.Type.Menu, menuAchievementValue);
			}
		}
	}

	public void TweenFinished()
	{
		wallTop.enabled = wallBottom.enabled = false;

		if(wallTop.direction == AnimationOrTween.Direction.Forward)
		{
			GameController.Instance.StartGame ();

			hud.SetActive (true);
			menu.SetActive (false);

			if(OnPanelOpened != null)
				OnPanelOpened();

			gameObject.SetActive(false);
		}
		else
		{
			SoundController.Instance.CrossFadeMusic(SoundController.Musics.MainMenuTheme, 1f);

			Time.timeScale = 1;

			gamesCount++;

			isMenuActive = true;
			if(OnPanelClosed != null)
				OnPanelClosed();

			ShowAchievements();

			if(goToShop)
			{
				MoveToShop();
				goToShop = false;
			}
		}
	}

	public void ShowAchievements()
	{
		#if !UNITY_WEBPLAYER
		List<AchievementUnlocked> achievements = Achievement.achievementRecentUnlocked;

		if(achievements.Count > 0)
		{
			AchievementUnlocked a = achievements[0];
			achievementOrbsToGive = a.orbReward;

			SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Achievement);

			Popup.ShowOk(string.Format(Localization.Get("ACHIEVEMENT_UNLOCKED"), a.title, a.orbReward), GiveAchievementOrbs);

			Achievement.achievementRecentUnlocked.Remove(a);
		}
		else
			ShowDailyMissions();
		#endif
	}

	private void GiveAchievementOrbs()
	{
		Global.TotalOrbs += achievementOrbsToGive;

		ShowAchievements();
	}

	public void ShowDailyMissions()
	{
		#if !UNITY_WEBPLAYER
		List<DailyMission> dailyMission = DailyMissionController.missionRecentUnlocked;
		
		if(dailyMission.Count > 0)
		{
			DailyMission dm = dailyMission[0];
			dailyMissionOrbsToGive = dm.reward;
			
			SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Achievement);
			
			Popup.ShowOk(string.Format(Localization.Get("DAILYMISSION_COMPLETED"), dm.Description, dm.reward), GiveDailyMissionOrbs);
			
			DailyMissionController.missionRecentUnlocked.Remove(dm);
		}
		#endif
	}
	
	private void GiveDailyMissionOrbs()
	{
		Debug.Log(dailyMissionOrbsToGive);
		Global.TotalOrbs += dailyMissionOrbsToGive;
		
		ShowDailyMissions();
	}

	public void OpenPanel()
	{
		isMenuActive = false;

		wallTop.enabled = wallBottom.enabled = true;
		
		wallTop.PlayForward();
		wallBottom.PlayForward();

		//SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuOut);

		if(OnPanelOpening != null)
			OnPanelOpening();
	}

	public void ClosePanel()
	{
		wallTop.enabled = wallBottom.enabled = true;
		
		wallTop.PlayReverse();
		wallBottom.PlayReverse();

		hud.SetActive (false);
		menu.SetActive (true);

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuIn);

		if (OnPanelClosing != null)
			OnPanelClosing ();
	}

	private void UpdateScore()
	{
		highScore.text = Global.HighScore.ToString ();
	}

	public void MoveToMain()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuIn);

		ActiveScreen = mainScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.Main;
		
		MoveScreen ();
	}

	public void MoveToShop()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive || Plasmette.IsSpinning) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuIn);

		ActiveScreen = shopScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.Shop;

		shopScreen.gameObject.SetActive(false);
		shopScreen.gameObject.SetActive(true);

		MoveScreen ();
	}

	public void MoveToSettings()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive || Plasmette.IsSpinning) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = settingsScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.Settings;

		settingsScreen.gameObject.SetActive(false);
		settingsScreen.gameObject.SetActive(true);

		MoveScreen (true);
	}

	public void MoveToCredits()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = creditsScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.Credits;

		MoveScreen (true);
	}

	public void MoveToHowToPlay()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = howToPlayScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.HowToPlay;
		
		MoveScreen (true);
	}

	public void MoveToHUBConnection()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive || Plasmette.IsSpinning) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuIn);

		ActiveScreen = hubConnectionScreen.gameObject;

		hubConnectionScreen.gameObject.SetActive(false);
		hubConnectionScreen.gameObject.SetActive(true);

		lastMenu = activeMenu;
		activeMenu = Menus.HUBConnection;
		
		MoveScreen ();
	}

	public void MoveToAchievements()
	{
		#if ACHIEVEMENTS_IMPLEMENTED
		if(AchievementsHelper.IsPlayerAuthenticated())
			AchievementsHelper.OpenAchievements();
		#endif
	}

	public void MoveToCreepypedia()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = creepypediaScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.Creepypedia;
		
		MoveScreen (true);
	}

	public void MoveToGameStats()
	{
		if(menuTween.isActiveAndEnabled || DailyRewardController.IsActive || Popup.IsActive) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = gameStatsScreen.gameObject;

		lastMenu = activeMenu;
		activeMenu = Menus.GameStats;
		
		MoveScreen (true);
	}

	public void MoveInstantToMainMenu()
	{
		ActiveScreen = mainScreen.gameObject;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		lastMenu = activeMenu;
		activeMenu = Menus.Main;
		
		MoveScreen (true);
	}

	public void CloseScreen()
	{
		ActiveScreen = lastScreen;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		Menus lastlastMenu = activeMenu;
		activeMenu = lastMenu;
		lastMenu = lastlastMenu;

		MoveScreen(true);
	}

	public void MoveScreen()
	{
		MoveScreen(false);
	}

	public void MoveScreen(bool instant)
	{
		ActiveScreen.SetActive (true);

		Vector3 from = menuTween.transform.localPosition;
		Vector3 to = -ActiveScreen.transform.localPosition;

		if(activeMenu == Menus.Shop || activeMenu == Menus.HUBConnection)
			to.x *= 2f;

		if(instant)
		{
			menuTween.transform.localPosition = to;
			OnMenuTransitionFinished();
		}
		else
		{
			menuTween.ResetToBeginning ();
			
			menuTween.from = from;
			menuTween.to = to;
			
			menuTween.PlayForward ();
		}
	}

	public void OnMenuTransitionFinished()
	{
		//if(activeMenu == Menus.HUBConnection || activeMenu == Menus.Main || activeMenu == Menus.Shop) return;

		lastScreen.SetActive (false);
	}

	private void ShowAds()
	{
		if(Global.IsAdFree) return;

		#if ADMOB_IMPLEMENTED
		Debug.Log(string.Format("Try to show ads (gamesCount % gamesToShowAd == 0 && AdsHelper.IsInterstitialReady) = {0} % {1} = {2} && {3}", gamesCount, gamesToShowAd, (gamesCount % gamesToShowAd), AdMobHelper.IsInterstitialReady));
		if(gamesCount % gamesToShowAd == 0 && AdMobHelper.IsInterstitialReady)
			AdMobHelper.ShowInterstitial();
		#endif
	}

	private void ShowRate()
	{
		if(Global.Rated) return;

		if(Global.GamesPlayed % gamesToShowRate == 0)
			Popup.ShowYesNo(Localization.Get("RATE_US"), OpenStoreToRate, null);
	}

	private void OpenStoreToRate()
	{
		Global.Rated = true;

		#if UNITY_ANDROID
		Application.OpenURL("market://details?id=com.overtimestudios.creepybuster");
		#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
		#endif
	}

	public void MoreGames()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		#if UNITY_ANDROID
		Application.OpenURL("https://play.google.com/store/apps/dev?id=8938813649462154472");
		//Application.OpenURL("market://dev?id=8938813649462154472");
		#elif UNITY_IPHONE
		//TODO: change to developer page
		Application.OpenURL("https://itunes.apple.com/app/id1060148248");
		#elif UNITY_WEBPLAYER
		Application.OpenURL("http://www.overtimestudios.com/games.php");
		#endif
	}

	public void FacebookLogin()
	{

	}

	public void FacebookLogout()
	{

	}

	public void AskApplicationQuit()
	{
		Popup.ShowYesNo(Localization.Get("QUIT_GAME"), ApplicationQuit, null);
	}

	private void ApplicationQuit()
	{
		Application.Quit();
	}
}
