using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrilhoGalaxia : MonoBehaviour 
{
	private int objectsInFront = 0;

	void OnTriggerStay2D(Collider2D col)
	{
		if(col.isTrigger) return;

		objectsInFront++;
	}

	void Update()
	{
		if(objectsInFront > 0)
			GetComponent<SpriteRenderer> ().enabled = false;
		else
			GetComponent<SpriteRenderer> ().enabled = true;

		objectsInFront = 0;
	}
}
