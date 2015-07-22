using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
	public UILabel score;
	public UISprite levelBar;
	public UILabel level;

	public GameObject endScreen;

	private UILabel orbsCollected;
	private UILabel points;
	private UILabel orbsFromPoints;
	private UILabel totalOrbs;

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
		GameController.OnReset += OnScoreUpdated;
		GameController.OnReset += OnStreakUpdated;
		GameController.OnReset += UpdateColor;
		GameController.OnReset += UpdateLevelNumber;
		GameController.OnScoreUpdated += OnScoreUpdated;
		GameController.OnStreakUpdated += OnStreakUpdated;
		GameController.OnGameOver += HideEndScreen;
		LevelDesign.OnPlayerLevelUp += UpdateColor;
		LevelDesign.OnPlayerLevelUp += UpdateLevelNumber;
		GameController.OnLoseStacks += UpdateColor;
		GameController.OnLoseStacks += UpdateLevelNumber;
		AttackTargets.OnSpecialTimerUpdated += OnSpecialTimerUpdated;
	}

	void OnDisable()
	{
		GameController.OnReset -= OnScoreUpdated;
		GameController.OnReset -= OnStreakUpdated;
		GameController.OnReset -= UpdateColor;
		GameController.OnReset -= UpdateLevelNumber;
		GameController.OnScoreUpdated -= OnScoreUpdated;
		GameController.OnStreakUpdated -= OnStreakUpdated;
		GameController.OnGameOver -= HideEndScreen;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateLevelNumber;
		GameController.OnLoseStacks -= UpdateColor;
		GameController.OnLoseStacks -= UpdateLevelNumber;
		AttackTargets.OnSpecialTimerUpdated -= OnSpecialTimerUpdated;
	}

	// Use this for initialization
	void Start () 
	{
		orbsCollected = endScreen.transform.FindChild ("Orbs Collected").FindChild ("Value").GetComponent<UILabel> ();
		points = endScreen.transform.FindChild ("Points").FindChild ("Value").GetComponent<UILabel> ();
		orbsFromPoints = endScreen.transform.FindChild ("Orbs from points").FindChild ("Value").GetComponent<UILabel> ();
		totalOrbs = endScreen.transform.FindChild ("Total Orbs").FindChild ("Value").GetComponent<UILabel> ();

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
		level.text = "Level " + ((LevelDesign.PlayerLevel < LevelDesign.MaxPlayerLevel) ? (LevelDesign.PlayerLevel + 1).ToString() : "MAX");
	}

	public void ShowEndScreen()
	{
		int totalOrbsFromPoints = (int)Mathf.Floor (GameController.Score / GameController.Instance.pointsPerOrb);

		orbsCollected.text = GameController.orbsCollected.ToString ();
		points.text = GameController.Score.ToString ();
		orbsFromPoints.text = totalOrbsFromPoints.ToString();
		totalOrbs.text = (GameController.orbsCollected + totalOrbsFromPoints).ToString ();

		Global.TotalOrbs += totalOrbsFromPoints;

		endScreen.SetActive (true);
	}

	private void HideEndScreen()
	{
		endScreen.SetActive (false);
	}
}
