using UnityEngine;
using System.Collections;

public class ZigZag : EnemyMovement 
{
	private Rigidbody2D myRigidbody2D;

	public float vel;
	private float angle;

	public RandomBetweenTwoConst timeToChangeDirection;

	private Vector2 sideVelocity;
	private Vector2 frontVelocity;
	private Vector2 finalVelocity;

	private bool isSlowed;

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
		
		StopAllCoroutines();
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		isSlowed = GameController.IsSlowedDown;

		myRigidbody2D = GetComponent<Rigidbody2D> ();

		vel += LevelDesign.EnemiesBonusVel;

		sideVelocity = -(Vector2)transform.up;
		frontVelocity = (Vector2)transform.right;
		finalVelocity = (frontVelocity + sideVelocity).normalized;

		angle = Mathf.Atan2 (finalVelocity.y, finalVelocity.x) * Mathf.Rad2Deg;

		StartCoroutine (ChangeDirection (timeToChangeDirection.Random ()));
	}

	private IEnumerator ChangeDirection(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);

		ChangeDirection ();

		StartCoroutine (ChangeDirection (timeToChangeDirection.Random ()));
	}

	private void ChangeDirection()
	{
		sideVelocity *= -1;

		finalVelocity = (frontVelocity + sideVelocity).normalized;

		angle = Mathf.Atan2 (finalVelocity.y, finalVelocity.x) * Mathf.Rad2Deg;

		myAnimator.SetInteger ("State", (myAnimator.GetInteger("State") == 0) ? 1 : 0);
	}

	protected override void Update()
	{
		base.Update ();

		Vector3 eulerAngle = transform.eulerAngles;
		eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, angle, 0.05f);
		transform.eulerAngles = eulerAngle;

		if(isSlowed)
			myRigidbody2D.velocity = transform.right * vel * SlowDown.SlowAmount;
		else
			myRigidbody2D.velocity = transform.right * vel;
	}

	private void ApplySlow()
	{
		isSlowed = true;
		myAnimator.speed *= SlowDown.SlowAmount;
	}

	private void RemoveSlow()
	{
		isSlowed = false;
		myAnimator.speed *= 1 / SlowDown.SlowAmount;
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
