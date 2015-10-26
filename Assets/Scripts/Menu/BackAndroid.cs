using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BackAndroid : MonoBehaviour 
{
	public EventDelegate onBackPressed;
	private List<EventDelegate> onBackPressedList;

	// Use this for initialization
	void Start () 
	{
		if(!onBackPressed.isValid)
			onBackPressedList = GetComponent<UIButton>().onClick;
		else
		{
			onBackPressedList = new List<EventDelegate> ();
			onBackPressedList.Add (onBackPressed);
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetButtonDown("Cancel") && !Popup.IsActive)
		{
		 	EventDelegate.Execute(onBackPressedList);
		}
	}
}
