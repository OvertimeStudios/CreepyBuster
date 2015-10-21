using UnityEngine;
using System.Collections;

public class StarRandomRotation : MonoBehaviour 
{
	// Use this for initialization
	void Start () 
	{
		transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
	}
	

}
