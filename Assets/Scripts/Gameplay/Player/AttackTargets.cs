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
		MenuController.OnPanelClosed += Reset;
		EnemyLife.OnDied += RemoveEnemyFromList;
		EnemyMovement.OnOutOfScreen += RemoveEnemyFromList;
		GameController.OnGameStart += GetDamage;
		GameController.OnGameStart += GetRange;
		GameController.OnShowContinueScreen += LoseAllTargets;
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
		EnemyLife.OnDied -= RemoveEnemyFromList;
		EnemyMovement.OnOutOfScreen -= RemoveEnemyFromList;
		GameController.OnGameStart -= GetDamage;
		GameController.OnGameStart -= GetRange;
		GameController.OnShowContinueScreen -= LoseAllTargets;
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
	void Update () 
	{
		GetTargets ();

		if (isSpecial)
			RunTimer ();
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
				//don't apply damage to those enemies who doesn't show up yet
				if(!t.GetComponent<EnemyLife>().IsDamagable) continue;

				if(newTargets.Count < LevelDesign.MaxRays)
					newTargets.Add(t);
				else
				{
					foreach(Transform nt in newTargets)
					{
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
			if(!nt.GetComponent<EnemyLife>().inLight)
				nt.GetComponent<EnemyLife>().OnLightEnter();
		}

		foreach(Transform t in targets)
		{
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
		Debug.Log ("LoseAllTargets");

		foreach (Transform t in targets)
			t.GetComponent<EnemyLife> ().OnLightExit ();

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

	private void Reset()
	{
		isSpecial = false;
		
		targets = new List<Transform> ();
		enemiesInRange = new List<Transform> ();
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.gameObject.layer != layerMask) return;

		if(!enemiesInRange.Contains(col.transform.parent))
		{
			enemiesInRange.Add (col.transform.parent);
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.gameObject.layer != layerMask) return;

		RemoveEnemyFromList (col.transform.parent.gameObject);
	}

	void RemoveEnemyFromList(GameObject enemy)
	{
		enemiesInRange.Remove (enemy.transform);

		if (targets.Remove (enemy.transform)) 
			enemy.GetComponent<EnemyLife>().OnLightExit();
	}

	//enemy collided with player finger
	void OnCollisionEnter2D(Collision2D col)
	{
		GameController.Instance.FingerHit ();
		
		col.gameObject.GetComponent<EnemyLife>().Dead(GameController.IsInvencible);
	}
}
