using UnityEngine;
using System;
using System.Collections;

public class BossLife : EnemyLife 
{
	[Header("Drop")]
	public int orbsToDrop = 50;
	public float spread = 1f;

	#region Action
	public static event Action OnBossDied;
	#endregion
	
	protected override void DropOrbs()
	{
		if(OnBossDied != null)
			OnBossDied();

		SpawnController.Instance.SpawnOrbs(orbsToDrop, transform.position, spread);
	}
}
