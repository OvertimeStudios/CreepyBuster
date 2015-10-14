using UnityEngine;
using System.Collections;

public class OutOfScreenDetector : MonoBehaviour 
{
	public bool destroyOutOfScreen = true;
	public bool destroyOnCollision = true;

	protected virtual void Start()
	{

	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
		
		if (pos.x < -0.3f || pos.x > 1.3f || pos.y < -0.3f || pos.y > 1.3f)
			OutOfScreen ();
	}
	
	public void OutOfScreen()
	{
		if(destroyOutOfScreen)
			Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(destroyOnCollision)
			Destroy (gameObject);
	}
}
