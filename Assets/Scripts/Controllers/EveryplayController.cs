using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EveryplayController : MonoBehaviour 
{
	#if EVERYPLAY_IMPLEMENTED
	public int minutesToRecord = 2;

	private static bool isReady;
	private static bool videoFinished;
	
	public static bool IsReady
	{
		get { Debug.Log(string.Format("isReady {0} && Everyplay.IsSupported()? {1} && Everyplay.IsRecordingSupported()? {2}", isReady, Everyplay.IsSupported(), Everyplay.IsRecordingSupported()));
			                return /*isReady &&*/ Everyplay.IsSupported() && Everyplay.IsRecordingSupported(); }
	}

	public static bool IsRecorded
	{
		get { return videoFinished; }
	}
	
	#region singleton
	private static EveryplayController instance;
	public static EveryplayController Instance
	{
		get { return instance; }
	}
	#endregion
	
	void Awake()
	{
		if(instance != null && instance != this)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Debug.Log("**** AWAKE EVERYPLAYCONTROLLER ****");
			Everyplay.ReadyForRecording += OnReadyForRecording;
			GameController.OnGameStart += StartRecording;
			GameController.OnPause += PauseRecording;
			GameController.OnResume += ResumeRecording;
			GameController.OnShowEndScreen += StopRecording;
			GameController.OnShowEndScreen += SetMetadata;

			instance = this;
		}

		transform.parent = null;
		DontDestroyOnLoad (gameObject);
	}
	
	void Start()
	{
		if(instance != null) return;
		Debug.Log("**** START EVERYPLAYCONTROLLER ****");

		EveryplayHelper.SetLowMemoryDevice(true);
		isReady = false;
		videoFinished = false;
	}
	
	private void OnReadyForRecording(bool enabled) 
	{
		Debug.Log("******OnReadyForRecording? " + enabled);
		
		isReady = enabled;
	}
	
	public static void StartRecording()
	{
		Debug.Log("Start Recording? " + IsReady);
		/*if(!Everyplay.IsRecording() && IsReady)
		{
			Everyplay.SetMaxRecordingMinutesLength(Instance.minutesToRecord);

			Everyplay.StartRecording();
			videoFinished = false;
		}*/

		EveryplayHelper.StartRecording((int)(Instance.minutesToRecord * 60f));
	}
	
	public static void StopRecording()
	{
		Debug.Log("Stop Recording");
		/*if(Everyplay.IsRecording())
		{
			Everyplay.StopRecording();
			videoFinished = true;
		}*/

		EveryplayHelper.StopRecording();
	}
	
	public static void PauseRecording()
	{
		/*if(!Everyplay.IsPaused())
			Everyplay.PauseRecording();*/

		EveryplayHelper.PauseRecording();
	}
	
	public static void ResumeRecording()
	{
		/*if(Everyplay.IsPaused())
			Everyplay.ResumeRecording();*/
		EveryplayHelper.ResumeRecording();
	}
	
	public static void OpenShareOptions()
	{
		
		OpenShareOptions(null);
	}
	
	public static void OpenShareOptions(Dictionary<string, object> metadata)
	{
		/*if(!videoFinished) return;
		
		if(metadata != null)
			Everyplay.SetMetadata(metadata);
		
		Everyplay.ShowSharingModal();*/

		EveryplayHelper.OpenShareOptions(metadata);
	}
	
	public static void OpenEveryplay()
	{
		EveryplayHelper.OpenFeed();
		//Everyplay.ShowWithPath("/feed/game");
	}
	
	public void PlayLastRecording()
	{
		GameController.watchedReplay = true;
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		Debug.Log("Video Recorded? " + IsRecorded);

		if(!EveryplayHelper.IsRecorded)
		{
			Popup.ShowOk(Localization.Get("EVERYPLAY_ERROR"));
			return;
		}
		PlayLastRecording(null);
	}
	
	public static void PlayLastRecording(Dictionary<string, object> metadata)
	{
		Debug.Log("Play Last Recording: " + videoFinished);
		
		/*if(!videoFinished) return;
		
		if(metadata != null)
			Everyplay.SetMetadata(metadata);
		
		Everyplay.PlayLastRecording();*/

		EveryplayHelper.PlayLastRecording(metadata);
	}

	private static void SetMetadata()
	{
		Everyplay.SetMetadata("score", GameController.Score);
	}
	#endif
}
