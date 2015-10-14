using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorChange : MonoBehaviour 
{
	public List<ColorPoints> colors;

	private SpriteRenderer sprite;

	private int level;

	#region get / set
	private Color CurrentColor
	{
		get { return colors[level].color; }
	}

	private Color NextColor
	{
		get { return colors[level + 1].color; }
	}

	private bool IsMaxLevel
	{
		get { return level == colors.Count - 1; }
	}

	private float NextLevelScore
	{
		get { return colors[level + 1].points; }
	}

	private float CurrentScore
	{
		get { return colors[level].points; }
	}
	#endregion

	void OnEnable()
	{
		GameController.OnScoreUpdated += UpdateColor;
	}

	void OnDisable()
	{
		GameController.OnScoreUpdated -= UpdateColor;
	}

	void Start()
	{
		level = 0;

		sprite = GetComponent<SpriteRenderer>();

		UpdateColor();
	}

	private void UpdateColor()
	{
		if(IsMaxLevel) return;

		if(GameController.Score > NextLevelScore)
			level++;

		sprite.color = (IsMaxLevel) ? CurrentColor : Color.Lerp(CurrentColor, NextColor, (GameController.Score - CurrentScore) / (NextLevelScore - CurrentScore));
	}
}

[System.Serializable]
public class ColorPoints
{
	public int points;
	public Color color = Color.white;
}