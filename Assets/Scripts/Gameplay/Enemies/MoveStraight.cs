using UnityEngine;
using System.Collections;
using System;

public class MoveStraight : EnemyMovement 
{
	public float vel;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		vel += LevelDesign.EnemiesBonusVel;

		GetComponent<Rigidbody2D> ().velocity = transform.right * vel;
	}

	void OnDied(GameObject enemy)
	{
		if(enemy == gameObject)
		{
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			
			enabled = false;
		}
	}
}
