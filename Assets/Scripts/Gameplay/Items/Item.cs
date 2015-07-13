using UnityEngine;
using System.Collections;
using System;

public class Item : MonoBehaviour 
{
	public static event Action<Type> OnItemCollected;

	public enum Type
	{
		Invecibility,
		LevelUp,
		DeathRay,
		SlowDown,
		PlasmaOrb,
		PlasmaOrb5,
		PlasmaOrb15,
		PlasmaOrb50,
	}
	
	public Type type;

	void Start()
	{

	}
}
