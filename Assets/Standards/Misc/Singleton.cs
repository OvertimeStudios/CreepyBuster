// ------------------------------------------------------------------------------------------------------------
// 							 Singleton.cs
// 				 Copyright (c) 2016 Dreams on Demand
//  	Authors: Leonardo M. Lopes <euleoo@gmail.com> - http://about.me/leonardo_lopes
// ------------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	protected Singleton(){}//Prevents other classes from creating

	private static T _instance;
	
	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (T) FindObjectOfType(typeof(T));
				
				if ( FindObjectsOfType(typeof(T)).Length > 1 )
				{
					Debug.LogError("[Singleton] Something went really wrong " +
					               " - there should never be more than 1 singleton!" +
					               " Reopening the scene might fix it.");
					return _instance;
				}
				
				if (_instance == null)
				{
					string prefabName = typeof(T).ToString();

					//Debug.Log("Prefab name: " + prefabName);

					Object prefab = Resources.Load<GameObject>( prefabName );
					GameObject singleton;

					//Prefab strategy
					if(prefab != null)
					{
						Debug.Log("[Singleton] An instance was created with a prefab found for it");
						singleton = Instantiate( prefab ) as GameObject;
						_instance = singleton.GetComponent<T>();
					}
					else//If we don't have a prefab for it, just create a new GO with the component attached
					{
						Debug.Log("[Singleton] An instance was created with NO prefab found for it");
						singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
					}

					singleton.name = typeof(T).ToString();

					Debug.Log("[Singleton] An instance of " + typeof(T) + 
					          " is needed in the scene, so '" + singleton +
					          "' was created with DontDestroyOnLoad.");
				} 
				else 
				{
					Debug.Log("[Singleton] Using instance already created: " +
					          _instance.gameObject.name);
				}

				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	public void Start(){
		HuntForDuplicates();
	}

	public void OnLevelWasLoaded(int level) 
	{
		HuntForDuplicates();			
	}

	void HuntForDuplicates()
	{
		//Debug.Log("Hunt for duplicates");

		T[] singletons = FindObjectsOfType(typeof(T)) as T[];

		int duplicateCount = 0;

		foreach(T singleton in singletons)
		{
			if(Instance != singleton)
			{
				Destroy(singleton.gameObject);
				duplicateCount++;
			}
		}

		if(duplicateCount > 0)
			Debug.Log("Destroyed " + duplicateCount + " duplicates");
	}
}