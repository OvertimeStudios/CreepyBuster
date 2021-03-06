﻿using UnityEngine;
using System.Collections;

public class BoomerangBehaviour : MonoBehaviour 
{
	private Rotate outterRotate;
	private Rotate innerRotate;
	private Animator myTailAnimator;

	private float originalInnerRotate;
	private float originalOutterRotate;
	private float originalTailAnimatorSpeed;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
		GameController.OnFrozenCollected += ApplyFrozen;
		GameController.OnFrozenFade += RemoveFrozen;
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
		GameController.OnFrozenCollected -= ApplyFrozen;
		GameController.OnFrozenFade -= RemoveFrozen;
	}

	// Use this for initialization
	void Start () 
	{
		if (GameController.IsSlowedDown)
			ApplySlow ();

		outterRotate = GetComponent<Rotate> ();
		innerRotate = transform.FindChild ("Sprite").GetComponent<Rotate> ();

		originalInnerRotate = innerRotate.rotVel;
		originalOutterRotate = outterRotate.rotVel;

		myTailAnimator = transform.FindChild ("Sprite").FindChild("Brilho").GetComponent<Animator> ();
		originalTailAnimatorSpeed = myTailAnimator.speed;
	}

	private void OnDied(GameObject enemy)
	{
		if(enemy == gameObject)
		{
			innerRotate.StopSmooth();
			outterRotate.StopSmooth();
		}
	}

	private void ApplySlow()
	{
		innerRotate.rotVel *= SlowDown.SlowAmount;
		outterRotate.rotVel *= SlowDown.SlowAmount;
	}
	
	private void RemoveSlow()
	{
		innerRotate.rotVel *= 1 / SlowDown.SlowAmount;
		outterRotate.rotVel *= 1 / SlowDown.SlowAmount;
	}

	private void ApplyFrozen()
	{
		innerRotate.rotVel = 0f;
		outterRotate.rotVel = 0f;
		myTailAnimator.speed = 0;
	}
	
	private void RemoveFrozen()
	{
		innerRotate.rotVel = originalInnerRotate;
		outterRotate.rotVel = originalOutterRotate;
		myTailAnimator.speed = originalTailAnimatorSpeed;
	}
}
