using UnityEngine;
using System.Collections;
using System;

public class MoveStraight : EnemyMovement 
{
	public float vel;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		vel += LevelDesign.EnemiesBonusVel;

		GetComponent<Rigidbody2D> ().velocity = transform.right * vel;

		if (GameController.IsSlowedDown)
			ApplySlow ();
	}

	private void ApplySlow()
	{
		GetComponent<Rigidbody2D> ().velocity = transform.right * vel * SlowDown.SlowAmount;

		myAnimator.speed *= SlowDown.SlowAmount;
	}
	
	private void RemoveSlow()
	{
		GetComponent<Rigidbody2D> ().velocity = transform.right * vel;

		myAnimator.speed *= 1 / SlowDown.SlowAmount;
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
