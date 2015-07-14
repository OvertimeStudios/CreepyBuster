using UnityEngine;
using System.Collections;

public class SlowDown : Item 
{
	public float slowAmount;
	public float slowTime;

	public static float SlowAmount
	{
		get { return Instance.slowAmount; }
	}

	public static float SlowTime
	{
		get { return Instance.slowTime; }
	}

	private static SlowDown instance;
	private static SlowDown Instance
	{
		get 
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<SlowDown>();

			return instance;
		}
	}
}
