using UnityEngine;
using System.Collections;

public class StarsParallax : MonoBehaviour 
{
	public Transform reference;
	public float parallax;

	private float initialPosition;
	private float initialReferencePosition;
	private Transform myTransform;

	[Header("Spawn")]
	public GameObject star;
	public int quantity;
	public float scale;

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		initialReferencePosition = reference.position.x;
		initialPosition = myTransform.position.x;

		for(byte i = 0; i < quantity; i++)
		{
			GameObject s = Instantiate(star);
			s.transform.parent = transform;

			Vector3 pos = new Vector3(Random.Range(-700f, 700f), Random.Range(-400f, 400f));
			s.transform.localPosition = pos;

			s.transform.localScale = Vector3.one * scale;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = myTransform.position;
		pos.x = initialPosition + ((reference.position.x - initialReferencePosition) * parallax);
		myTransform.position = pos;
	}
}
