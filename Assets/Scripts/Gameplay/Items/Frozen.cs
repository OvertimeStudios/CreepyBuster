using UnityEngine;
using System.Collections;

public class Frozen : Item 
{
	public float frozenTime;

	private static float staticFrozenTime;

	public static float FrozenTime
	{
		get 
		{
			if(staticFrozenTime == 0)
				return GameController.Instance.timeFrozen;

			return staticFrozenTime; 
		}
	}

	protected override void Start ()
	{
		base.Start ();

		staticFrozenTime = frozenTime;
	}
}
