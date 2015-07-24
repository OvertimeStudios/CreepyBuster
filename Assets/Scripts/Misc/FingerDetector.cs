using UnityEngine;
using System;
using System.Collections;

public class FingerDetector : MonoBehaviour 
{
	public static event Action<FingerDownEvent> OnFingerDownEvent;
	public static event Action<FingerUpEvent> OnFingerUpEvent;
	public static event Action<FingerMotionEvent> OnFingerMotionEvent;

	private static bool fingerDown;
	private static Vector2 position;

	#region get / set
	public static bool IsFingerDown
	{
		get { return fingerDown; }
	}

	public static bool IsFingerUp
	{
		get { return !fingerDown; }
	}

	public static Vector2 FingerPosition
	{
		get { return position; }
	}
	#endregion

	void OnFingerDown(FingerDownEvent e)
	{
		fingerDown = true;

		position = e.Position;

		if (OnFingerDownEvent != null)
			OnFingerDownEvent (e);
	}
	
	void OnFingerUp(FingerUpEvent e)
	{
		fingerDown = false;

		position = e.Position;

		if (OnFingerUpEvent != null)
			OnFingerUpEvent (e);
	}

	void OnFingerMove(FingerMotionEvent e)
	{
		position = e.Position;

		if (OnFingerMotionEvent != null)
			OnFingerMotionEvent (e);
	}
}
