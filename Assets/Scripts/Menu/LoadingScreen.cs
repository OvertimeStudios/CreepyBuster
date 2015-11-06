using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour 
{
	public string sceneToLoad;
	
	private AsyncOperation async;

	[Header("Web")]
	public GameObject logo;

	// Use this for initialization
	void Start () 
	{
		#if !UNITY_WEBPLAYER
		logo.SetActive(false);
		#endif

		StartCoroutine (Load ());
	}
	
	private IEnumerator Load() 
	{
		float startLoadTime = Time.time;

		async = Application.LoadLevelAsync(sceneToLoad);

		yield return async;

		Debug.Log("Loading Complete in " + (Time.time - startLoadTime) + " seconds.");
	}

	void Update()
	{
		Debug.Log(async.progress);
	}
}
