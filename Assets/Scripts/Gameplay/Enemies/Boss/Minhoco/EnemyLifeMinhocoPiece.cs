using UnityEngine;
using System;
using System.Collections;

public class EnemyLifeMinhocoPiece : EnemyLife 
{
	#region actions
	public static event Action<Transform> OnPieceDied;
	#endregion

	public override void Dead (bool countPoints)
	{
		base.Dead (countPoints);

		if(OnPieceDied != null)
			OnPieceDied(gameObject.transform);

		Rigidbody2D r = GetComponent<Rigidbody2D>();
		r.gravityScale = 0.5f;
		r.velocity = Vector2.up * 5f;
		r.isKinematic = false;
	}
}
