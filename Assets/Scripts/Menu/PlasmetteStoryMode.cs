using UnityEngine;
using System.Collections;

public class PlasmetteStoryMode : MonoBehaviour 
{
	void OnFingerDown(FingerDownEvent e)
	{
		if(e.Selection == gameObject)
			MenuController.Instance.MoveToStoryMode();
	}
}
