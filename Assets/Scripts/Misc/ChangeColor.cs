using UnityEngine;
using System.Collections;

public class ChangeColor : MonoBehaviour 
{
	public float timeToChange;
	public Color[] colors;

	private UISprite sprite;
	private SpriteRenderer spriteRenderer;

	private int currentColor;
	private float t = 0;

	void Start()
	{
		sprite = GetComponent<UISprite>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		if(sprite != null)
			sprite.color = colors[0];
		else
			spriteRenderer.color = colors[0];

		currentColor = 1;
	}

	void Update()
	{
		if(sprite != null)
			sprite.color = Color.Lerp(LastColor, NextColor, t);
		else
			spriteRenderer.color = Color.Lerp(LastColor, NextColor, t);

		t += Time.deltaTime / timeToChange;

		if(t > 1)
		{
			currentColor++;

			if(currentColor > colors.Length - 1)
				currentColor = 0;

			t = 0;
		}
	}

	private Color LastColor
	{
		get 
		{
			return colors[currentColor];

			/*if(currentColor == 0)
				return colors[colors.Length - 1];

			return colors[currentColor - 1];*/
		}
	}

	private Color NextColor
	{
		get 
		{ 
			if(currentColor == colors.Length - 1)
				return colors[0];

			return colors[currentColor + 1];
		}
	}
}
