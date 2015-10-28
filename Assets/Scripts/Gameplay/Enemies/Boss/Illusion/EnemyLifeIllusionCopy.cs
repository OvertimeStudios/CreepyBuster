using UnityEngine;
using System.Collections;

public class EnemyLifeIllusionCopy : EnemyLife 
{
	[Header("Level Design")]
	public float lifeToAdd = 5f;

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
