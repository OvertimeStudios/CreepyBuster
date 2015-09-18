using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IllusionBoss : MonoBehaviour 
{
	public enum State
	{
		Appearing,
		CreatingIllusions,
		Moving,
		Stopping,
		ReadyToAttack,
		Attacking,
		BackToCenter,
		Defeated,
	}

	public float appearTime;
	public float delayToStartCreateIllusions = 1f;
	public float delayToStartMoving = 1f;
	public RandomBetweenTwoConst attackTime;
	public GameObject projectile;

	[Header("Copy")]
	public GameObject illusionCopy;
	public float illusionSpawnSpread = 1.3f;
	public float delayToCreateIllusion = 0.5f;
	public float delayToIllusionsRegainAlpha = 1f;

	[Header("Level Design")]
	public List<IllusionLevel> levels;

	private int level = 0;
	private Vector3 waypoint;
	private Coroutine attackCoroutine;
	[HideInInspector]
	public State state;

	/// <summary>
	/// Viewport points where illusions and boss could go
	/// </summary>
	private List<Vector3> waypoints;
	private List<IllusionBossCopy> illusions;
	private List<Vector3> waypointsAlreadyChosen;

	private SpriteRenderer mySprite;
	private Collider2D[] myColliders;
	private Transform myTransform;
	private Rigidbody2D myRigidbody2D;
	private List<GameObject> brilhos;
	private BossLife bossLife;

	private CameraShake cameraShake;
	private float totalLife;
	private float lifePerLevel;

	private Transform spawnPosition;

	#region singleton
	private static IllusionBoss instance;
	public static IllusionBoss Instance
	{
		get { return instance; }
	}
	#endregion

	#region get / set
	public static float Velocity
	{
		get { return Instance.levels[Instance.level].vel; }
	}

	public static bool IsAttacking
	{
		get { return Instance.state == State.Attacking; }
	}

	public static GameObject Projectile
	{
		get { return Instance.projectile; }
	}

	public static float ProjectileVelocity
	{
		get { return Instance.levels[Instance.level].projectileVel; }
	}

	public static bool IsLevelMax
	{
		get { return Instance.level == Instance.levels.Count - 1; }
	}

	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;

		level = 0;

		illusions = new List<IllusionBossCopy>();
		waypointsAlreadyChosen = new List<Vector3>();

		waypoints = new List<Vector3>();
		waypoints.Add(new Vector3(0.2f, 0.2f));//bottom left
		waypoints.Add(new Vector3(0.2f, 0.5f));//center left
		waypoints.Add(new Vector3(0.2f, 0.8f));//top left
		waypoints.Add(new Vector3(0.8f, 0.2f));//bottom right
		waypoints.Add(new Vector3(0.8f, 0.5f));//center right
		waypoints.Add(new Vector3(0.8f, 0.8f));//top right

		//convert waypoint to world space
		for(byte i = 0; i < waypoints.Count; i++)
		{
			Vector3 waypoint = Camera.main.ViewportToWorldPoint(waypoints[i]);
			waypoint.z = transform.position.z;

			waypoints[i] = waypoint;
		}

		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
		pos.z = 0;
		transform.position = pos;
		transform.rotation = Quaternion.Euler(0, 0, -90f);

		mySprite = transform.FindChild("Sprite").GetComponent<SpriteRenderer>();
		myColliders = GetComponentsInChildren<Collider2D>();
		myTransform = transform;
		myRigidbody2D = GetComponent<Rigidbody2D>();
		spawnPosition = transform.FindChild("Spawn");
		bossLife = GetComponent<BossLife>();

		cameraShake = GetComponent<CameraShake>();
		totalLife = bossLife.life;
		lifePerLevel = totalLife / levels.Count;

		brilhos = new List<GameObject>();
		foreach(Transform brilho in mySprite.transform)
			brilhos.Add(brilho.gameObject);

		StartCoroutine(Appear());
	}

	private IEnumerator Appear()
	{
		state = State.Appearing;

		foreach(GameObject brilho in brilhos)
			brilho.SetActive(false);

		foreach(Collider2D col in myColliders)
			col.enabled = false;

		Color c = mySprite.color;
		c.a = 0;
		mySprite.color = c;

		float alpha = 0;

		while(alpha < 1)
		{
			alpha += Time.deltaTime / appearTime;

			c = mySprite.color;
			c.a = alpha;
			mySprite.color = c;

			yield return null;
		}

		c = mySprite.color;
		c.a = 1;
		mySprite.color = c;

		foreach(Collider2D col in myColliders)
			col.enabled = true;

		Debug.Log("Finished Appear");

		StartCoroutine(CreateIllusions());
	}

	private IEnumerator CreateIllusions()
	{
		state = State.CreatingIllusions;

		yield return new WaitForSeconds(delayToStartCreateIllusions);

		for(byte i = 0; i < levels[level].copies; i++)
		{
			GameObject illusion = Instantiate(illusionCopy) as GameObject;
			illusion.transform.position = transform.position;
			illusion.transform.rotation = transform.rotation;

			float angle = levels[level].angleToAppear[i] * Mathf.Deg2Rad;

			illusion.GetComponent<IllusionBossCopy>().HeadToStartPosition(transform.position + (new Vector3(Mathf.Cos (angle), Mathf.Sin(angle)) * illusionSpawnSpread));

			illusions.Add(illusion.GetComponent<IllusionBossCopy>());

			yield return new WaitForSeconds(delayToCreateIllusion);
		}

		yield return new WaitForSeconds(delayToIllusionsRegainAlpha);

		foreach(IllusionBossCopy ill in illusions)
			ill.BackAlphaToNormal();

		StartCoroutine(WaitForIllusionsMove());
	}

	public IEnumerator WaitForIllusionsMove()
	{
		bool isReady = false;

		while(!isReady)
		{
			isReady = true;

			foreach(IllusionBossCopy illusion in illusions)
			{
				if(!illusion.IsReadyToMovement)
					isReady = false;
			}

			yield return null;
		}

		Debug.Log("All illusions are ready");

		yield return new WaitForSeconds(delayToStartMoving);

		StartCoroutine(StartMoving());
	}

	private IEnumerator StartMoving()
	{
		state = State.Moving;

		foreach(IllusionBossCopy illusion in illusions)
			illusion.StartMoving();

		GetWaypoint();

		yield return new WaitForSeconds(levels[level].timeMoving);

		StopMoving();
	}

	private void GetWaypoint()
	{
		waypoint = GetValidWaypoint(waypoint);

		if(waypoint != Vector3.zero)
			StartCoroutine(WaitForArrival());
		else
			StartCoroutine(AlignRotation());
	}

	private IEnumerator WaitForArrival()
	{
		while(Vector3.Distance(myTransform.position, waypoint) > 0.25f)
		{
			myRigidbody2D.velocity = transform.right * Velocity;

			float angle = Mathf.Atan2(waypoint.y - myTransform.position.y, waypoint.x - myTransform.position.x) * Mathf.Rad2Deg;
			Vector3 eulerAngle = transform.eulerAngles;
			eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, angle, 0.1f);
			myTransform.eulerAngles = eulerAngle;

			yield return null;
		}

		GetWaypoint();
	}

	private void StopMoving()
	{
		state = State.Stopping;

		Debug.Log("Stop Moving... waiting for illusions");

		StartCoroutine(WaitForIllusionsAttack());
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
		
		state = State.ReadyToAttack;
	}

	private IEnumerator WaitForIllusionsAttack()
	{
		bool isReady = false;
		
		while(!isReady)
		{
			isReady = true;
			
			foreach(IllusionBossCopy illusion in illusions)
			{
				if(!illusion.IsReadyToAttack)
					isReady = false;
			}

			//verify me as well
			if(state != State.ReadyToAttack)
				isReady = false;

			yield return null;
		}

		Debug.Log("All Illusions are ready to attack");

		foreach(IllusionBossCopy illusion in illusions)
			illusion.StartAttacking();

		StartAttacking();
	}

	private void StartAttacking()
	{
		state = State.Attacking;

		foreach(GameObject brilho in brilhos)
			brilho.SetActive(true);

		attackCoroutine = StartCoroutine(Attack());
		StartCoroutine(WaitForLifeLose());
	}

	private IEnumerator Attack()
	{
		yield return new WaitForSeconds(attackTime.Random());

		FireProjectile();

		attackCoroutine = StartCoroutine(Attack());
	}

	private void FireProjectile()
	{
		Vector3 player = AttackTargets.Instance.transform.position;
		float angle = Mathf.Atan2 (player.y - spawnPosition.position.y, player.x - spawnPosition.position.x);
		Quaternion rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
		
		GameObject energyBall = Instantiate(IllusionBoss.Projectile, spawnPosition.position, rotation) as GameObject;
		energyBall.GetComponent<Rigidbody2D>().velocity = energyBall.transform.right * IllusionBoss.ProjectileVelocity;
	}

	private IEnumerator WaitForLifeLose()
	{
		while(bossLife.life > totalLife - (lifePerLevel * (level + 1)))
			yield return null;

		StopCoroutine(attackCoroutine);

		foreach(IllusionBossCopy illusion in illusions)
		{
			if(illusion != null)
				illusion.GetComponent<EnemyLifeIllusionCopy>().Dead(false);
		}

		illusions.Clear();

		if(!IsLevelMax)
		{
			state = State.BackToCenter;

			cameraShake.Shake();

			foreach(GameObject brilho in brilhos)
				brilho.SetActive(false);
			
			yield return new WaitForSeconds(cameraShake.duration);
			
			StartCoroutine(BackToCenter());
		}
		else
		{
			state = State.Defeated;

			GetComponentInChildren<Collider2D>().enabled = false;
			
			Time.timeScale = 0.4f;
			
			cameraShake.Shake(bossLife.deathTime);
			ScreenFeedback.ShowBlank(bossLife.deathTime, 0.5f);

			//Color color = brilhos[0].GetComponent<SpriteRenderer>().color;
			while(brilhos[0].GetComponent<SpriteRenderer>().color.a > 0)
			{
				foreach(GameObject brilho in brilhos)
				{
					SpriteRenderer s = brilho.GetComponent<SpriteRenderer>();
					Color c = s.color;
					c.a -= Time.deltaTime / (bossLife.deathTime);
					s.color = c;
				}
				yield return null;
			}
			
			Time.timeScale = 1f;
		}
	}

	private IEnumerator BackToCenter()
	{
		TweenPosition tween = GetComponent<TweenPosition>();

		Vector3 from = transform.position;
		Vector3 to = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));
		to.z = transform.position.z;

		tween.ResetToBeginning();
		
		tween.from = from;
		tween.to = to;
		
		tween.PlayForward ();

		while(tween.enabled)
			yield return null;

		level++;

		StartCoroutine(CreateIllusions());
	}

	public static Vector3 GetValidWaypoint(Vector3 prevWaypoint)
	{
		if(prevWaypoint != Vector3.zero)
		{
			for(byte i = 0; i < Instance.waypointsAlreadyChosen.Count; i++)
			{
				Vector3 waypoint = Instance.waypointsAlreadyChosen[i];
				if(waypoint.x == prevWaypoint.x && waypoint.y == prevWaypoint.y)
				{
					Instance.waypointsAlreadyChosen.RemoveAt(i);

					break;
				}
			}
		}

		if(Instance.state == State.Moving)
		{
			Vector3 waypoint;
			int rnd;
			do
			{
				rnd = (int)Random.Range(0, Instance.waypoints.Count);
				waypoint = Instance.waypoints[rnd];
			}
			while(Instance.waypointsAlreadyChosen.Contains(waypoint));

			waypoint.z = Instance.transform.position.z;

			Instance.waypointsAlreadyChosen.Add(waypoint);

			return waypoint;
		}

		return Vector3.zero;
	}
}

[System.Serializable]
public class IllusionLevel
{
	public float vel;
	public int copies;
	public List<float> angleToAppear;
	public float timeMoving;
	public float projectileVel;
}