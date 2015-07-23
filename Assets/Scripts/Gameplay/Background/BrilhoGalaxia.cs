using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrilhoGalaxia : MonoBehaviour 
{
	private List<Collider2D> objectsInFront;

	void OnEnable()
	{
		EnemyLife.OnDied += RemoveEnemy;
		GameController.OnGameOver += RemovePlayer;
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= RemoveEnemy;
		GameController.OnGameOver -= RemovePlayer;
	}

	void Awake()
	{
		objectsInFront = new List<Collider2D> ();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.isTrigger) return;

		if (!objectsInFront.Contains (col))
			objectsInFront.Add (col);

		if(objectsInFront.Count > 0)
			GetComponent<SpriteRenderer> ().enabled = false;
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (objectsInFront.Contains (col))
			objectsInFront.Remove (col);

		if(objectsInFront.Count == 0)
			GetComponent<SpriteRenderer> ().enabled = true;
	}

	void RemoveEnemy(GameObject obj)
	{
		if (objectsInFront.Contains (obj.transform.FindChild("collider").GetComponent<Collider2D>()))
			objectsInFront.Remove (obj.transform.FindChild("collider").GetComponent<Collider2D>());

		if(objectsInFront.Count == 0)
			GetComponent<SpriteRenderer> ().enabled = true;
	}

	void RemovePlayer()
	{
		GameObject player = GameObject.FindWithTag ("Player");

		foreach(Collider2D col in player.GetComponents<Collider2D>())
		{
			if(objectsInFront.Contains(col))
				objectsInFront.Remove(col);
		}

		if(objectsInFront.Count == 0)
			GetComponent<SpriteRenderer> ().enabled = true;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("List of objects in front of galaxy: (" + objectsInFront.Count + ")");
			foreach(Collider2D col in objectsInFront)
			{
				Debug.Log(col.gameObject.name);
			}
		}
	}
}
