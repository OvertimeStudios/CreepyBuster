using UnityEngine;
using System.Collections;

public class Frozen : Item 
{
	public float frozenTime;

	private static float staticFrozenTime;
	
	public static float FrozenTime
	{
		get { return staticFrozenTime; }
	}
	
	protected override void Start ()
	{
		base.Start ();

		staticFrozenTime = frozenTime;
	}
}
