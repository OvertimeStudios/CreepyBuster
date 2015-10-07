using UnityEngine;
using System.Collections;

public class EveryplayButtonBehaviour : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		gameObject.SetActive(Everyplay.IsSupported());
	}
}
