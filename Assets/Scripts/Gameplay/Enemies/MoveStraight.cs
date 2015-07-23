using UnityEngine;
using System.Collections;
using System;

public class MoveStraight : EnemyMovement 
{
	public float vel;
	private Vector2 lastVelocityBeforeFrozen;

	protected override void OnEnable()
	{
		base.OnEnable ();

		EnemyLife.OnDied += OnDied;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
		GameController.OnFrozenCollected += ApplyFrozen;
		GameController.OnFrozenFade += RemoveFrozen;
	}
	
	protected override void OnDisable()
	{
		base.OnDisable ();

		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
		GameController.OnFrozenCollected -= ApplyFrozen;
		GameController.OnFrozenFade -= RemoveFrozen;
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
	}
	
	private void RemoveSlow()
	{
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

	private void ApplyFrozen()
	{
		lastVelocityBeforeFrozen = GetComponent<Rigidbody2D> ().velocity;
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}
	
	private void RemoveFrozen()
	{
		GetComponent<Rigidbody2D> ().velocity = lastVelocityBeforeFrozen;
	}
}
