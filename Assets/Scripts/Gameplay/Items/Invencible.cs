using UnityEngine;
using System.Collections;

public class Invencible : Item
{
	public float invencibleTime;

	public static float Time
	{
		get { return Instance.invencibleTime; }
	}
	
	private static Invencible instance;
	private static Invencible Instance
	{
		get 
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<Invencible>();
			
			return instance;
		}
	}
}
