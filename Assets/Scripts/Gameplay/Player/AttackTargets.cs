using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackTargets : MonoBehaviour 
{
	public static event Action<float> OnSpecialTimerUpdated;

	private static List<Transform> targets;

	private static List<Transform> enemiesInRange;
	
	private float specialCounter;

	private static bool isSpecial;

	private int layerMask;

	public float damage;

	private CircleCollider2D range;

	#region get / set
	public static bool IsSpecialActive
	{
		get { return isSpecial; }
	}

	public static bool IsAttacking
	{
		get { return targets.Count > 0; }
	}

	#endregion

	#region singleton
	private static AttackTargets instance;

	public static AttackTargets Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<AttackTargets>();

			return instance;
		}
	}
	#endregion

	void OnEnable()
	{
		Reset ();

		MenuController.OnPanelClosed += Reset;
		GameController.OnGameStart += GetDamage;
		GameController.OnGameStart += GetRange;
		GameController.OnFingerHit += OnFingerHit;
		FingerDetector.OnFingerUpEvent += OnFingerUp;
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
		GameController.OnGameStart -= GetDamage;
		GameController.OnGameStart -= GetRange;
		GameController.OnFingerHit -= OnFingerHit;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;
	}

	// Use this for initialization
	void Start () 
	{
		layerMask = LayerMask.NameToLayer ("AttackCollider");

		isSpecial = false;

		targets = new List<Transform> ();
		enemiesInRange = new List<Transform> ();

		foreach(CircleCollider2D col in GetComponents<CircleCollider2D>())
		{
			if(col.isTrigger)
			{
				range = col;
				break;
			}
		}

		gameObject.SetActive (false);
	}

	private void GetDamage()
	{
		damage = LevelDesign.CurrentDamage;
	}

	private void GetRange()
	{
		range.radius = LevelDesign.CurrentRange;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		GetTargets ();

		if (isSpecial)
			RunTimer ();

		enemiesInRange.Clear ();
	}

	private void GetTargets ()
	{
		List<Transform> newTargets = new List<Transform> ();

		if (GameController.gameOver) 
		{
			//do nothing
		}
		else if (isSpecial)
		{
			//get all damagable targets
			foreach(Transform t in SpawnController.enemiesInGame)
			{
				if(t == null) continue;

				//don't apply damage to those enemies who doesn't show up yet
				if(t.GetComponent<EnemyLife>().IsDamagable)
					newTargets.Add(t);
			}
		}
		else
		{
			//get closest targets
			foreach(Transform t in enemiesInRange)
			{
				if(t == null) continue;

				//don't apply damage to those enemies who doesn't show up yet
				if(!t.GetComponent<EnemyLife>().IsDamagable) continue;

				if(newTargets.Count < LevelDesign.MaxRays)
					newTargets.Add(t);
				else
				{
					foreach(Transform nt in newTargets)
					{
						if(nt == null) continue;

						if(Vector3.Distance(transform.position, t.position) < Vector3.Distance(transform.position, nt.position))
						{
							newTargets.Remove(nt);
							newTargets.Add(t);
							break;
						}
					}
				}
			}
		}

		//see if they are new
		foreach(Transform nt in newTargets)
		{
			if(nt == null) continue;

			if(!nt.GetComponent<EnemyLife>().inLight)
				nt.GetComponent<EnemyLife>().OnLightEnter();
		}

		foreach(Transform t in targets)
		{
			if(t == null) continue;

			if(!newTargets.Contains(t))
				t.GetComponent<EnemyLife>().OnLightExit();
		}

		targets = newTargets;
	}

	public void UseSpecial()
	{
		isSpecial = true;
		specialCounter = LevelDesign.Instance.specialTime;
		GameController.specialStreak++;
	}

	private void RunTimer()
	{
		specialCounter -= Time.deltaTime;

		if (specialCounter <= 0)
			StopSpecial ();

		if (OnSpecialTimerUpdated != null)
			OnSpecialTimerUpdated (specialCounter / LevelDesign.Instance.specialTime);
	}

	private void LoseAllTargets()
	{
		foreach (Transform t in targets)
		{
			if(t == null) continue;

			t.GetComponent<EnemyLife> ().OnLightExit ();
		}

		targets = new List<Transform> ();
	}

	private IEnumerator StopSpecial(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);

		StopSpecial ();
	}

	public void StopSpecial()
	{
		isSpecial = false;
	}

	private void OnFingerHit()
	{
		if(LevelDesign.PlayerLevel == 0)
		{
			LoseAllTargets();
			gameObject.SetActive (false);
		}
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		LoseAllTargets ();
		gameObject.SetActive (false);
	}

	private void Reset()
	{
		isSpecial = false;

		targets.Clear ();
		enemiesInRange.Clear ();
	}
	

	void OnTriggerStay2D(Collider2D col)
	{
		if(col.gameObject.layer != layerMask) return;

		enemiesInRange.Add (col.transform.parent);
	}

	//enemy collided with player finger
	void OnCollisionEnter2D(Collision2D col)
	{
		GameController.Instance.FingerHit ();
		
		col.gameObject.GetComponent<EnemyLife>().Dead(false);
	}
}
