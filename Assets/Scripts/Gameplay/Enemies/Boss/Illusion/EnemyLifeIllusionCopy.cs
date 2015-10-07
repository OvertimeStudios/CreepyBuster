using UnityEngine;
using System.Collections;

public class EnemyLifeIllusionCopy : EnemyLife 
{
	public override bool IsDamagable 
	{
		get 
		{
			return base.IsDamagable && IllusionBoss.IsAttacking;
		}
	}
	
}
