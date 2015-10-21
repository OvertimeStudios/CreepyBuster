using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class RandomMovement : EnemyMovement 
{
	Rigidbody2D myRigidbody2D;

	public float vel;
	public RandomBetweenTwoConst timeToChangeVel;

	public bool freezeRotation = false;

	public EventDelegate onEnterRange;
	private List<EventDelegate> onEnterRangeList;

	private float angle;
	private bool isSlowed;
	private bool isFrozen;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;

	protected override void OnEnable()
	{
		base.OnEnable ();

		EnemyLife.OnDied += OnDied;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
		GameController.OnFrozenCollected += ApplyFrozen;
		GameController.OnFrozenFade += RemoveFrozen;

		GetComponent<Rigidbody2D>().isKinematic = false;

		StartCoroutine (ChangeVelocity (timeToChangeVel.Random ()));
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

		isSlowed = GameController.IsSlowedDown;

		vel += LevelDesign.EnemiesBonusVel;

		myRigidbody2D = GetComponent<Rigidbody2D> ();

		if(transform.FindChild("Sprite") != null)
			spriteRenderer = transform.FindChild("Sprite").GetComponent<SpriteRenderer>();

		myRigidbody2D.velocity = transform.right * vel;

		if (GameController.IsSlowedDown)
			ApplySlow ();

		StartCoroutine (WaitForPosition ());

		onEnterRangeList = new List<EventDelegate> ();
		onEnterRangeList.Add (onEnterRange);
	}

	private IEnumerator WaitForPosition()
	{
		yield return new WaitForEndOfFrame();

		angle = transform.eulerAngles.z;
	}

	private IEnumerator ChangeVelocity(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		ChangeVelocity ();

		StartCoroutine (ChangeVelocity (timeToChangeVel.Random ()));
	}

	private void ChangeVelocity()
	{
		angle = transform.eulerAngles.z + Random.Range (-90f, 90f);
	}

	protected override void Update()
	{
		base.Update ();

		if(isFrozen) return;

		UpdateRotation();

		if (freezeRotation) 
		{
			Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);

			if(isSlowed)
				myRigidbody2D.velocity = dir * vel * SlowDown.SlowAmount;
			else
				myRigidbody2D.velocity = dir * vel;
		}
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

	private void UpdateRotation()
	{
		if(spriteRenderer == null) return;

		Bounds bounds = spriteRenderer.bounds;

		Vector3 minPos = Camera.main.WorldToViewportPoint(bounds.min);
		Vector3 maxPos = Camera.main.WorldToViewportPoint(bounds.max);

		if(minPos.x < 0.1f)
			angle = 0;
		else if(minPos.y < 0.1f)
			angle = 90;
		else if(maxPos.x > 0.9f)
			angle = 180;
		else if(maxPos.y > 0.9f)
			angle = -90;
	}

	private bool CheckInsideScreen()
	{
		if(spriteRenderer == null) return false;
		
		Bounds bounds = spriteRenderer.bounds;
		
		Vector3 minPos = Camera.main.WorldToViewportPoint(bounds.min);
		Vector3 maxPos = Camera.main.WorldToViewportPoint(bounds.max);
		
		if(minPos.x > 0f && minPos.y > 0f && maxPos.x < 1f && maxPos.y < 1f)
			return true;
		
		return false;
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

	public void OnRangeEntered()
	{
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		
		GetComponent<ChargeBehaviour>().enabled = true;
		enabled = false;
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if(!this.enabled || isFrozen) return;

		if(col.gameObject.name == AttackTargets.Instance.gameObject.name && !col.isTrigger && CheckInsideScreen())
			EventDelegate.Execute(onEnterRangeList);
	}

	private void ApplyFrozen()
	{
		isFrozen = true;
		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}
	
	private void RemoveFrozen()
	{
		isFrozen = false;
		GetComponent<Rigidbody2D> ().velocity = transform.right * vel;
	}
}
