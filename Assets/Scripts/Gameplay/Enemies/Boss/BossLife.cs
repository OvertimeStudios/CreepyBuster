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

	public override void Dead (bool countPoints)
	{
		base.Dead (countPoints);
		
		if(OnBossDied != null)
			OnBossDied();
	}
	
	protected override void DropOrbs()
	{
		SpawnController.Instance.SpawnOrbs(orbsToDrop, transform.position, spread);
	}
}
