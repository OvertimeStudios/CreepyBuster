using UnityEngine;
using System.Collections;

public class VersionControl : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		GetComponent<UILabel>().text = Application.version;
	}
}
