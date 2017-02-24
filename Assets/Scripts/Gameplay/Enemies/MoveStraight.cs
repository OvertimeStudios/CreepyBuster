using UnityEngine;
using System.Collections;
using System;

public class MoveStraight : EnemyMovement 
{
	public EnemiesPercent.EnemyNames type;

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
		ConsumablesController.OnAnyItemUsed += ApplyFrozen;
		ConsumablesController.OnAllItensUsed += RemoveFrozen;
	}
	
	protected override void OnDisable()
	{
		base.OnDisable ();

		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
		GameController.OnFrozenCollected -= ApplyFrozen;
		GameController.OnFrozenFade -= RemoveFrozen;
		ConsumablesController.OnAnyItemUsed -= ApplyFrozen;
		ConsumablesController.OnAllItensUsed -= RemoveFrozen;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		//load stats
		EnemyStats stats = LevelDesign.GetEnemyStats(type);
		if(stats != null)
			vel = stats.vel;
		
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
		if(GetComponent<Rigidbody2D> ().velocity != Vector2.zero)
			lastVelocityBeforeFrozen = GetComponent<Rigidbody2D> ().velocity;
		
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}
	
	private void RemoveFrozen()
	{
		if(!ConsumablesController.IsUsingConsumables && !GameController.IsFrozen)
			GetComponent<Rigidbody2D> ().velocity = lastVelocityBeforeFrozen;
	}
}
