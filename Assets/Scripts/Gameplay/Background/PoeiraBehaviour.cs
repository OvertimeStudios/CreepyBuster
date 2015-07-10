using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PoeiraBehaviour : MonoBehaviour 
{
	public float vel;

	private SpriteRenderer spriteRenderer;
	private Bounds bounds;

	private static Rect screenBounds;

	private Rigidbody2D myRigidbody2D;

	// Use this for initialization
	void Start () 
	{
		spriteRenderer = GetComponent<SpriteRenderer> ();
		myRigidbody2D = GetComponent<Rigidbody2D> ();
		bounds = spriteRenderer.bounds;

		//calculate screen bounds
		Vector3 minBounds = Camera.main.ViewportToWorldPoint (Vector3.zero);
		Vector3 maxBounds = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0));

		screenBounds = new Rect (minBounds.x, maxBounds.y, maxBounds.x - minBounds.x, maxBounds.y - minBounds.y);

		RandomPosition ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		bounds = spriteRenderer.bounds;

		float posx = transform.position.x;

		if (posx < -bounds.size.x || posx > +bounds.size.x)
			PositionOutOfScreen ();
	}

	private void PositionOutOfScreen()
	{
		float randomX = Random.Range (0f, 1f);
		randomX = (randomX < 0.5f) ? -bounds.size.x : +bounds.size.x;

		float randomY = Random.Range (screenBounds.y, screenBounds.y - screenBounds.height);
		
		transform.position = new Vector3 (randomX, randomY, transform.position.z);
		
		Move ();
	}

	private void RandomPosition()
	{
		float randomX = Random.Range (-bounds.size.x, +bounds.size.x);
		float randomY = Random.Range (screenBounds.y, screenBounds.y - screenBounds.height);

		transform.position = new Vector3 (randomX, randomY, transform.position.z);

		Move ();
	}

	private void Move()
	{
		myRigidbody2D.velocity = (transform.position.x < 0) ? Vector3.right : -Vector3.right;
		myRigidbody2D.velocity *= vel;
	}
}
