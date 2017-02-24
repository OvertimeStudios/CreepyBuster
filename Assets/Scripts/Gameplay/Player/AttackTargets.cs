using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackTargets : MonoBehaviour 
{
	#region action
	public static event Action OnSpecialStarted;
	public static event Action OnSpecialEnded;
	#endregion

	public static event Action<float> OnSpecialTimerUpdated;

	private static List<Transform> targets;

	private static List<Transform> enemiesInRange;
	
	private float specialCounter;

	private static bool isSpecial;
	private Transform myTransform;

	private int layerMask;

	private AudioSource audioSourceAttack;
	private AudioSource audioSourceSpecial;
	private bool isAttacking;

	public float damage;
	public CircleCollider2D attackCollider;
	public CircleCollider2D collisionCollider;

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

	public static float Damage
	{
		get
		{
			return Instance.damage + ((IsSpecialActive) ? LevelDesign.Instance.gameBalance.specialAttributes.bonusDamage : 0);
		}
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
		ConsumablesController.OnAnyItemUsed += RemoveCollider;
		ConsumablesController.OnAllItensUsed += RestoreCollider;
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
		GameController.OnGameStart -= GetDamage;
		GameController.OnGameStart -= GetRange;
		GameController.OnFingerHit -= OnFingerHit;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;
		ConsumablesController.OnAnyItemUsed -= RemoveCollider;
		ConsumablesController.OnAllItensUsed -= RestoreCollider;

		LoseAllTargets();
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;

		layerMask = 1 << LayerMask.NameToLayer ("AttackCollider");

		isAttacking = false;
		isSpecial = false;

		myTransform = transform;

		AudioSource[] audioSources = GetComponents<AudioSource>();
		audioSourceAttack = audioSources[0];
		audioSourceSpecial = audioSources[1];

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
		{
			GameController.timeOnSpecial6 += Time.deltaTime;
			RunTimer ();
		}
		else
		{
			if(LevelDesign.PlayerLevel == 0)
				GameController.timeOnSpecial1 += Time.deltaTime;
			else if(LevelDesign.PlayerLevel == 1)
				GameController.timeOnSpecial2 += Time.deltaTime;
			else if(LevelDesign.PlayerLevel == 2)
				GameController.timeOnSpecial3 += Time.deltaTime;
			else if(LevelDesign.PlayerLevel == 3)
				GameController.timeOnSpecial4 += Time.deltaTime;
			else if(LevelDesign.PlayerLevel == 4)
				GameController.timeOnSpecial5 += Time.deltaTime;
		}
	}

	private void GetEnemiesInRange()
	{
		enemiesInRange.Clear();

		if(!attackCollider.enabled) return;

		Collider2D[] colliders = new Collider2D[50];
		int enemiesInRangeCount = Physics2D.OverlapCircleNonAlloc((Vector2)myTransform.position + attackCollider.offset, attackCollider.radius, colliders, layerMask);

		for(int i = 0; i < enemiesInRangeCount; i++)
			enemiesInRange.Add (colliders[i].transform.parent);
	}

	private void GetTargets ()
	{
		GetEnemiesInRange();

		List<Transform> newTargets = new List<Transform> ();

		if (GameController.gameOver) 
		{
			//do nothing
		}
		else
		{
			//get closest targets
			foreach(Transform t in enemiesInRange)
			{
				if(t == null) continue;

				//don't apply damage to those enemies who doesn't show up yet or are dead
				if(!t.GetComponent<EnemyLife>().IsDamagable) continue;

				//LevelDesign.MaxRays is handling the extra ray from special
				if(newTargets.Count < LevelDesign.MaxRays)
					newTargets.Add(t);
				else
				{
					foreach(Transform nt in newTargets)
					{
						if(nt == null) continue;

						//get closer enemy
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

		//if they are targeting and not in new targets, it means they are not going to be attacked anymore, so call OnLightExit
		foreach(Transform t in targets)
		{
			if(t == null) continue;

			if(!newTargets.Contains(t))
				t.GetComponent<EnemyLife>().OnLightExit();
		}

		targets = newTargets;

		//start attacking
		if(!isAttacking && targets.Count > 0)
		{
			isAttacking = true;

			if(Global.IsSoundOn)
				audioSourceAttack.Play();
		}
		//stop attacking
		else if(isAttacking && targets.Count == 0)
		{
			isAttacking = false;

			if(Global.IsSoundOn)
				audioSourceAttack.Stop();
		}
	}

	public void UseSpecial()
	{
		if(Global.IsSoundOn)
		{
			audioSourceSpecial.mute = false;
			audioSourceSpecial.Stop ();
			audioSourceSpecial.Play ();

			if(GameController.IsInvencible)
			{
				audioSourceSpecial.mute = true;

				StartCoroutine (WaitForEndInvincible());
			}
		}

		isSpecial = true;
		specialCounter = LevelDesign.Instance.gameBalance.specialAttributes.duration;
		GameController.specialStreak++;

		if(OnSpecialStarted != null)
			OnSpecialStarted();
	}

	private IEnumerator WaitForEndInvincible()
	{
		while(GameController.IsInvencible)
			yield return null;

		audioSourceSpecial.mute = false;
	}

	private void RunTimer()
	{
		specialCounter -= Time.deltaTime;

		if (specialCounter <= 0)
			StopSpecial ();

		if (OnSpecialTimerUpdated != null)
			OnSpecialTimerUpdated (specialCounter / LevelDesign.Instance.gameBalance.specialAttributes.duration);
	}

	private void LoseAllTargets()
	{
		if(targets == null) return;

		foreach (Transform t in targets)
		{
			if(t == null) continue;

			t.GetComponent<EnemyLife> ().OnLightExit ();
		}

		targets.Clear();
	}

	private IEnumerator StopSpecial(float waitTime)
	{
		yield return new WaitForSeconds (waitTime);

		StopSpecial ();
	}

	public void StopSpecial()
	{
		isSpecial = false;

		if(OnSpecialEnded != null)
			OnSpecialEnded();
	}

	private void OnFingerHit()
	{	
		if(LevelDesign.PlayerLevel == 0 && !GameController.IsShieldActive)
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
		isAttacking = false;
		isSpecial = false;

		if(targets != null)
			targets.Clear ();

		if(enemiesInRange != null)
			enemiesInRange.Clear ();
	}

	//enemy collided with player finger
	void OnCollisionEnter2D(Collision2D col)
	{
		GameController.Instance.FingerHit (col.transform.gameObject);

		if(col.gameObject.GetComponent<EnemyLife>() != null)
		{
			if(!GameController.IsBossTime)
				col.gameObject.GetComponent<EnemyLife>().Dead(false);
		}
	}

	private void RemoveCollider()
	{
		attackCollider.enabled = false;
		collisionCollider.enabled = false;
	}

	private void RestoreCollider()
	{
		attackCollider.enabled = true;
		collisionCollider.enabled = true;
	}
}
