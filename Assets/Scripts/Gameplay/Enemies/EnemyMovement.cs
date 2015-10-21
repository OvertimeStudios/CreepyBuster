using UnityEngine;
using System.Collections;
using System;

public class EnemyMovement : MonoBehaviour 
{
	public static event Action<GameObject> OnOutOfScreen;

	protected EnemyLife enemyLife;
	protected Animator myAnimator;

	private float originalAnimatorSpeed;

	protected virtual void OnEnable()
	{
		GameController.OnSlowDownCollected += OnSlowDownCollected;
		GameController.OnSlowDownFade += OnSlowDownFade;
		GameController.OnFrozenCollected += OnFrozenCollected;
		GameController.OnFrozenFade += OnFrozenFade;
	}

	protected virtual void OnDisable()
	{
		GameController.OnSlowDownCollected -= OnSlowDownCollected;
		GameController.OnSlowDownFade -= OnSlowDownFade;
		GameController.OnFrozenCollected -= OnFrozenCollected;
		GameController.OnFrozenFade -= OnFrozenFade;
	}

	protected virtual void Start()
	{
		enemyLife = GetComponent<EnemyLife> ();

		if(transform.FindChild("Sprite") != null)
			myAnimator = transform.FindChild ("Sprite").GetComponent<Animator> ();

		if(myAnimator != null)
			originalAnimatorSpeed = myAnimator.speed;
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
		GameController.enemiesMissed++;

		if(OnOutOfScreen != null)
			OnOutOfScreen(gameObject);
		
		Destroy (gameObject);
	}

	private void OnFrozenCollected()
	{
		myAnimator.speed = 0;
	}

	private void OnFrozenFade()
	{
		myAnimator.speed = originalAnimatorSpeed;
	}

	private void OnSlowDownCollected()
	{
		myAnimator.speed *= SlowDown.SlowAmount;
	}
	
	private void OnSlowDownFade()
	{
		myAnimator.speed = originalAnimatorSpeed;
	}
}
