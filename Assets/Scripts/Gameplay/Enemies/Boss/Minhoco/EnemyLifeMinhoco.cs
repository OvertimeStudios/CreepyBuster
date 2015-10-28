using UnityEngine;
using System.Collections;

public class EnemyLifeMinhoco : BossLife 
{
	private BossMinhoco bossMinhoco;

	public override bool IsDamagable 
	{
		get 
		{
			return base.IsDamagable && bossMinhoco.corpoPieces.Count == 2;
		}
	}

	protected override void Start ()
	{
		base.Start ();

		partsToDestroy = 2;
		bossMinhoco = transform.parent.GetComponent<BossMinhoco>();

		life += lifeToAdd * GameController.boss2Killed;
	}
}
