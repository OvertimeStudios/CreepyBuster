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
		float startLoadTime = Time.time;

		async = Application.LoadLevelAsync(sceneToLoad);

		yield return async;

		Debug.Log("Loading Complete in " + (Time.time - startLoadTime) + " seconds.");
	}
}
