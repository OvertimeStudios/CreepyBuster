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

	protected override void Start ()
	{
		base.Start ();

		life += lifeToAdd * GameController.boss3Killed;
	}

}
