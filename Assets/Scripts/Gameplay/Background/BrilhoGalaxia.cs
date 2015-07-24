using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrilhoGalaxia : MonoBehaviour 
{
	private List<Collider2D> objectsInFront;
	GameObject player;

	void OnEnable()
	{
		EnemyLife.OnDied += RemoveEnemy;
		GameController.OnGameOver += RemovePlayer;
		MenuController.OnPanelClosed += Reset;
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= RemoveEnemy;
		GameController.OnGameOver -= RemovePlayer;
		MenuController.OnPanelClosed -= Reset;
	}

	void Awake()
	{
		objectsInFront = new List<Collider2D> ();
	}

	void Start()
	{
		StartCoroutine (WaitForPlayer ());
	}

	private IEnumerator WaitForPlayer()
	{
		while (GameObject.FindWithTag ("Player") == null)
			yield return null;

		player = GameObject.FindWithTag ("Player");
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
		foreach(Collider2D col in player.GetComponents<Collider2D>())
		{
			if(objectsInFront.Contains(col))
				objectsInFront.Remove(col);
		}

		if(objectsInFront.Count == 0)
			GetComponent<SpriteRenderer> ().enabled = true;
	}

	void Reset()
	{
		objectsInFront = new List<Collider2D> ();
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
