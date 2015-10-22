using UnityEngine;
using System.Collections;

public class EveryplayButtonBehaviour : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		#if EVERYPLAY_IMPLEMENTED
		gameObject.SetActive(Everyplay.IsSupported());
		#endif
	}
}
