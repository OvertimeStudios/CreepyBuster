using UnityEngine;
using System;
using System.Collections;

public class Star : MonoBehaviour 
{
	[HideInInspector]
	public StarsGenerator generator;

	private Transform myTransform;
	private Vector3 initialPosition;
	private SpriteRenderer spriteRenderer;
	private float parallax;

	public RandomBetweenTwoConst alpha;
	public float moveDistance = 20f;

	IEnumerator Start()
	{
		myTransform = transform;
		spriteRenderer = GetComponent<SpriteRenderer>();

		yield return new WaitForEndOfFrame();

		initialPosition = myTransform.position;

		Color c = GetComponent<SpriteRenderer>().color;
		c.a = alpha.Random();
		GetComponent<SpriteRenderer>().color = c;

		while(generator == null)
			yield return null;

		//0.1 - 1
		parallax = ((transform.localScale.x / (generator.starsScale.max - generator.starsScale.min)) * 0.9f) + 0.1f;

		PlayerPosition.OnUpdated += UpdatePosition;
	}

	private void UpdatePosition(Vector2 position)
	{
		myTransform.position = initialPosition + ((Vector3)position * moveDistance * parallax);
	}
}
