using UnityEngine;
using System;
using System.Collections;

public class Star : MonoBehaviour 
{
	#region Action
	public static Action OnOutOfScreen;
	#endregion

	private Transform myTransform;
	private float parallax;
	private float maxY;
	private float velx;

	void Start()
	{
		myTransform = transform;

		parallax = ((transform.localScale.x / StarsGenerator.Instance.starsScale.max - StarsGenerator.Instance.starsScale.min) * 0.9f) + 0.1f;

		//velx = UnityEngine.Random.Range(-0.3f, 0.3f);
		maxY = Camera.main.ViewportToWorldPoint(new Vector3(0, -0.2f, 0)).y;

		StartCoroutine(CheckOutOfBounds());
	}

	void Update()
	{
		myTransform.Translate(velx * Time.deltaTime, -StarsGenerator.Instance.vel * parallax * Time.deltaTime, 0, Space.World);

		/*Debug.Log(myTransform.position.y + " > " + maxY);
		if(myTransform.position.y < maxY)
		{
			if(OnOutOfScreen != null)
				OnOutOfScreen();

			Destroy(gameObject);
		}*/
	}

	private IEnumerator CheckOutOfBounds()
	{
		yield return new WaitForSeconds(3f);

		if(myTransform.position.y < maxY)
		{
			if(OnOutOfScreen != null)
				OnOutOfScreen();

			Destroy(gameObject);
		}

		StartCoroutine(CheckOutOfBounds());
	}
}
