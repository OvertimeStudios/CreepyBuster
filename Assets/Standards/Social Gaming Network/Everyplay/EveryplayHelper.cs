using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EveryplayHelper : MonoBehaviour 
{
	private static bool isRecorded;
	public static bool IsRecorded
	{
		get { return isRecorded; }
	}

	public static void SetLowMemoryDevice(bool state)
	{
		Everyplay.SetLowMemoryDevice(state);
	}

	public static void StartRecording()
	{
		StartRecording(0);
	}

	public static void StartRecording(int secondsToRecord)
	{
		isRecorded = false;

		if(!Everyplay.IsRecording() && Everyplay.IsReadyForRecording())
		{
			if(secondsToRecord > 0)
				Everyplay.SetMaxRecordingSecondsLength(secondsToRecord);

			Everyplay.StartRecording();
			isRecorded = true;
		}
	}

	public static void PauseRecording()
	{
		Everyplay.PauseRecording();
	}

	public static void ResumeRecording()
	{
		Everyplay.ResumeRecording();
	}

	public static void StopRecording()
	{
		Everyplay.StopRecording();
	}

	public static void StopRecording(string key, object val)
	{
		Everyplay.SetMetadata(key, val);

		Everyplay.StopRecording();
	}

	public static void StopRecording(Dictionary<string, object> metadata)
	{
		Everyplay.SetMetadata(metadata);

		Everyplay.StopRecording();
	}

	public static void OpenShareOptions()
	{
		OpenShareOptions(null);
	}

	public static void OpenShareOptions(string key, object val)
	{
		Everyplay.SetMetadata(key, val);

		Everyplay.ShowSharingModal();
	}

	public static void OpenShareOptions(Dictionary<string, object> metadata)
	{
		if(metadata != null)
			Everyplay.SetMetadata(metadata);

		Everyplay.ShowSharingModal();
	}

	public static void OpenFeed()
	{
		Everyplay.ShowWithPath("/feed/game");
	}

	public static void PlayLastRecording()
	{
		Everyplay.PlayLastRecording();
	}

	public static void PlayLastRecording(string key, object val)
	{
		Everyplay.SetMetadata(key, val);

		Everyplay.PlayLastRecording();
	}

	public static void PlayLastRecording(Dictionary<string, object> metadata)
	{
		if(metadata != null)
			Everyplay.SetMetadata(metadata);

		Everyplay.PlayLastRecording();
	}
}
