using UnityEngine;
using System;
using System.Collections;

public class PlayerPosition : MonoBehaviour 
{
	#region Action
	public static Action<Vector2> OnUpdated;
	#endregion

	//make (0,0) at the middle of the screen
	private static Vector2 middleScreen;

	// Use this for initialization
	void Start () 
	{
		middleScreen = new Vector2(Screen.width, Screen.height) * 0.5f;
	}
	
	void Update()
	{
		//-1 to 1: 
		//-1 most left/bottom
		//1 most right/top

		Vector2 fingerPosition = (Vector2)Input.mousePosition - middleScreen;
		Vector2 viewportPosition = new Vector2(fingerPosition.x / middleScreen.x, fingerPosition.y / middleScreen.y);

		if(OnUpdated != null)
			OnUpdated(viewportPosition);
	}
}
