using UnityEngine;
using System.Collections;

public class RangeBehaviour : MonoBehaviour 
{
	public Transform rangeTransform;
	public TweenScale rangeTween;
	public SpriteRenderer rangeSprite;
	private float originalRangeAlpha;

	// Use this for initialization
	void Start () 
	{
		originalRangeAlpha = rangeSprite.color.a;

		GameController.OnGameStart += ChangeColor;
		LevelDesign.OnPlayerLevelUp += ChangeColor;
		GameController.OnLoseStacks += ChangeColor;

		GameController.OnGameStart += ResizeRange;
	}
	
	// Update is called once per frame
	void Update()
	{
		//position plasmette under finger
		if(GameController.isGameRunning)
		{
			Vector3 pos;
			if(Input.touches.Length > 0)
			{
				pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 5));
			}
			else
			{
				pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 5));
			}

			rangeTransform.position = new Vector3(pos.x, pos.y, pos.z);

			//animate alpha on range
			if(Mathf.Abs(rangeTween.value.x) > Mathf.Abs(rangeTween.to.x - rangeTween.from.x) * 0.8f)
			{
				float total = Mathf.Abs(rangeTween.to.x - rangeTween.from.x) - (Mathf.Abs(rangeTween.to.x - rangeTween.from.x) * 0.8f);
				float part = Mathf.Abs(rangeTween.value.x) - (Mathf.Abs(rangeTween.to.x - rangeTween.from.x) * 0.8f);
				float percent = 1 - (part / total);

				Color c = rangeSprite.color;
				c.a = originalRangeAlpha * percent;
				rangeSprite.color = c;
			}
			else
			{
				Color c = rangeSprite.color;
				c.a = originalRangeAlpha;
				rangeSprite.color = c;
			}
		}
	}

	private void ChangeColor()
	{
		Color c = LevelDesign.CurrentColor;
		c.a = 0.6f;
		rangeSprite.color = c;
	}

	private void ResizeRange()
	{
		rangeTransform.localScale = new Vector3(LevelDesign.CurrentRange, LevelDesign.CurrentRange, LevelDesign.CurrentRange);
	}
}
