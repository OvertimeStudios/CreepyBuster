using UnityEngine;
using System.Collections;

public class BlinkStars : MonoBehaviour 
{
	public RandomBetweenTwoConst visibleTime;
	public float fadeoutTime;
	public RandomBetweenTwoConst invisibleTime;
	public float fadeinTime;

	private SpriteRenderer sprite;

	// Use this for initialization
	void Start () 
	{
		sprite = GetComponent<SpriteRenderer>();

		StartCoroutine(Blink());
	}

	private IEnumerator Blink()
	{
		//visible
		Debug.Log("Visible: " + Time.time);
		yield return new WaitForSeconds(visibleTime.Random());

		//fade out
		Debug.Log("Fade Out: " + Time.time);
		while(sprite.color.a > 0)
		{
			Color c = sprite.color;
			c.a -= Time.deltaTime / fadeoutTime;
			sprite.color = c;

			yield return null;
		}

		//invisible
		Debug.Log("Invisible: " + Time.time);
		yield return new WaitForSeconds(invisibleTime.Random());
		
		//fade in
		Debug.Log("Fade In: " + Time.time);
		while(sprite.color.a < 1)
		{
			Color c = sprite.color;
			c.a += Time.deltaTime / fadeinTime;
			sprite.color = c;
			
			yield return null;
		}

		StartCoroutine(Blink());
	}
}
