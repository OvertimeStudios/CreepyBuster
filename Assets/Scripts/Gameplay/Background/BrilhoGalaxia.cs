using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrilhoGalaxia : MonoBehaviour 
{
	private List<Collider2D> objectsInFront;

	void OnEnable()
	{
		EnemyLife.OnDied += RemoveEnemy;
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= RemoveEnemy;
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

		GetComponent<SpriteRenderer> ().enabled = false;
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.isTrigger) return;

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
}
