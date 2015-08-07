using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossMeteoro : MonoBehaviour 
{
	public enum State
	{
		EyesOpen,
		EyesClosed,
	}

	public float deathTime;
	[HideInInspector]
	public State state;

	[Header("Meteor Rain")]
	public GameObject[] meteors;
	public BossMeteoroLevel[] bossLevel;

	private int level;
	private float totalLife;
	private float lifePerLevel;
	private Rigidbody2D myRigidbody2D;
	private EnemyLife enemyLife;
	private CameraShake cameraShake;
	private RandomMovement randomMovement;
	private Animator myAnimator;
	private List<GameObject> brilhos;
	private Transform brilhoOlho;
	private float brilhoOlhoOriginalScale;

	#region get / set
	private float CurrentVel
	{
		get { return bossLevel [level].vel; }
	}

	private int CurrentMeteorsQty
	{
		get { return bossLevel [level].meteorsQty; }
	}

	private float CurrentMeteorsTime
	{
		get { return bossLevel [level].meteorsTime; }
	}

	private bool IsLevelMax
	{
		get { return level == bossLevel.Length - 1; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		myRigidbody2D = GetComponent<Rigidbody2D> ();
		enemyLife = GetComponent<EnemyLife> ();
		cameraShake = GetComponent<CameraShake> ();
		randomMovement = GetComponent<RandomMovement> ();
		myAnimator = transform.FindChild("Sprite").GetComponent<Animator>();

		brilhos = new List<GameObject> ();
		
		foreach (Transform t in transform.FindChild("Sprite"))
		{
			if(t.gameObject.name.Contains("Olho"))
				brilhoOlho = t;
			else
				brilhos.Add (t.gameObject);
		}

		brilhos.Reverse();

		for (byte i = 0; i < brilhos.Count; i++)
			brilhos [i].SetActive (false);

		brilhoOlhoOriginalScale = brilhoOlho.transform.localScale.y;

		brilhoOlho.localScale = new Vector3(brilhoOlho.localScale.x, 0f, brilhoOlho.localScale.y);

		state = State.EyesClosed;

		level = 0;
		totalLife = enemyLife.life;
		lifePerLevel =  totalLife / bossLevel.Length;
		randomMovement.enabled = false;

		StartCoroutine(MeteorRain ());
	}

	private IEnumerator MeteorRain()
	{
		int meteorsSpawned = 0;

		while(meteorsSpawned < CurrentMeteorsQty)
		{
			SpawnMeteor();

			meteorsSpawned++;

			yield return new WaitForSeconds(CurrentMeteorsTime / CurrentMeteorsQty);
		}

		StartCoroutine(BackToScreen ());
	}

	private void SpawnMeteor()
	{
		Vector3 pos = new Vector3 (Random.Range (0.1f, 0.9f), 1.3f, 0);
		pos = Camera.main.ViewportToWorldPoint (pos);
		pos.z = 0f;

		Instantiate (meteors [(int)Random.Range (0, meteors.Length)], pos, Quaternion.identity);
	}

	private IEnumerator BackToScreen ()
	{
		Vector3 pos = new Vector3 (0.5f, 0.5f, 0);
		pos = Camera.main.ViewportToWorldPoint (pos);
		pos.z = transform.position.z;

		float angle = Mathf.Atan2 (pos.y - transform.position.y, pos.x - transform.position.x);

		myRigidbody2D.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * CurrentVel;

		while (Vector3.Distance(pos, transform.position) > 1)
			yield return null;

		myRigidbody2D.velocity = Vector2.zero;

		StartCoroutine(OpenEyes());
	}

	private IEnumerator OpenEyes()
	{
		myAnimator.SetBool("CloseEyes", false);

		//wait for animation clip change
		yield return new WaitForEndOfFrame();

		Vector3 scale = brilhoOlho.localScale;

		while(myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			scale.y = myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * brilhoOlhoOriginalScale;
			brilhoOlho.localScale = scale;

			yield return null;
		}

		scale = brilhoOlho.localScale;
		scale.y = brilhoOlhoOriginalScale;
		brilhoOlho.localScale = scale;

		state = State.EyesOpen;
		
		randomMovement.vel = CurrentVel;
		randomMovement.enabled = true;

		for (byte i = 0; i < brilhos.Count; i++)
			brilhos [i].SetActive (true);

		StartCoroutine(WaitForLifeLose());
	}

	private IEnumerator WaitForLifeLose()
	{
		while(enemyLife.life > totalLife - (lifePerLevel * (level + 1)))
		{
			for(int i = level * 2; i < (level + 1) * 2; i++)
			{
				Color c = brilhos[i].GetComponent<SpriteRenderer>().color;
				c.a = (enemyLife.life - (totalLife - (lifePerLevel * (level + 1))))/ lifePerLevel;
				brilhos[i].GetComponent<SpriteRenderer>().color = c;
			}

			yield return null;
		}

		state = State.EyesClosed;

		randomMovement.enabled = false;
		myRigidbody2D.velocity = Vector2.zero;

		if(!IsLevelMax)
		{
			cameraShake.Shake();

			yield return new WaitForSeconds(cameraShake.duration);

			StartCoroutine(CloseEyes());
		}
		else
		{
			GetComponentInChildren<Collider2D>().enabled = false;

			Time.timeScale = 0.2f;

			cameraShake.Shake(enemyLife.deathTime * 0.5f);
			ScreenFeedback.ShowBlank(enemyLife.deathTime * 0.5f, 0.5f);

			SpriteRenderer olho = brilhoOlho.GetComponent<SpriteRenderer>();

			while(olho.color.a > 0)
			{
				Color c = olho.color;
				c.a -= Time.deltaTime / (enemyLife.deathTime * 0.5f);
				olho.color = c;

				yield return null;
			}

			Time.timeScale = 1f;
		}
	}

	private IEnumerator CloseEyes()
	{
		myAnimator.SetBool("CloseEyes", true);
		
		//wait for animation clip change
		yield return new WaitForEndOfFrame();

		Vector3 scale = brilhoOlho.localScale;

		while(myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			scale.y = brilhoOlhoOriginalScale - (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime * brilhoOlhoOriginalScale);
			brilhoOlho.localScale = scale;
			
			yield return null;
		}

		scale = brilhoOlho.localScale;
		scale.y = 0;
		brilhoOlho.localScale = scale;

		for (byte i = 0; i < brilhos.Count; i++)
			brilhos [i].SetActive (false);
		
		StartCoroutine(GoOutOfScreen());
	}

	private IEnumerator GoOutOfScreen ()
	{
		randomMovement.enabled = false;

		Vector3 pos = new Vector3 (0.5f, 1.3f, 0);
		pos = Camera.main.ViewportToWorldPoint (pos);
		pos.z = transform.position.z;
		
		float angle = Mathf.Atan2 (pos.y - transform.position.y, pos.x - transform.position.x);
		
		myRigidbody2D.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * CurrentVel;
		
		while (Vector3.Distance(pos, transform.position) > 1)
			yield return null;
		
		myRigidbody2D.velocity = Vector2.zero;

		level++;

		StartCoroutine (MeteorRain ());
	}
}

[System.Serializable]
public class BossMeteoroLevel
{
	public float vel;
	public int meteorsQty;
	public float meteorsTime;
}
