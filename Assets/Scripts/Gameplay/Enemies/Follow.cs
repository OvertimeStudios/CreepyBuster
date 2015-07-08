using UnityEngine;
using System.Collections;

public class Follow : EnemyMovement 
{
	public float vel;

	private Transform player;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		player = LightBehaviour.Instance.transform;

		vel += LevelDesign.EnemiesBonusVel;
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update ();

		float angle = Mathf.Atan2(player.position.y - transform.position.y, player.position.x - transform.position.x);

		transform.rotation = Quaternion.Euler (0, 0, angle * Mathf.Rad2Deg);

		GetComponent<Rigidbody2D> ().velocity = new Vector2 (Mathf.Cos (angle), Mathf.Sin (angle)) * vel;
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
