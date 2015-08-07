using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour 
{
	public string sceneToLoad;
	
	private AsyncOperation async;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (Load ());
	}
	
	private IEnumerator Load() 
	{
		async = Application.LoadLevelAsync(sceneToLoad);

		yield return async;
	}
}
