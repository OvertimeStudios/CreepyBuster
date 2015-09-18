using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IllusionBossCopy : MonoBehaviour 
{
	public float initialAlpha = 0.5f;
	public float timeToRegainAlpha = 0.5f;
	public RandomBetweenTwoConst attackTime;
	public float projectileAlpha = 0.8f;

	private Vector3 waypoint;

	private SpriteRenderer sprite;
	private TweenPosition tween;
	private Transform myTransform;
	private Rigidbody2D myRigidbody2D;
	private Collider2D[] myColliders;
	private Transform spawnPosition;
	private List<GameObject> brilhos;

	private bool readyToAttack;

	#region get / set
	public bool IsReadyToMovement
	{
		get { return !tween.enabled && sprite.color.a == 1; }
	}

	public bool IsReadyToAttack
	{
		get { return readyToAttack; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		readyToAttack = false;

		sprite = transform.FindChild("Sprite").GetComponent<SpriteRenderer>();
		myTransform = transform;
		myRigidbody2D = GetComponent<Rigidbody2D>();
		myColliders = GetComponentsInChildren<Collider2D>();
		spawnPosition = transform.FindChild("Spawn");

		brilhos = new List<GameObject>();
		foreach(Transform brilho in sprite.transform)
			brilhos.Add(brilho.gameObject);

		Color c = sprite.color;
		c.a = initialAlpha;
		sprite.color = c;

		foreach(Collider2D col in myColliders)
			col.enabled = false;

		foreach(GameObject brilho in brilhos)
			brilho.SetActive(false);
	}

	public void HeadToStartPosition(Vector3 startPosition)
	{
		tween = GetComponent<TweenPosition>();

		tween.from = transform.position;
		tween.to = startPosition;

		tween.PlayForward();
	}

	public void BackAlphaToNormal()
	{
		StartCoroutine(BackAlpha());
	}

	private IEnumerator BackAlpha()
	{
		Color c;
		while(sprite.color.a < 1)
		{
			c = sprite.color;
			c.a += Time.deltaTime / timeToRegainAlpha;
			sprite.color = c;

			yield return null;
		}

		c = sprite.color;
		c.a = 1;
		sprite.color = c;

		foreach(Collider2D col in myColliders)
			col.enabled = true;
	}

	public void StartMoving()
	{
		GetWaypoint();
	}

	private void GetWaypoint()
	{
		waypoint = IllusionBoss.GetValidWaypoint(waypoint);
		
		if(waypoint != Vector3.zero)
			StartCoroutine(WaitForArrival());
		else
			StartCoroutine(AlignRotation());
	}
	
	private IEnumerator WaitForArrival()
	{
		while(Vector3.Distance(myTransform.position, waypoint) > 0.15f)
		{
			myRigidbody2D.velocity = transform.right * IllusionBoss.Velocity;
			
			float angle = Mathf.Atan2(waypoint.y - myTransform.position.y, waypoint.x - myTransform.position.x) * Mathf.Rad2Deg;
			Vector3 eulerAngle = transform.eulerAngles;
			eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, angle, 0.1f);
			myTransform.eulerAngles = eulerAngle;
			
			yield return null;
		}
		
		GetWaypoint();
	}

	private IEnumerator AlignRotation()
	{
		myRigidbody2D.velocity = Vector2.zero;

		while(Mathf.Abs(Mathf.DeltaAngle(myTransform.eulerAngles.z, -90f)) > 2f)
		{
			Vector3 eulerAngle = myTransform.eulerAngles;
			eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, -90f, 0.05f);
			myTransform.eulerAngles = eulerAngle;

			yield return null;
		}

		transform.rotation = Quaternion.Euler(0, 0, -90f);

		readyToAttack = true;
	}

	public void StartAttacking()
	{
		foreach(GameObject brilho in brilhos)
			brilho.SetActive(true);

		StartCoroutine(Attack());
	}
	
	private IEnumerator Attack()
	{
		yield return new WaitForSeconds(attackTime.Random());
		
		FireProjectile();
		
		StartCoroutine(Attack());
	}
	
	private void FireProjectile()
	{
		Vector3 player = AttackTargets.Instance.transform.position;
		float angle = Mathf.Atan2 (player.y - spawnPosition.position.y, player.x - spawnPosition.position.x);
		Quaternion rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
		
		GameObject energyBall = Instantiate(IllusionBoss.Projectile, spawnPosition.position, rotation) as GameObject;
		energyBall.GetComponent<Rigidbody2D>().velocity = energyBall.transform.right * IllusionBoss.ProjectileVelocity;
	}
}
