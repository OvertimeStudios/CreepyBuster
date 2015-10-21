using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class EnemyLife : MonoBehaviour 
{
	#region actions
	public static event Action<GameObject> OnDied;
	#endregion

	private bool alreadyDead;

	public float life;
	public int score;
	public bool countAsStreak = true;
	[HideInInspector]
	public bool countAsKill = true;

	public bool destroyUponDeath = true;
	public float deathTime = 1f;

	[HideInInspector]
	public bool inLight = false;

	private GameObject lightning;
	private SpriteRenderer spriteRenderer;
	protected List<SpriteRenderer> brilhos;
	public Color basicColor = Color.yellow;
	public Color damageColor = Color.red;

	public GameObject explosion;

	[Header("Item")]
	public float chanceToDrop;
	public List<ItemPercent> itens;

	public List<SpriteRenderer> spritesToWhite;

	//public List<EventDelegate> onDeadEvent;

	#region get / set
	public bool IsOffscreen
	{
		get
		{
			Bounds bounds = spriteRenderer.bounds;

			Vector3 max = Camera.main.WorldToViewportPoint(bounds.max);
			Vector3 min = Camera.main.WorldToViewportPoint(bounds.min);

			return max.x < 0 || max.y < 0 || min.x > 1 || min.y > 1;
		}
	}

	public virtual bool IsDamagable
	{
		get { return !IsOffscreen; }
	}

	public bool IsDead
	{
		get
		{
			return life <= 0;
		}
	}
	#endregion

	protected virtual void OnEnable()
	{
		AttackTargets.OnSpecialStarted += UpdateColor;
		AttackTargets.OnSpecialEnded += UpdateColor;
		LevelDesign.OnPlayerLevelUp += UpdateColor;
		GameController.OnLoseStacks += UpdateColor;
	}

	protected virtual void OnDisable()
	{
		AttackTargets.OnSpecialStarted -= UpdateColor;
		AttackTargets.OnSpecialEnded -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		GameController.OnLoseStacks -= UpdateColor;
	}

	// Use this for initialization
	protected virtual void Start () 
	{
		alreadyDead = false;
		spriteRenderer = transform.FindChild ("Sprite").GetComponent<SpriteRenderer> ();

		if(!spritesToWhite.Contains(spriteRenderer))
			spritesToWhite.Add(spriteRenderer);

		brilhos = new List<SpriteRenderer> ();

		//get all spriterenderer children
		foreach (Transform t in spriteRenderer.transform)
			brilhos.Add (t.GetComponent<SpriteRenderer>());

		foreach(SpriteRenderer brilho in brilhos)
			brilho.color = basicColor;

		lightning = transform.FindChild ("Lightning Emitter").gameObject;
		lightning.SetActive (false);
		lightning.GetComponent<LightningBolt> ().target = AttackTargets.Instance.transform;
		lightning.GetComponent<ParticleRenderer>().material.SetColor ("_TintColor", LevelDesign.CurrentColor);

		life += LevelDesign.EnemiesBonusLife;
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		if(inLight)
		{
			//game stats
			GameController.energySpent += Time.deltaTime;

			life -= AttackTargets.Damage * Time.deltaTime;

			if(life <= 0)
				Dead();
		}
	}

	public void OnLightEnter()
	{
		inLight = true;

		foreach(SpriteRenderer brilho in brilhos)
		{
			Color c = damageColor;
			c.a = brilho.color.a;
			brilho.color = c;
		}

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.AttackStart);

		lightning.SetActive (true);
	}

	public void OnLightExit()
	{
		inLight = false;

		if(!IsDead)
		{
			//return back to normal color
			foreach(SpriteRenderer brilho in brilhos)
			{
				Color c = basicColor;
				c.a = brilho.color.a;
				brilho.color = c;
			}
		}

		lightning.SetActive (false);
	}

	private void UpdateColor()
	{
		if(IsDead) return;

		lightning.GetComponent<ParticleRenderer>().material.SetColor ("_TintColor", LevelDesign.CurrentColor);
	}

	public void Dead()
	{
		Dead (true);
	}

	public virtual void Dead(bool countPoints)
	{
		if(alreadyDead) return;

		alreadyDead = true;

		foreach(SpriteRenderer brilho in brilhos)
		{
			Color c = damageColor;
			c.a = brilho.color.a;
			brilho.color = c;
		}

		foreach (Collider2D col in GetComponentsInChildren<Collider2D>())
			col.enabled = false;

		countAsKill = countPoints;
		
		StartCoroutine (FadeAway (deathTime));

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Score);

		if (OnDied != null)
			OnDied (gameObject);
	}

	protected virtual IEnumerator FadeAway (float deathTime)
	{
		Animator animator = spriteRenderer.GetComponent<Animator> ();
		float maxAnimatorSpeed = animator.speed;

		//vanish damage conter
		while(spriteRenderer.material.GetFloat("_FlashAmount") < 1)
		{
			foreach(SpriteRenderer brilho in brilhos)
			{
				Color c = brilho.color;
				c.a -= Time.deltaTime / (deathTime);
				brilho.color = c;

				if(animator.recorderMode != AnimatorRecorderMode.Offline)
					animator.speed -= Time.deltaTime * deathTime * maxAnimatorSpeed;
			}

			//make it white
			float amount = spriteRenderer.material.GetFloat("_FlashAmount");
			amount += Time.deltaTime / (deathTime);
			
			foreach(SpriteRenderer sp in spritesToWhite)
				sp.material.SetFloat("_FlashAmount", amount);

			yield return null;
		}

		//make it white
		/*while(spriteRenderer.material.GetFloat("_FlashAmount") < 1)
		{
			float amount = spriteRenderer.material.GetFloat("_FlashAmount");
			amount += Time.deltaTime / (deathTime / 2);

			foreach(SpriteRenderer sp in spritesToWhite)
				sp.material.SetFloat("_FlashAmount", amount);

			yield return null;
		}*/

		spriteRenderer.material.SetFloat("_FlashAmount", 1);

		//Time.timeScale = 1f;

		if(GameController.isGameRunning)
		{
			SpawnItem ();
			DropOrbs();
		}

		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.EnemyDie);

		if(destroyUponDeath)
		{
			if(explosion != null)
				Instantiate(explosion, transform.position, Quaternion.identity);

			foreach(SpriteRenderer sp in spritesToWhite)
				Destroy (sp.transform.parent.gameObject);
		}
	}

	//Boss Only
	protected virtual void DropOrbs() { }

	private void SpawnItem()
	{
		float rnd = UnityEngine.Random.Range (0f, 1f);

		if(rnd < chanceToDrop)
		{
			float maxPercent = 0;

			foreach(ItemPercent ip in itens)
				maxPercent += ip.percent;

			rnd = UnityEngine.Random.Range(0f, maxPercent);

			float currentPercent = 0f;
			GameObject objToSpawn =  null;
			
			foreach(ItemPercent ip in itens)
			{
				currentPercent += ip.percent;
				
				if(rnd <= currentPercent)
				{
					objToSpawn = ip.item;
					break;
				}
			}

			SpawnController.Instance.SpawnItem(transform.position, objToSpawn);
		}
	}
}
