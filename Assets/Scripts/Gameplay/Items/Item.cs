﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour 
{
	public static event Action<Type, GameObject> OnCollected;

	public enum Type
	{
		Invecibility,
		LevelUp,
		DeathRay,
		SlowDown,
		PlasmaOrb,
	}
	
	public Type type;
	public float vel;

	void Start()
	{
		GetComponent<Rigidbody2D> ().velocity = transform.right * vel;

		transform.rotation = Quaternion.identity;
	}

	// Update is called once per frame
	protected virtual void Update () 
	{
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		
		if (pos.x < -0.3f || pos.x > 1.3f || pos.y < -0.3f || pos.y > 1.3f)
			OutOfScreen ();
	}
	
	public void OutOfScreen()
	{
		Destroy (gameObject);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.tag == "Player" && !col.isTrigger)
		{
			if(OnCollected != null)
				OnCollected(type, gameObject);

			Destroy(gameObject);
		}
	}
}