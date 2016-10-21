using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour 
{
	public string videoName;
	public string sceneToLoad;

	private bool readyForChangeScene;
	#if UNITY_ANDROID
	private bool isOpenGLReady;
	#endif

	private AsyncOperation async;

	// Use this for initialization
	void Start () 
	{
		isOpenGLReady = false;
		readyForChangeScene = false;

		StartCoroutine(LoadNextLevel());
		StartCoroutine(PlayFullScreenMovie ());
	}

	IEnumerator LoadNextLevel()
	{
		if (sceneToLoad != "")
			async = SceneManager.LoadSceneAsync (sceneToLoad);
		else
			async = SceneManager.LoadSceneAsync (1);

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
