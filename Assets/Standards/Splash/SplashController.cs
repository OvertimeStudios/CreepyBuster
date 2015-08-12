using UnityEngine;
using System.Collections;

public class SplashController : MonoBehaviour 
{
	public string videoName;
	public string sceneToLoad;

	private AsyncOperation async;

	// Use this for initialization
	void Start () 
	{
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

		yield return async;
	}

	IEnumerator PlayFullScreenMovie()
	{
		Handheld.PlayFullScreenMovie (videoName, Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);

		yield return new WaitForSeconds (3.0f);

		SwitchScene ();
	}

	private void SwitchScene()
	{
		Debug.Log("switching");
		
		if (async != null)
			async.allowSceneActivation = true;
	}
}
