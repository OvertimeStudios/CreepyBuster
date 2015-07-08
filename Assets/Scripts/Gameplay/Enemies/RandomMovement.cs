using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class RandomMovement : EnemyMovement 
{
	Rigidbody2D myRigidbody2D;

	public float vel;
	public RandomBetweenTwoConst timeToChangeVel;

	public EventDelegate onEnterRange;
	private List<EventDelegate> onEnterRangeList;

	private bool gotAngle = false;
	private float angle;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;

		GetComponent<Rigidbody2D>().isKinematic = false;
		StartCoroutine (ChangeVelocity (timeToChangeVel.Random ()));
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;

		StopAllCoroutines();
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		vel += LevelDesign.EnemiesBonusVel;

		myRigidbody2D = GetComponent<Rigidbody2D> ();

		myRigidbody2D.velocity = transform.right * vel;

		StartCoroutine (WaitForPosition ());

		onEnterRangeList = new List<EventDelegate> ();
		onEnterRangeList.Add (onEnterRange);
	}

	private IEnumerator WaitForPosition()
	{
		yield return transform.position == Vector3.zero;

		angle = transform.eulerAngles.z;

		gotAngle = true;
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

		if (gotAngle)
		{
			Vector3 eulerAngle = transform.eulerAngles;
			eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, angle, 0.05f);
			transform.eulerAngles = eulerAngle;

			myRigidbody2D.velocity = transform.right * vel;
		}
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

	void OnTriggerEnter2D(Collider2D col)
	{
		if(!this.enabled) return;

		if(col.gameObject.name == AttackTargets.Instance.gameObject.name && !col.isTrigger)
			EventDelegate.Execute(onEnterRangeList);
	}
}
