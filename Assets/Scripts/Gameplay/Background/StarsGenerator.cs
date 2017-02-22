using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StarsGenerator : MonoBehaviour
{
	public float vel = 0.2f;
	public float acceleration = 0.1f;

	public GameObject starPrefab;
	public Camera myCamera;

	public int starsQuantity = 40;
	public RandomBetweenTwoConst starsScale;

	// Use this for initialization
	void Start () 
	{
		if(myCamera == null)
			myCamera = Camera.main;

		for(byte i = 0; i < starsQuantity; i++)
		{
			Vector3 pos = myCamera.ViewportToWorldPoint(new Vector3(Random.Range(0, 1f), Random.Range(0f, 1.2f), 0));
			pos.z = 0;
			Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360f));
			float scale = (float)starsScale.Random();

			GameObject star = Instantiate(starPrefab, pos, rot) as GameObject;
			star.transform.parent = transform;
			star.transform.localScale = new Vector3(scale, scale, scale);

			if(star.GetComponent<Star>() != null)
				star.GetComponent<Star>().generator = this;
			else
				star.GetComponent<StarMenu>().generator = this;
		}
	}

	public void GenerateNewStar()
	{
		Debug.Log("Generate New Star");
		Vector3 pos = myCamera.ViewportToWorldPoint(new Vector3(Random.Range(0, 1f), Random.Range(1.0f, 1.2f), 0));
		pos.z = 0;
		Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360f));
		float scale = (float)starsScale.Random();

		GameObject star = Instantiate(starPrefab, pos, rot) as GameObject;
		star.transform.parent = transform;
		star.transform.localScale = new Vector3(scale, scale, scale);
	}

	public void GenerateNewStarMenu()
	{
		if(this == null) return;

		Debug.Log("Generate New Star");
		Vector3 pos = myCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f), 0));
		pos.z = 0;
		Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360f));
		float scale = (float)starsScale.Random();

		GameObject star = Instantiate(starPrefab, pos, rot) as GameObject;
		star.transform.parent = transform;
		star.transform.localScale = new Vector3(scale, scale, scale);

		if(star.GetComponent<Star>() != null)
			star.GetComponent<Star>().generator = this;
		else
			star.GetComponent<StarMenu>().generator = this;
	}
}
