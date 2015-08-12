using UnityEngine;
using System;
using System.Collections;

public class BossLife : EnemyLife 
{
	#region Action
	public static event Action OnBossDied;
	#endregion

	public override void Dead (bool countPoints)
	{
		base.Dead (countPoints);
		
		if(OnBossDied != null)
			OnBossDied();
	}
}
