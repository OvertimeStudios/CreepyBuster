using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
	public UILabel score;
	public UISprite levelBar;
	public UILabel level;

	public GameObject endScreen;
	public GameObject pauseScreen;
	public GameObject demoScreen;

	#region singleton
	private static HUDController instance;
	public static HUDController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<HUDController>();

			return instance;
		}
	}
	#endregion

	void OnEnable()
	{
		MenuController.OnPanelOpened += OnScoreUpdated;
		MenuController.OnPanelOpened += OnStreakUpdated;
		MenuController.OnPanelOpened += UpdateColor;
		MenuController.OnPanelOpened += UpdateLevelNumber;

		GameController.OnScoreUpdated += OnScoreUpdated;
		GameController.OnStreakUpdated += OnStreakUpdated;
		GameController.OnShowEndScreen += ShowEndScreen;
		GameController.OnGameOver += HideEndScreen;
		LevelDesign.OnPlayerLevelUp += UpdateColor;
		LevelDesign.OnPlayerLevelUp += UpdateLevelNumber;
		GameController.OnLoseStacks += UpdateColor;
		GameController.OnLoseStacks += UpdateLevelNumber;
		GameController.OnLoseStacks += OnStreakUpdated;
		AttackTargets.OnSpecialStarted += UpdateColor;
		AttackTargets.OnSpecialEnded += UpdateColor;
		AttackTargets.OnSpecialTimerUpdated += OnSpecialTimerUpdated;

		#if UNITY_WEBGL
		GameController.OnDemoOver += ShowDemoScreen;
		#endif

		GameController.OnPause += GamePaused;
		GameController.OnResume += GameResumed;
	}

	void OnDisable()
	{
		MenuController.OnPanelOpened -= OnScoreUpdated;
		MenuController.OnPanelOpened -= OnStreakUpdated;
		MenuController.OnPanelOpened -= UpdateColor;
		MenuController.OnPanelOpened -= UpdateLevelNumber;

		GameController.OnScoreUpdated -= OnScoreUpdated;
		GameController.OnStreakUpdated -= OnStreakUpdated;
		GameController.OnShowEndScreen -= ShowEndScreen;
		GameController.OnGameOver -= HideEndScreen;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateLevelNumber;
		GameController.OnLoseStacks -= UpdateColor;
		GameController.OnLoseStacks -= UpdateLevelNumber;
		GameController.OnLoseStacks -= OnStreakUpdated;
		AttackTargets.OnSpecialStarted -= UpdateColor;
		AttackTargets.OnSpecialEnded -= UpdateColor;
		AttackTargets.OnSpecialTimerUpdated -= OnSpecialTimerUpdated;

		#if UNITY_WEBGL
		GameController.OnDemoOver -= ShowDemoScreen;
		#endif

		GameController.OnPause -= GamePaused;
		GameController.OnResume -= GameResumed;
	}

	// Use this for initialization
	void Start () 
	{
		endScreen.SetActive (false);

		score.text = GameController.Score.ToString();
	}

	void OnScoreUpdated()
	{
		score.text = GameController.Score.ToString();
	}

	void OnStreakUpdated()
	{
		if (LevelDesign.IsPlayerMaxLevel && LevelDesign.PlayerLevel != 4)
			levelBar.fillAmount = 1;
		else
			levelBar.fillAmount = ((float)GameController.StreakCount - LevelDesign.CurrentPlayerLevelUnlockStreak) / (float)LevelDesign.StreakDifferenceToNextPlayerLevel;
	}

	void OnSpecialTimerUpdated(float percent)
	{
		levelBar.fillAmount = percent;
	}

	void UpdateColor()
	{
		Color c = LevelDesign.CurrentColor;
		c.a = 1f;
		levelBar.color = c;
	}

	void UpdateLevelNumber()
	{
		level.text = Localization.Get("SPECIAL") + " " + ((LevelDesign.PlayerLevel < LevelDesign.MaxPlayerLevel) ? (LevelDesign.PlayerLevel + 1).ToString() : "MAX");
	}

	public void ShowEndScreen()
	{
		//int totalOrbsFromPoints = (int)Mathf.Floor (GameController.Score / GameController.Instance.pointsPerOrb);

		/*orbsCollected.text = GameController.orbsCollected.ToString ();
		points.text = GameController.Score.ToString ();
		orbsFromPoints.text = totalOrbsFromPoints.ToString();
		totalOrbs.text = (GameController.orbsCollected + totalOrbsFromPoints).ToString ();*/

		//Global.TotalOrbs += totalOrbsFromPoints;

		#if UNITY_WEBGL
		if(!GameController.IsBossTime)
		#endif
		endScreen.SetActive (true);
		
	}

	private void HideEndScreen()
	{
		#if UNITY_WEBGL
		demoScreen.SetActive(false);
		#endif

		endScreen.SetActive (false);
	}

	private void GamePaused()
	{
		pauseScreen.SetActive(true);

		Transform ring = pauseScreen.transform.FindChild("Ring");
		Transform arrow = pauseScreen.transform.FindChild("Arrow");
		Transform hold = pauseScreen.transform.FindChild("Hold To Continue");
		Transform doubleTap = pauseScreen.transform.FindChild("Double Tap");
		Transform paused = pauseScreen.transform.FindChild("Game Paused");
		Transform consumables = pauseScreen.transform.FindChild("Consumables");

		Vector3 playerLastPosition = AttackTargets.Instance.transform.position;
		playerLastPosition = Camera.main.WorldToViewportPoint(playerLastPosition);

		if(playerLastPosition.y > 0.7f)
			paused.position = UICamera.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, paused.position.z));
		else
			paused.position = UICamera.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.8f, paused.position.z));
		
		if(playerLastPosition.y < 0.3f)
			doubleTap.position = UICamera.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, doubleTap.position.z));
		else
			doubleTap.position = UICamera.mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.1f, doubleTap.position.z));

		float angleToCenter = Mathf.Atan2(playerLastPosition.y - 0.5f, playerLastPosition.x - 0.5f) * Mathf.Rad2Deg;

		playerLastPosition = UICamera.mainCamera.ViewportToWorldPoint(playerLastPosition);

		ring.position = playerLastPosition;

		arrow.position = playerLastPosition;
		arrow.localEulerAngles = new Vector3(0, 0, angleToCenter);

		consumables.position = playerLastPosition;
		consumables.localEulerAngles = new Vector3(0, 0, angleToCenter);
		foreach(Transform consumable in consumables)
			consumable.localEulerAngles = new Vector3(0, 0, -angleToCenter);

		hold.transform.position = playerLastPosition;
	}

	private void GameResumed()
	{
		pauseScreen.SetActive(false);
	}

	#if UNITY_WEBGL
	private void ShowDemoScreen()
	{
		demoScreen.SetActive(true);
	}
	#endif
}
