using UnityEngine;
using System.Collections;

public class EnemyLifeIllusion : BossLife 
{
	public override bool IsDamagable 
	{
		get 
		{
			return base.IsDamagable && IllusionBoss.IsAttacking;
		}
	}

}
