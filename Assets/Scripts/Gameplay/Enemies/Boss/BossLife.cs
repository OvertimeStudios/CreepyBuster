using UnityEngine;
using System;
using System.Collections;

public class BossLife : EnemyLife 
{
	[Header("Drop")]
	public int orbsToDrop = 50;
	public float spread = 1f;

	[Header("Level Design")]
	public float lifeToAdd = 5f;

	public static int partsToDestroy;

	#region Action
	public static event Action<GameObject> OnBossDied;
	#endregion

	protected override void Start ()
	{
		base.Start ();

		partsToDestroy = 1;
	}

	protected override void DropOrbs()
	{
		partsToDestroy--;

		if(partsToDestroy == 0)
		{
			if(OnBossDied != null)
				OnBossDied(gameObject);
		}
	
		SpawnController.Instance.SpawnOrbs(orbsToDrop, transform.position, spread);
	}
}
