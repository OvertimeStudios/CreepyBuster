using UnityEngine;
using UnityEngine.Advertisements;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class MenuController : MonoBehaviour 
{
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
	
	private static Menus activeMenu;

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

	public GameObject tapAndHold;
	public GameObject hud;
	private TweenPosition wallTop;
	private TweenPosition wallBottom;
	private UILabel highScore;

	public static bool goToShop = false;

	public float timeToStartGame = 3f;
	private float timeCounter;
	private float initialTapAndHoldRotation;
	public float maxTapAndHoldRotation;

	private GameObject trailRenderer;

	private int achievementOrbsToGive;

	//ADS
	public int gamesToShowAd;
	private int gamesCount;
	
	public static float timeSpentOnMenu;

	[Header("Telas Tween")]
	public TweenPosition menuTween;
	public Transform mainScreen;
	public Transform shopScreen;
	public Transform settingsScreen;
	public Transform creditsScreen;
	public Transform hubConnectionScreen;
	public Transform achievementsScreen;
	public Transform creepypediaScreen;
	public Transform gameStatsScreen;
	public Transform howToPlayScreen;

	[Header("Menu Achievement")]
	public Achievement achievement;
	
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

	#endregion

	void OnEnable()
	{
		timeSpentOnMenu = 0;

		GameController.OnGameOver += ClosePanel;
		GameController.OnGameOver += UpdateScore;
		MenuController.OnPanelClosed += ShowAds;
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerUpEvent += OnFingerUp;
	}

	void OnDisable()
	{
		timeSpentOnMenu = 0;

		GameController.OnGameOver -= ClosePanel;
		GameController.OnGameOver -= UpdateScore;
		MenuController.OnPanelClosed -= ShowAds;
		FingerDetector.OnFingerDownEvent -= OnFingerDown;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;
	}

	// Use this for initialization
	void Start ()
	{
		instance = this;

		activeMenu = Menus.Main;

		activeScreen = mainScreen.gameObject;

		//hide all others menus
		shopScreen.gameObject.SetActive (false);
		settingsScreen.gameObject.SetActive (false);
		creditsScreen.gameObject.SetActive (false);
		hubConnectionScreen.gameObject.SetActive (false);
		achievementsScreen.gameObject.SetActive (false);
		creepypediaScreen.gameObject.SetActive (false);
		gameStatsScreen.gameObject.SetActive (false);

		wallTop = mainScreen.FindChild ("WallTop").GetComponent<TweenPosition> ();
		wallBottom = mainScreen.FindChild ("WallBottom").GetComponent<TweenPosition> ();
		highScore = wallTop.transform.FindChild ("High Score").FindChild ("Score").GetComponent<UILabel> ();

		initialTapAndHoldRotation = tapAndHold.GetComponent<Rotate> ().rotVel;

		timeCounter = timeToStartGame;

		hud.SetActive (false);
		UpdateScore ();

		if(!Global.sentOnEnterMenu)
		{
			UnityAnalyticsHelper.EnterOnMenu();
			Global.sentOnEnterMenu = true;
		}
	}

	void OnFingerDown(FingerDownEvent e)
	{
		/*if(!wallTop.enabled)
		{
			if(e.Selection)
			{
				StopCoroutine("CountdownAborted");
				StartCoroutine("CountdownBeginGame", e.Selection);
			}
		}*/
	}

	void Update()
	{
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.Escape))
		{
			Global.ClearPurchasedOnly();
		}

		if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.Escape))
		{
			Global.Reset();
		}

		//game stats
		timeSpentOnMenu += Time.deltaTime;

		if(!achievement.unlocked && timeSpentOnMenu >= achievement.value)
		{
			achievement.Unlock();
			ShowAchievements();
		}
	}

	IEnumerator CountdownBeginGame(GameObject selection)
	{
		timeCounter = timeToStartGame;
		//float maxY = selection.transform.GetChild (0).localPosition.y;
		trailRenderer = selection;

		Rotate rotate = tapAndHold.GetComponent<Rotate> ();

		while(timeCounter > 0)
		{
			timeCounter -= Time.deltaTime;

			rotate.rotVel = initialTapAndHoldRotation + ((maxTapAndHoldRotation - initialTapAndHoldRotation) * ((timeToStartGame - timeCounter) / timeCounter));

			yield return null;
		}

		rotate.rotVel = initialTapAndHoldRotation;
		trailRenderer.SetActive(false);


		OpenPanel();
	}

	void OnFingerUp(FingerUpEvent e)
	{
		/*if(!wallTop.enabled && timeCounter < timeToStartGame)
		{
			StopCoroutine("CountdownBeginGame");
			StartCoroutine("CountdownAborted");
		}*/
	}

	private IEnumerator CountdownAborted()
	{
		Rotate rotate = tapAndHold.GetComponent<Rotate> ();

		while(timeCounter < timeToStartGame)
		{
			timeCounter += Time.deltaTime;
			
			rotate.rotVel = initialTapAndHoldRotation + ((maxTapAndHoldRotation - initialTapAndHoldRotation) * ((timeToStartGame - timeCounter) / timeCounter));
			
			yield return null;
		}
	}

	public void TweenFinished()
	{
		wallTop.enabled = wallBottom.enabled = false;

		if(wallTop.direction == AnimationOrTween.Direction.Forward)
		{
			GameController.Instance.StartGame ();

			hud.SetActive (true);

			if(OnPanelOpened != null)
				OnPanelOpened();

			gameObject.SetActive(false);
		}
		else
		{
			if(trailRenderer != null)
				trailRenderer.SetActive(true);

			SoundController.Instance.CrossFadeMusic(SoundController.Musics.MainMenuTheme, 1f);

			Time.timeScale = 1;

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
		List<AchievementUnlocked> achievements = Achievement.achievementRecentUnlocked;

		if(achievements.Count > 0)
		{
			AchievementUnlocked a = achievements[0];
			achievementOrbsToGive = a.orbReward;

			Popup.ShowOk(string.Format(Localization.Get("ACHIEVEMENT_UNLOCKED"), a.title, a.orbReward), GiveAchievementOrbs);

			Achievement.achievementRecentUnlocked.Remove(a);
		}
	}

	private void GiveAchievementOrbs()
	{
		Global.TotalOrbs += achievementOrbsToGive;

		ShowAchievements();
	}

	public void OpenPanel()
	{
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
		
		activeMenu = Menus.Main;
		
		MoveScreen ();
	}

	public void MoveToShop()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuIn);

		ActiveScreen = shopScreen.gameObject;

		activeMenu = Menus.Shop;

		MoveScreen ();
	}

	public void MoveToSettings()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = settingsScreen.gameObject;

		activeMenu = Menus.Settings;
		
		MoveScreen (true);
	}

	public void MoveToCredits()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = creditsScreen.gameObject;

		activeMenu = Menus.Settings;

		MoveScreen (true);
	}

	public void MoveToHowToPlay()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = howToPlayScreen.gameObject;
		
		activeMenu = Menus.HowToPlay;
		
		MoveScreen (true);
	}

	public void MoveToHUBConnection()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.MenuIn);

		ActiveScreen = hubConnectionScreen.gameObject;
		
		activeMenu = Menus.HUBConnection;
		
		MoveScreen ();
	}

	public void MoveToAchievements()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = achievementsScreen.gameObject;
		
		activeMenu = Menus.Achievements;
		
		MoveScreen (true);
	}

	public void MoveToCreepypedia()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = creepypediaScreen.gameObject;
		
		activeMenu = Menus.Creepypedia;
		
		MoveScreen (true);
	}

	public void MoveToGameStats()
	{
		if(menuTween.isActiveAndEnabled) return;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		ActiveScreen = gameStatsScreen.gameObject;
		
		activeMenu = Menus.GameStats;
		
		MoveScreen (true);
	}

	public void MoveInstantToMainMenu()
	{
		ActiveScreen = mainScreen.gameObject;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		activeMenu = Menus.Main;
		
		MoveScreen (true);
	}

	public void CloseScreen()
	{
		ActiveScreen = lastScreen;

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

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
		lastScreen.SetActive (false);
	}

	private void ShowAds()
	{
		if(Global.IsAdFree) return;

		gamesCount++;

		if(gamesCount % gamesToShowAd == 0)
			UnityAdsHelper.ShowAd();
	}

	public void MoreGames()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		Application.OpenURL ("http://www.overtimestudios.com/games.php");
	}

	public void FacebookLogin()
	{

	}

	public void FacebookLogout()
	{

	}
}
