using UnityEngine;
using System.Collections;
using System;

public class Legion : EnemyMovement 
{
	public static Action<GameObject> OnMinionReleased;
	public static Action<GameObject> OnMinionSpawned;

	public GameObject minion;
	public int minionsQty;
	public int minionsPerRow;
	public float minionsDistance;
	public float rotVel;
	public bool dropMinionsOnDeath;

	private Transform spriteTransform;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
		EnemyMovement.OnOutOfScreen += RemoveMinions;
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
		EnemyMovement.OnOutOfScreen -= RemoveMinions;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		spriteTransform = transform;//.FindChild ("Sprite");

		int totalRows = Mathf.Min(minionsQty / minionsPerRow);

		for(byte j = 1; j <= totalRows; j++)
		{
			int minionsOnThisRow = Mathf.Min(minionsQty - ((j - 1)  * minionsPerRow), minionsPerRow);
			float initialSpawnAngle = ((float)j / (float)totalRows) * (360f / minionsOnThisRow);
			float distance = minionsDistance + (minionsDistance * 0.4f * (j - 1));

			for(byte i = 0; i < minionsOnThisRow; i++)
			{
				float angle = initialSpawnAngle + (i * (360f / minionsOnThisRow)) * Mathf.Deg2Rad;

				Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * distance;
				Quaternion rot = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f));

				GameObject obj = Instantiate(minion) as GameObject;
				obj.transform.parent = transform.FindChild("Minions");
				obj.transform.localPosition = pos;
				obj.transform.rotation = rot;

				if(OnMinionSpawned != null)
					OnMinionSpawned(obj);
			}
		}
	}
	
	// Update is called once per frame
	protected override void Update () 
	{
		base.Update ();

		spriteTransform.Rotate (Vector3.forward * rotVel);
	}

	void OnDied(GameObject enemy)
	{
		if (enemy.Equals (gameObject)) 
		{
			StartCoroutine (StopSpinning (EnemyLife.deathTime));

			if(dropMinionsOnDeath)
			{
				foreach(Transform t in transform.FindChild("Minions"))
				{
					t.parent = transform.parent;
					t.GetComponent<RandomMovement>().enabled = true;

					if(OnMinionReleased != null)
						OnMinionReleased(t.gameObject);
				}
			}
			else
			{
				RemoveMinions(gameObject);
			}
		}
	}

	private void RemoveMinions(GameObject obj)
	{
		if(obj == gameObject)
			RemoveMinions();
	}

	private void RemoveMinions()
	{
		foreach(Transform t in transform.FindChild("Minions"))
		{
			if(!t.GetComponent<EnemyLife>().IsDead)
				t.GetComponent<EnemyLife>().Dead(false);
		}
	}

	private IEnumerator StopSpinning(float waitTime)
	{
		float maxRotVel = rotVel;
		
		while(rotVel > 0)
		{
			rotVel -= Time.deltaTime * EnemyLife.deathTime * maxRotVel;
			
			yield return null;
		}
	}
}
