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
	IEnumerator Start () 
	{
		#if !UNITY_WEBPLAYER
		//logo.SetActive(false);
		#endif

		yield return new WaitForSeconds(1f);
		StartCoroutine (Load ());
	}
	
	private IEnumerator Load() 
	{
		float startLoadTime = Time.time;

		Debug.Log("Loading next scene");

		yield return new WaitForEndOfFrame();

		async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneToLoad);
		async.allowSceneActivation = false;

		while(async.progress < 0.9f)
			yield return null;

		Debug.Log("DataCloudPrefs.Load from LOADING SCREEN");

		DataCloudPrefs.Load(snapshotSaveName);

		//wait for google play finished loading
		while(!DataCloudPrefs.IsLoaded)
			yield return null;

		async.allowSceneActivation = true;

		Debug.Log("Loading Complete in " + (Time.time - startLoadTime) + " seconds.");
	}

	void Update()
	{
		//if(async != null)
			//Debug.Log(async.progress);
	}
}
