using UnityEngine;
using System.Collections;

public class SplashController : MonoBehaviour 
{
	public string videoName;
	public string sceneToLoad;

	private bool readyForChangeScene;

	private AsyncOperation async;

	// Use this for initialization
	void Start () 
	{
		readyForChangeScene = false;

		StartCoroutine(LoadNextLevel());
		StartCoroutine(PlayFullScreenMovie ());
	}

	IEnumerator LoadNextLevel()
	{
		if (sceneToLoad != "")
			async = Application.LoadLevelAsync (sceneToLoad);
		else
			async = Application.LoadLevelAsync (1);

		async.allowSceneActivation = false;

		while(async.progress < 0.9f || !readyForChangeScene)
			yield return null;

		Debug.Log("Changing scene");

		async.allowSceneActivation = true;
	}

	IEnumerator PlayFullScreenMovie()
	{
		#if !UNITY_WEBPLAYER
		Handheld.PlayFullScreenMovie (videoName, Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);
		#endif

		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();

		SwitchScene ();
	}

	private void SwitchScene()
	{
		Debug.Log("readyForChangeScene");

		readyForChangeScene = true;
	}
}
