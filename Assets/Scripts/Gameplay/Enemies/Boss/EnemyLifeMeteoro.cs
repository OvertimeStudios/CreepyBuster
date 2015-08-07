using UnityEngine;
using System.Collections;

public class EnemyLifeMeteoro : BossLife 
{
	public override bool IsDamagable
	{
		get 
		{
			return base.IsDamagable && GetComponent<BossMeteoro>().state == BossMeteoro.State.EyesOpen;
		}
	}
}
