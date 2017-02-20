using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StarsGenerator : Singleton<StarsGenerator>
{
	public float vel = 0.2f;
	public float acceleration = 0.1f;

	public GameObject starPrefab;

	public int starsQuantity = 40;
	public RandomBetweenTwoConst starsScale;
	//private List<GameObject> stars;


	// Use this for initialization
	void Start () 
	{
		//stars = new List<GameObject>();

		for(byte i = 0; i < starsQuantity; i++)
		{
			Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0, 1f), Random.Range(0f, 1.2f), 0));
			pos.z = 0;
			Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360f));
			float scale = (float)starsScale.Random();

			GameObject star = Instantiate(starPrefab, pos, rot) as GameObject;
			star.transform.parent = transform;
			star.transform.localScale = new Vector3(scale, scale, scale);

			//stars.Add(star);
		}

		//stars = stars.OrderBy(star => star.transform.position.y).ToList();

		//StartCoroutine(CheckForOutOfBounds());

		Star.OnOutOfScreen += GenerateNewStar;
	}

	/*private IEnumerator CheckForOutOfBounds()
	{
		yield return new WaitForSeconds(5f);

		foreach(GameObject star in stars)
		{
			Debug.Log(star.transform.position.y);
		}
	}*/

	private void GenerateNewStar()
	{
		Debug.Log("Generate New Star");
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0, 1f), Random.Range(1.0f, 1.2f), 0));
		pos.z = 0;
		Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360f));
		float scale = (float)starsScale.Random();

		GameObject star = Instantiate(starPrefab, pos, rot) as GameObject;
		star.transform.parent = transform;
		star.transform.localScale = new Vector3(scale, scale, scale);

		//stars.Add(star);
	}
}
