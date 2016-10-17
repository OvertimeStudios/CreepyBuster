using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour 
{
	public string snapshotSaveName = "insert_snapshot_name_here";

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

		DataCloudPrefs.Load(snapshotSaveName);

		StartCoroutine (Load ());
	}
	
	private IEnumerator Load() 
	{
		float startLoadTime = Time.time;

		async = Application.LoadLevelAsync(sceneToLoad);
		async.allowSceneActivation = false;

		//wait for google play finished loading
		while(!DataCloudPrefs.IsLoaded)
			yield return null;

		async.allowSceneActivation = true;

		Debug.Log("Loading Complete in " + (Time.time - startLoadTime) + " seconds.");
	}

	void Update()
	{
		Debug.Log(async.progress);
	}
}
