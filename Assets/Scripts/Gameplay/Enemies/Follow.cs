using UnityEngine;
using System.Collections;

public class Follow : EnemyMovement 
{
	public float vel;

	private Transform player;
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

		if(isFrozen) return;

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
	}
	
	private void RemoveSlow()
	{
		isSlowed = false;
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
		isFrozen = true;

		GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}
	
	private void RemoveFrozen()
	{
		isFrozen = false;
	}
}
