using UnityEngine;
using System.Collections;

public class SlowDown : Item 
{
	public float slowAmount;
	public float slowTime;

	private static float staticSlowAmout;
	private static float staticSlowTime;

	public static float SlowAmount
	{
		get { return staticSlowAmout; }
	}

	public static float SlowTime
	{
		get { return staticSlowTime; }
	}

	protected override void Start ()
	{
		base.Start ();

		staticSlowAmout = slowAmount;
		staticSlowTime = slowTime;
	}
}
