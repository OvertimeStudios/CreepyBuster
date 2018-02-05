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
	private bool isFrozen;

	private Vector2 lastVelocityBeforeFrozen;

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

		foreach (Transform t in transform.Find("Sprite"))
			brilhos.Add (t.gameObject);

		for (byte i = 1; i < brilhos.Count; i++)
			brilhos [i].SetActive (false);

		brilhosActive = 1;

		player = AttackTargets.Instance.transform;

		enabled = false;
	}

	protected override void OnEnable()
	{
		base.OnEnable ();

		charging = true;
		StartCoroutine (LightNextBrilho ());

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

		StopAllCoroutines ();

		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
		GameController.OnFrozenCollected -= ApplyFrozen;
		GameController.OnFrozenFade -= RemoveFrozen;
		ConsumablesController.OnAnyItemUsed -= ApplyFrozen;
		ConsumablesController.OnAllItensUsed -= RemoveFrozen;
	}

	private IEnumerator LightNextBrilho()
	{
		float counter = 0f;

		while(counter < timeToCharge / 4)
		{
			if(!isFrozen)
				counter += Time.deltaTime;

			yield return null;
		}

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

		if(isFrozen) return;

		if(brilhosActive <= 3)
		{
			float angle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x);
			
			transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);
		}

		if(ChargeReleased)
		{
			myRigidbody.velocity *= 0.95f;

			if(myRigidbody.velocity.magnitude < 0.5f)
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
	}
	
	private void RemoveSlow()
	{
		isSlowed = false;
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

	private void ApplyFrozen()
	{
		isFrozen = true;
		lastVelocityBeforeFrozen = myRigidbody.velocity;

		myRigidbody.velocity = Vector2.zero;
	}
	
	private void RemoveFrozen()
	{
		if(!ConsumablesController.IsUsingConsumables && !GameController.IsFrozen)
		{
			isFrozen = false;

			myRigidbody.velocity = lastVelocityBeforeFrozen;
		}
	}
}
