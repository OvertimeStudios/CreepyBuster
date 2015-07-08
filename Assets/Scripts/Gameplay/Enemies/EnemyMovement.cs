using UnityEngine;
using System.Collections;
using System;

public class EnemyMovement : MonoBehaviour 
{
	public static event Action<GameObject> OnOutOfScreen;

	protected EnemyLife enemyLife;

	protected virtual void Start()
	{
		enemyLife = GetComponent<EnemyLife> ();
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
		if(OnOutOfScreen != null)
			OnOutOfScreen(gameObject);
		
		Destroy (gameObject);
	}
}
