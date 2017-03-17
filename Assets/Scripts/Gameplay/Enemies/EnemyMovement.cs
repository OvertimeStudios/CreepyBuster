using UnityEngine;
using System.Collections;
using System;

public class EnemyMovement : MonoBehaviour 
{
	public static event Action<GameObject> OnOutOfScreen;

	protected EnemyLife enemyLife;
	protected Animator myAnimator;

	[HideInInspector]
	public float originalAnimatorSpeed;

	protected virtual void OnEnable()
	{
		GameController.OnSlowDownCollected += OnSlowDownCollected;
		GameController.OnSlowDownFade += OnSlowDownFade;
		GameController.OnFrozenCollected += ApplyFrozen;
		GameController.OnFrozenFade += RemoveFrozen;
		EnemyLife.OnDied += OnDied;
		ConsumablesController.OnAnyItemUsed += ApplyFrozen;
		ConsumablesController.OnAllItensUsed += RemoveFrozen;
	}

	protected virtual void OnDisable()
	{
		GameController.OnSlowDownCollected -= OnSlowDownCollected;
		GameController.OnSlowDownFade -= OnSlowDownFade;
		GameController.OnFrozenCollected -= ApplyFrozen;
		GameController.OnFrozenFade -= RemoveFrozen;
		EnemyLife.OnDied -= OnDied;
		ConsumablesController.OnAnyItemUsed -= ApplyFrozen;
		ConsumablesController.OnAllItensUsed -= RemoveFrozen;
	}

	protected virtual void Start()
	{
		enemyLife = GetComponent<EnemyLife> ();
	
		myAnimator = transform.GetComponentInChildren<Animator> ();

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

		if(GetComponent<EnemyLife>() != null)
			GetComponent<EnemyLife>().Dead(false, false);
	}

	private void ApplyFrozen()
	{
		if(myAnimator != null)
			myAnimator.speed = 0;
	}

	private void RemoveFrozen()
	{
		if(myAnimator != null)
		{
			if(!ConsumablesController.IsUsingConsumables && !GameController.IsFrozen)
			{
				//TODO: death animation not playing after frozen
				myAnimator.speed = originalAnimatorSpeed;
			}
		}
	}

	private void OnSlowDownCollected()
	{
		if(myAnimator != null)
			myAnimator.speed *= SlowDown.SlowAmount;
	}
	
	private void OnSlowDownFade()
	{
		if(myAnimator != null)
			myAnimator.speed = originalAnimatorSpeed;
	}

	private void OnDied(GameObject obj)
	{
		if(obj == gameObject)
		{
			Debug.Log("Is Dead!");

			if(GameController.IsFrozen && myAnimator != null)
				myAnimator.speed = originalAnimatorSpeed;
		}
	}
}
