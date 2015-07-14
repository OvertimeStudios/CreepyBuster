using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChargeBehaviour : EnemyMovement 
{
	private Rigidbody2D myRigidbody;

	private List<GameObject> brilhos;

	public float timeToCharge;
	public float vel;

	private int brilhosActive;
	private Transform player;

	private bool charging;
	private bool isSlowed;

	public bool ChargeReleased
	{
		get { return !charging; }
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		isSlowed = GameController.IsSlowedDown;

		myRigidbody = GetComponent<Rigidbody2D> ();

		brilhos = new List<GameObject> ();

		foreach (Transform t in transform.FindChild("Sprite"))
			brilhos.Add (t.gameObject);

		for (byte i = 1; i < brilhos.Count; i++)
			brilhos [i].SetActive (false);

		brilhosActive = 1;

		player = AttackTargets.Instance.transform;

		enabled = false;
	}

	void OnEnable()
	{
		charging = true;
		StartCoroutine (LightNextBrilho ());

		EnemyLife.OnDied += OnDied;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
	}

	void OnDisable()
	{
		StopAllCoroutines ();

		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
	}

	private IEnumerator LightNextBrilho()
	{
		yield return new WaitForSeconds (timeToCharge / 4);

		//length - 1
		if (brilhosActive < brilhos.Count)
		{
			brilhos [brilhosActive].SetActive (true);
			
			brilhosActive++;

			StartCoroutine (LightNextBrilho ());
		}
		else
			Charge ();
	}

	protected override void Update()
	{
		base.Update ();

		if(brilhosActive <= 3)
		{
			float angle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x);
			
			transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);
		}

		if(ChargeReleased)
		{
			myRigidbody.velocity *= 0.95f;

			if(myRigidbody.velocity.magnitude < 0.2f)
			{
				//back to normal state
				myAnimator.SetInteger ("State", 0);
				charging = true;
				brilhosActive = 1;

				brilhos [0].SetActive (true);

				GetComponent<RandomMovement>().enabled = true;
				enabled = false;
			}
		}
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

	private void Charge()
	{
		charging = false;

		myAnimator.SetInteger ("State", 1);

		if(isSlowed)
			myRigidbody.velocity = transform.right * vel * SlowDown.SlowAmount;
		else
			myRigidbody.velocity = transform.right * vel;

		foreach (GameObject go in brilhos)
			go.SetActive (false);
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
