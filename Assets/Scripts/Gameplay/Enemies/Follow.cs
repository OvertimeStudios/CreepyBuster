using UnityEngine;
using System.Collections;

public class Follow : EnemyMovement 
{
	public float vel;

	private Transform player;
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
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		isSlowed = GameController.IsSlowedDown;

		player = LightBehaviour.Instance.transform;

		vel += LevelDesign.EnemiesBonusVel;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update ();

		float angle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x);

		transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);

		if(isSlowed)
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * vel * SlowDown.SlowAmount;
		else
			GetComponent<Rigidbody2D> ().velocity = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * vel;
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
			GetComponent<Rigidbody2D>().velocity = Vector2.zero;
			
			enabled = false;
		}
	}
}
