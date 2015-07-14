using UnityEngine;
using System.Collections;

public class BoomerangBehaviour : MonoBehaviour 
{
	private Rotate outterRotate;
	private Rotate innerRotate;
	private Animator myAnimator;

	void OnEnable()
	{
		EnemyLife.OnDied += OnDied;
		GameController.OnSlowDownCollected += ApplySlow;
		GameController.OnSlowDownFade += RemoveSlow;
	}
	
	void OnDisable()
	{
		EnemyLife.OnDied -= OnDied;
		GameController.OnSlowDownCollected -= ApplySlow;
		GameController.OnSlowDownFade -= RemoveSlow;
	}

	// Use this for initialization
	void Start () 
	{
		if (GameController.IsSlowedDown)
			ApplySlow ();

		outterRotate = GetComponent<Rotate> ();
		innerRotate = transform.FindChild ("Sprite").GetComponent<Rotate> ();

		myAnimator = transform.FindChild ("Sprite").GetComponent<Animator> ();
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
		
		myAnimator.speed *= SlowDown.SlowAmount;
	}
	
	private void RemoveSlow()
	{
		innerRotate.rotVel *= 1 / SlowDown.SlowAmount;
		outterRotate.rotVel *= 1 / SlowDown.SlowAmount;

		myAnimator.speed *= 1 / SlowDown.SlowAmount;
	}
}
