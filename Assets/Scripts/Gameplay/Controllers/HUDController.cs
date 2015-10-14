using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
	public UILabel score;
	public UISprite levelBar;
	public UILabel level;

	public GameObject endScreen;
	public GameObject pauseScreen;

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
		OnScoreUpdated ();
		OnStreakUpdated ();
		UpdateColor ();
		UpdateLevelNumber ();

		MenuController.OnPanelOpened += OnScoreUpdated;
		MenuController.OnPanelOpened += OnStreakUpdated;
		MenuController.OnPanelOpened += UpdateColor;
		MenuController.OnPanelOpened += UpdateLevelNumber;

		GameController.OnScoreUpdated += OnScoreUpdated;
		GameController.OnStreakUpdated += OnStreakUpdated;
		GameController.OnGameOver += HideEndScreen;
		LevelDesign.OnPlayerLevelUp += UpdateColor;
		LevelDesign.OnPlayerLevelUp += UpdateLevelNumber;
		GameController.OnLoseStacks += UpdateColor;
		GameController.OnLoseStacks += UpdateLevelNumber;
		GameController.OnLoseStacks += OnStreakUpdated;
		AttackTargets.OnSpecialStarted += UpdateColor;
		AttackTargets.OnSpecialEnded += UpdateColor;
		AttackTargets.OnSpecialTimerUpdated += OnSpecialTimerUpdated;

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
		GameController.OnGameOver -= HideEndScreen;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateLevelNumber;
		GameController.OnLoseStacks -= UpdateColor;
		GameController.OnLoseStacks -= UpdateLevelNumber;
		GameController.OnLoseStacks -= OnStreakUpdated;
		AttackTargets.OnSpecialStarted -= UpdateColor;
		AttackTargets.OnSpecialEnded -= UpdateColor;
		AttackTargets.OnSpecialTimerUpdated -= OnSpecialTimerUpdated;

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

		endScreen.SetActive (true);
	}

	private void HideEndScreen()
	{
		endScreen.SetActive (false);
	}

	private void GamePaused()
	{
		pauseScreen.SetActive(true);

		Transform ring = pauseScreen.transform.FindChild("Ring");
		Transform arrow = pauseScreen.transform.FindChild("Arrow");
		Transform label = pauseScreen.transform.FindChild("Label");

		Vector3 playerLastPosition = AttackTargets.Instance.transform.position;
		playerLastPosition = Camera.main.WorldToViewportPoint(playerLastPosition);

		float angleToCenter = Mathf.Atan2(playerLastPosition.y - 0.5f, playerLastPosition.x - 0.5f) * Mathf.Rad2Deg;

		playerLastPosition = UICamera.mainCamera.ViewportToWorldPoint(playerLastPosition);

		ring.position = playerLastPosition;

		arrow.position = playerLastPosition;
		arrow.localEulerAngles = new Vector3(0, 0, angleToCenter);

		label.transform.position = playerLastPosition;
		label.localEulerAngles = new Vector3(0, 0, angleToCenter);
		label.GetChild(0).localEulerAngles = new Vector3(0, 0, -angleToCenter);
	}

	private void GameResumed()
	{
		pauseScreen.SetActive(false);
	}
}
