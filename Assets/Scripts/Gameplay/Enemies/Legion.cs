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
	private Rotate innerRotate;
	private Rotate outterRotate;

	protected override void OnEnable()
	{
		base.OnEnable ();

		EnemyLife.OnDied += OnDied;
		EnemyMovement.OnOutOfScreen += RemoveMinions;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
		GameController.OnFrozenCollected += ApplyFrozen;
		GameController.OnFrozenFade += RemoveFrozen;
	}
	
	protected override void OnDisable()
	{
		base.OnDisable ();

		EnemyLife.OnDied -= OnDied;
		EnemyMovement.OnOutOfScreen -= RemoveMinions;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
		GameController.OnFrozenCollected += ApplyFrozen;
		GameController.OnFrozenFade += RemoveFrozen;
	}

	// Use this for initialization
	protected override void Start () 
	{
		base.Start ();

		innerRotate = transform.FindChild ("Sprite").GetComponent<Rotate> ();
		outterRotate = GetComponent<Rotate> ();

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

	void OnDied(GameObject enemy)
	{
		if (enemy.Equals (gameObject)) 
		{
			innerRotate.StopSmooth();
			outterRotate.StopSmooth();

			if(dropMinionsOnDeath)
			{
				foreach(Transform t in transform.FindChild("Minions"))
				{
					t.parent = transform.parent;
					t.GetComponent<EnemyMovement>().enabled = false;
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

	private void ApplySlow()
	{
		innerRotate.rotVel *= SlowDown.SlowAmount;
		outterRotate.rotVel *= SlowDown.SlowAmount;
	}
	
	private void RemoveSlow()
	{
		innerRotate.rotVel = innerRotate.originalVel;
		outterRotate.rotVel = innerRotate.originalVel;
	}

	private void ApplyFrozen()
	{
		innerRotate.rotVel = 0;
		outterRotate.rotVel = 0;
	}
	
	private void RemoveFrozen()
	{
		innerRotate.rotVel = innerRotate.originalVel;
		outterRotate.rotVel = innerRotate.originalVel;
	}
}
