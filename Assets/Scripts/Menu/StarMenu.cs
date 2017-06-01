using UnityEngine;
using System;
using System.Collections;

public class StarMenu : MonoBehaviour 
{
	#region Action
	//public static Action OnOutOfScreen;
	#endregion

	[HideInInspector]
	public StarsGenerator generator;

	private Transform myTransform;

	private float minY;
	private float maxY;
	private float minX;
	private float maxX;

	private float velx;
	private float vely;
	private float velScale;

	void OnEnable()
	{
		StartCoroutine(CheckOutOfBounds());
	}

	void OnDisable()
	{
		StopAllCoroutines();
	}

	IEnumerator Start()
	{
		myTransform = transform;

		yield return new WaitForEndOfFrame();

		maxX = generator.myCamera.ViewportToWorldPoint(new Vector3(1.1f, 0, 0)).x;
		minX = generator.myCamera.ViewportToWorldPoint(new Vector3(-0.1f, 0, 0)).x;

		maxY = generator.myCamera.ViewportToWorldPoint(new Vector3(0, 1.1f, 0)).y;
		minY = generator.myCamera.ViewportToWorldPoint(new Vector3(0, -0.1f, 0)).y;

		Vector3 centerPosition = generator.myCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
		float angle = Mathf.Atan2(myTransform.position.y - centerPosition.y, myTransform.position.x - centerPosition.x);

		velx = generator.vel * Mathf.Cos(angle);
		vely = generator.vel * Mathf.Sin(angle);
		velScale = Mathf.Min(velx, vely) * 0.3f;

		//check bounds on coroutine - on update consumes too much processing
		StartCoroutine(CheckOutOfBounds());
	}

	void Update()
	{
		myTransform.Translate(velx * Time.deltaTime, vely * Time.deltaTime, 0, Space.World);
		myTransform.localScale += new Vector3(velScale, velScale) * Time.deltaTime;
	}

	private IEnumerator CheckOutOfBounds()
	{
		yield return new WaitForSeconds(1f);

		if(myTransform.position.y > maxY || myTransform.position.y < minY ||
			myTransform.position.x > maxX || myTransform.position.x < minX)
		{
			generator.GenerateNewStarMenu();

			Destroy(gameObject);
		}

		StartCoroutine(CheckOutOfBounds());
	}
}
