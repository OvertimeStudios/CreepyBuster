using UnityEngine;
using System.Collections;

public class EnemyLifeCharge : EnemyLife 
{
	public override bool IsDamagable
	{
		get 
		{
			return base.IsDamagable && !GetComponent<ChargeBehaviour>().ChargeReleased;
		}
	}
}
