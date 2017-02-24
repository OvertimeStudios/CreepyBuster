using UnityEngine;
using System;
using System.Collections;

public class Star : MonoBehaviour 
{
	#region Action
	//public static Action OnOutOfScreen;
	#endregion

	[HideInInspector]
	public StarsGenerator generator;

	private Transform myTransform;
	private float parallax;
	private float maxY;

	private float upToBottomDistance = 0;

	IEnumerator Start()
	{
		myTransform = transform;

		while(generator == null)
			yield return null;

		parallax = ((transform.localScale.x / generator.starsScale.max - generator.starsScale.min) * 0.9f) + 0.1f;

		maxY = generator.myCamera.ViewportToWorldPoint(new Vector3(0, -0.1f, 0)).y;

		if(upToBottomDistance == 0)
			upToBottomDistance = Mathf.Abs(generator.myCamera.ViewportToWorldPoint(new Vector3(0, 1.1f, 0)).y - maxY);

		//check bounds on coroutine - on update consumes too much processing
		StartCoroutine(CheckOutOfBounds());
	}

	void Update()
	{
		if(generator == null) return;

		myTransform.Translate(0, -generator.vel * parallax * Time.deltaTime, 0, Space.World);
	}

	private IEnumerator CheckOutOfBounds()
	{
		yield return new WaitForSeconds(3f);

		if(myTransform.position.y < maxY)
		{
			myTransform.position += new Vector3(0, upToBottomDistance, 0);

			//Destroy(gameObject);
		}

		StartCoroutine(CheckOutOfBounds());
	}
}
