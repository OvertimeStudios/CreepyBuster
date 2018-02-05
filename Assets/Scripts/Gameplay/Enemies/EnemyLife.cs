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

	public EnemiesPercent.EnemyNames type;

	public float life;
	public int score;
	public bool countAsStreak = true;
	[HideInInspector]
	public bool countAsKill = true;

	public bool runDeathAnimation = true;
	public bool destroyUponDeath = true;
	public float deathTime = 1f;
	private bool playSound = true;

	[HideInInspector]
	public bool inLight = false;

	private GameObject lightning;
	private Coroutine lightningAnimation;
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
		TouchPressure.On3DTouchStart += UpdateColor;
		TouchPressure.On3DTouchEnd += UpdateColor;
	}

	protected virtual void OnDisable()
	{
		AttackTargets.OnSpecialStarted -= UpdateColor;
		AttackTargets.OnSpecialEnded -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		GameController.OnLoseStacks -= UpdateColor;
		TouchPressure.On3DTouchStart -= UpdateColor;
		TouchPressure.On3DTouchEnd -= UpdateColor;
	}

	// Use this for initialization
	protected virtual void Start () 
	{
		//load stats
		EnemyStats stats = LevelDesign.GetEnemyStats(type);
		if(stats != null)
		{
			life = stats.life;
			score = stats.score;
			chanceToDrop = stats.chanceToDrop;
			itens = stats.itens;
		}

		alreadyDead = false;
		spriteRenderer = transform.Find ("Sprite").GetComponent<SpriteRenderer> ();

		if(!spritesToWhite.Contains(spriteRenderer))
			spritesToWhite.Add(spriteRenderer);

		brilhos = new List<SpriteRenderer> ();

		//get all spriterenderer children
		foreach (Transform t in spriteRenderer.transform)
			brilhos.Add (t.GetComponent<SpriteRenderer>());

		foreach(SpriteRenderer brilho in brilhos)
			brilho.color = basicColor;

		lightning = transform.Find ("Lightning Emitter").gameObject;
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

		//do a lerp to give player feeling the lightning is going from his finger to enemy
		lightningAnimation = StartCoroutine(AnimateLightning());
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

		if(lightningAnimation != null)
			StopCoroutine(lightningAnimation);

	}


	/// <summary>
	/// Animates the lightning. Do a lerp to give player feeling the lightning is going from his finger to enemy
	/// </summary>
	/// <returns>The lightning.</returns>
	private IEnumerator AnimateLightning()
	{
		lightning.transform.position = AttackTargets.Instance.transform.position;

		Transform to = (type == EnemiesPercent.EnemyNames.Spiral) ? spriteRenderer.transform : transform;

		while(Vector3.Distance(AttackTargets.Instance.transform.position, to.position) > 0.3f)
		{
			lightning.transform.position = Vector3.Lerp(lightning.transform.position, to.position, 0.25f);
			yield return null;
		}

		lightning.transform.position = to.position;
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
		Dead(countPoints, true);
	}

	public void Dead(bool countPoints, bool playSound)
	{
		if(alreadyDead) return;

		this.playSound = playSound;
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


		if(runDeathAnimation)
			StartCoroutine(RunDeathAnimation());
		else
			StartCoroutine (FadeAway (deathTime));

		if(playSound)
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

		if(playSound)
			SoundController.Instance.PlaySoundFX(SoundController.SoundFX.EnemyDie);

		if(destroyUponDeath)
		{
			if(explosion != null)
				Instantiate(explosion, transform.position, Quaternion.identity);

			foreach(SpriteRenderer sp in spritesToWhite)
				Destroy (sp.transform.parent.gameObject);
		}
	}

	protected IEnumerator RunDeathAnimation()
	{
		foreach(SpriteRenderer brilho in brilhos)
			brilho.gameObject.SetActive(false);

		Animator animator = spriteRenderer.GetComponent<Animator> ();
		animator.SetBool("IsDead", true);

		yield return new WaitForEndOfFrame();

		while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 || animator.IsInTransition(0))
			yield return null;

		if(GameController.isGameRunning)
		{
			SpawnItem ();
			DropOrbs();
		}

		if(playSound)
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

	//TODO: verify this bug.
	//this shouldn't be here, but sometimes OnDied isn't called, so we force call OnDestroy
	void OnDestroy()
	{
		if(IsDead) return;

		//if (OnDied != null)
			//OnDied (gameObject);
	}
}
