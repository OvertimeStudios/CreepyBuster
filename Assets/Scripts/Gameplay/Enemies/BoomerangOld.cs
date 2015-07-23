using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class BoomerangOld : EnemyMovement 
{
	private enum State
	{
		Going,
		Backing,
	}

	private enum SpawnPosition
	{
		Right,
		Left,
		Up,
		Down,
	}

	private State state = State.Going;
	private SpawnPosition spawnPosition;

	public float vel;
	public float rotVel;
	public float centerRadius;

	private Vector2 forceForward = Vector2.zero;
	private Vector2 forceSideway = Vector2.zero;
	private Vector2 center = Vector2.zero;

	private Rigidbody2D myRigidbody2D;
	private Transform spriteTransform;

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		spriteTransform = transform.FindChild ("Sprite");

		vel += LevelDesign.EnemiesBonusVel;
		rotVel += LevelDesign.EnemiesBonusVel;

		myRigidbody2D = GetComponent<Rigidbody2D> ();

		forceForward = (Vector2)transform.right;

		StartCoroutine (WaitForPosition ());
	}

	private IEnumerator WaitForPosition()
	{
		while(transform.position == Vector3.zero)
		{
			yield return null;
		}

		//set position it will change forward direction
		Vector2 pos = (Vector2)transform.position;
		center = Camera.main.ViewportToWorldPoint (new Vector3 (0.5f, 0.5f));
		
		float angle = (float)Math.Atan2 (center.y - pos.y, center.x - pos.x);

		center.x = 0.5f + (float)(Math.Cos (angle) * centerRadius);
		center.y = 0.5f + (float)(Math.Sin (angle) * centerRadius);;

		center = Camera.main.ViewportToWorldPoint (center);

		float totalTime = (Vector2.Distance (pos, center) * 2) / vel;
		float sideVel = 0f;

		//set side position where it will land
		pos = Camera.main.WorldToViewportPoint (pos);

		if (pos.x < 0)
			spawnPosition = SpawnPosition.Left;
		else if (pos.x > 1)
			spawnPosition = SpawnPosition.Right;
		else if (pos.y < 0)
			spawnPosition = SpawnPosition.Down;
		else if (pos.y > 1)
			spawnPosition = SpawnPosition.Up;

		//move Y
		if(spawnPosition == SpawnPosition.Left || spawnPosition == SpawnPosition.Right)
		{
			float landPosition = 0f;
			if(pos.y > 0.5f)//UP
				landPosition = 0.2f;
			else//DOWN
				landPosition = 0.8f;

			pos = Camera.main.ViewportToWorldPoint(new Vector2(pos.x, landPosition));

			sideVel = (pos.y - transform.position.y) / totalTime;

			forceSideway = transform.up * sideVel;

			//invert direction on RIGHT
			if(spawnPosition == SpawnPosition.Right)
				forceSideway *= -1;
		}
		else//move X
		{
			float landPosition = 0f;
			if(pos.x > 0.5f)//RIGHT
				landPosition = 0.2f;
			else//LEFT
				landPosition = 0.8f;
			
			pos = Camera.main.ViewportToWorldPoint(new Vector2(landPosition, pos.y));
			
			sideVel = (pos.x - transform.position.x) / totalTime;
			
			forceSideway = transform.up * sideVel;

			//invert direction on DOWN
			if(spawnPosition == SpawnPosition.Down)
				forceSideway *= -1;
		}

		myRigidbody2D.velocity = (forceForward + forceSideway).normalized * vel;
	}

	protected override void Update ()
	{
		base.Update ();

		spriteTransform.Rotate (Vector3.forward * rotVel);

		if(enemyLife.IsDead) return;

		Vector2 pos = (Vector2)transform.position;

		float distance = 0f;
		if (spawnPosition == SpawnPosition.Left || spawnPosition == SpawnPosition.Right)
			distance = Mathf.Abs (center.x - pos.x);
		else
			distance = Mathf.Abs (center.y - pos.y);

		if(state == State.Going)
		{
			forceForward = (Vector2)transform.right * distance;

			if(distance < 0.3f)
				state = State.Backing;
		}
		else
		{
			forceForward = -(Vector2)transform.right * distance;
		}

		myRigidbody2D.velocity = (forceForward + forceSideway).normalized * vel;
	}
	
	protected override void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
	}
	
	protected override void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
	}
	
	void OnDied(GameObject enemy)
	{
		if(enemy.Equals(gameObject))
		{
			myRigidbody2D.velocity = Vector2.zero;

			StartCoroutine(StopSpinning(EnemyLife.deathTime));
		}
	}

	private IEnumerator StopSpinning(float waitTime)
	{
		float maxRotVel = rotVel;

		while(rotVel > 0)
		{
			rotVel -= Time.deltaTime * EnemyLife.deathTime * maxRotVel;

			yield return null;
		}
	}
}
