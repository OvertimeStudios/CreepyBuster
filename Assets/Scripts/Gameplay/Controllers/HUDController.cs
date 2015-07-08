using UnityEngine;
using System.Collections;

public class HUDController : MonoBehaviour 
{
	public UILabel score;
	public UISprite levelBar;
	public UILabel level;

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
		MenuController.OnPanelOpening += OnScoreUpdated;
		MenuController.OnPanelOpening += OnStreakUpdated;
		MenuController.OnPanelOpening += UpdateColor;
		MenuController.OnPanelOpening += UpdateLevelNumber;
		GameController.OnScoreUpdated += OnScoreUpdated;
		GameController.OnStreakUpdated += OnStreakUpdated;
		LevelDesign.OnPlayerLevelUp += UpdateColor;
		LevelDesign.OnPlayerLevelUp += UpdateLevelNumber;
		GameController.OnLoseStacks += UpdateColor;
		GameController.OnLoseStacks += UpdateLevelNumber;
		AttackTargets.OnSpecialTimerUpdated += OnSpecialTimerUpdated;
	}

	void OnDisable()
	{
		MenuController.OnPanelOpening -= OnScoreUpdated;
		MenuController.OnPanelOpening -= OnStreakUpdated;
		MenuController.OnPanelOpening -= UpdateColor;
		MenuController.OnPanelOpening -= UpdateLevelNumber;
		GameController.OnScoreUpdated -= OnScoreUpdated;
		GameController.OnStreakUpdated -= OnStreakUpdated;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateLevelNumber;
		GameController.OnLoseStacks -= UpdateColor;
		GameController.OnLoseStacks -= UpdateLevelNumber;
		AttackTargets.OnSpecialTimerUpdated -= OnSpecialTimerUpdated;
	}

	// Use this for initialization
	void Start () 
	{
		score.text = GameController.Score.ToString();
	}

	void OnScoreUpdated()
	{
		score.text = GameController.Score.ToString();
	}

	void OnStreakUpdated()
	{
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

	public void Funcao()
	{

	}
}
