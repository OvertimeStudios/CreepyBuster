using UnityEngine;
using System.Collections;

public class StarsParallax : MonoBehaviour 
{
	public Transform reference;
	public float parallax;

	private float initialPosition;
	private float initialReferencePosition;
	private Transform myTransform;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		initialReferencePosition = reference.position.x;
		initialPosition = myTransform.position.x;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = myTransform.position;
		pos.x = initialPosition + ((reference.position.x - initialReferencePosition) * parallax);
		myTransform.position = pos;
	}
}
