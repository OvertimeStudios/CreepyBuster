using UnityEngine;
using System.Collections;

public class ZigZag : EnemyMovement 
{
	enum Side
	{
		Right,
		Left,
	}

	private Rigidbody2D myRigidbody2D;

	public EnemiesPercent.EnemyNames type;
	public float vel;
	private float angle;

	public RandomBetweenTwoConst timeToChangeDirection;

	private Vector2 sideVelocity;
	private Vector2 frontVelocity;
	private Vector2 finalVelocity;

	private Side lastSide;

	private bool isSlowed;
	private bool isFrozen;

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
		
		StopAllCoroutines();
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		//load stats
		EnemyStats stats = LevelDesign.GetEnemyStats(type);
		if(stats != null)
			vel = stats.vel;
		
		isSlowed = GameController.IsSlowedDown;

		myRigidbody2D = GetComponent<Rigidbody2D> ();

		vel += LevelDesign.EnemiesBonusVel;

		StartCoroutine (WaitForPosition ());
	}

	private IEnumerator WaitForPosition()
	{
		yield return new WaitForEndOfFrame();

		int dir = 1;
		lastSide = Side.Right;

		Vector3 viewportPosition = Camera.main.WorldToViewportPoint (transform.position);
		if((viewportPosition.x < 0 && viewportPosition.y > 0.5f) ||
		   (viewportPosition.x > 1 && viewportPosition.y < 0.5f) ||
		   (viewportPosition.y < 0 && viewportPosition.x < 0.5f) ||
		   (viewportPosition.y > 1 && viewportPosition.x > 0.5f))
		{
			dir = -1;
			lastSide = Side.Left;
		}

		sideVelocity = (Vector2)transform.up * dir;
		frontVelocity = (Vector2)transform.right;
		finalVelocity = (frontVelocity + sideVelocity).normalized;

		myAnimator.SetInteger ("State", (lastSide == Side.Right) ? 0 : 1);

		yield return new WaitForEndOfFrame();

		myAnimator.SetInteger("State", 2);

		angle = Mathf.Atan2 (finalVelocity.y, finalVelocity.x) * Mathf.Rad2Deg;
		
		StartCoroutine (ChangeDirection (timeToChangeDirection.Random ()));
	}

	private IEnumerator ChangeDirection(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);

		if(!isFrozen)
			ChangeDirection ();

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		myAnimator.SetInteger("State", 2);

		StartCoroutine (ChangeDirection (timeToChangeDirection.Random ()));
	}

	private void ChangeDirection()
	{
		sideVelocity *= -1;

		finalVelocity = (frontVelocity + sideVelocity).normalized;

		angle = Mathf.Atan2 (finalVelocity.y, finalVelocity.x) * Mathf.Rad2Deg;

		lastSide = (lastSide == Side.Left) ? Side.Right : Side.Left;

		myAnimator.SetInteger ("State", (lastSide == Side.Right) ? 0 : 1);
	}

	protected override void Update()
	{
		base.Update ();

		if (isFrozen)
			myRigidbody2D.velocity = Vector2.zero;
		else
		{
			Vector3 eulerAngle = transform.eulerAngles;
			eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, angle, 0.05f);
			transform.eulerAngles = eulerAngle;

			if(isSlowed)
				myRigidbody2D.velocity = transform.right * vel * SlowDown.SlowAmount;
			else
				myRigidbody2D.velocity = transform.right * vel;
		}
	}

	private void ApplySlow()
	{
		isSlowed = true;
	}

	private void RemoveSlow()
	{
		isSlowed = false;
	}

	private void ApplyFrozen()
	{
		isFrozen = true;
	}
	
	private void RemoveFrozen()
	{
		isFrozen = false;
	}

	void OnDied(GameObject enemy)
	{
		if(enemy == gameObject)
		{
			StopAllCoroutines();
			
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			
			enabled = false;
		}
	}
}
