﻿using UnityEngine;
using System.Collections;

public class Plasmette : MonoBehaviour 
{
	public float vel;
	public float spinningVel;
	public float timeSpinning;
	public float finalScale;
	
	private Animator myAnimator;
	private Transform myTransform;
	private Vector3 initialPosition;
	private Coroutine spinningCoroutine;
	private AudioSource myAudioSource;
	
	private Vector3 waypoint;
	private float angle;
	private Vector3 originalScale;
	
	void OnEnable()
	{
		//FingerDetector.OnFingerDownEvent += OnFingerDown;
		//FingerDetector.OnFingerUpEvent += OnFingerUp;
	}
	
	void OnDisable()
	{
		//FingerDetector.OnFingerDownEvent -= OnFingerDown;
		//FingerDetector.OnFingerUpEvent -= OnFingerUp;
	}
	
	// Use this for initialization
	void Start () 
	{
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerUpEvent += OnFingerUp;

		myAudioSource = GetComponent<AudioSource>();
		myTransform = transform;
		myAnimator = GetComponent<Animator>();
		
		initialPosition = transform.position;
		originalScale = myTransform.localScale;
	}
	
	void Update()
	{
		if(GameController.isGameRunning)
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(FingerDetector.FingerPosition);
			pos.z = initialPosition.z;
			myTransform.position = pos;
		}
	}
	
	private void OnFingerDown(FingerDownEvent e)
	{
		Vector3 click = Camera.main.ScreenToWorldPoint(e.Position);
		click.z = initialPosition.z;
		
		float distance = Vector3.Distance(click, myTransform.position);
		if(distance < 1f && waypoint == Vector3.zero)
		{
			StopAllCoroutines();
			spinningCoroutine = StartCoroutine(StartSpinning());
		}
		else
		{
			waypoint = click;
			
			StopAllCoroutines();
			StartCoroutine (FollowWaypoint());
		}
	}
	
	private void OnFingerUp(FingerUpEvent e)
	{
		StartCoroutine(StopSpinning());
	}
	
	private IEnumerator FollowWaypoint()
	{
		myAnimator.SetInteger("State", 1);
		
		while(Vector3.Distance(myTransform.position, waypoint) > 0.3f)
		{
			angle = Mathf.Atan2(waypoint.y - myTransform.position.y, waypoint.x - myTransform.position.x) * Mathf.Rad2Deg;
			
			Vector3 eulerAngle = transform.eulerAngles;
			eulerAngle.z = Mathf.LerpAngle (eulerAngle.z, angle, 0.1f * vel);
			myTransform.eulerAngles = eulerAngle;
			
			myTransform.position += transform.right * vel * Time.deltaTime;
			
			yield return null;
		}
		
		myAnimator.SetInteger("State", 2);

		yield return new WaitForEndOfFrame();

		if(FingerDetector.IsFingerDown)
		{
			Vector3 fingerPosition = Camera.main.ScreenToWorldPoint(FingerDetector.FingerPosition);
			fingerPosition.z = myTransform.position.z;
			float distance = Vector3.Distance(fingerPosition, myTransform.position);
			if(distance < 1f)
			{
				StopAllCoroutines();
				spinningCoroutine = StartCoroutine(StartSpinning());;
			}
		}

		if(waypoint != initialPosition)
		{
			waypoint = initialPosition;
			yield return new WaitForSeconds(1f);
			StartCoroutine(FollowWaypoint());
		}
		else
			waypoint = Vector3.zero;
	}
	
	private IEnumerator StartSpinning()
	{
		if(Global.IsSoundOn)
			myAudioSource.Play();
		
		myAnimator.SetInteger("State", 3);
		
		float time = 0;
		float rotVel = 0;
		float scale = originalScale.x;
		
		while(time < timeSpinning)
		{
			time += Time.deltaTime;
			
			rotVel = (time / timeSpinning) * spinningVel;
			myTransform.Rotate(0, 0, rotVel);
			
			Vector3 pos = Camera.main.ScreenToWorldPoint(FingerDetector.FingerPosition);
			pos.z = initialPosition.z;
			myTransform.position = pos;
			
			scale = originalScale.x - ((time / timeSpinning) * Mathf.Abs(finalScale - originalScale.x));
			myTransform.localScale = new Vector3(scale, scale, scale);
			
			yield return null;
		}
		
		spinningCoroutine = null;
		MenuController.Instance.OpenPanel();
		myAnimator.SetInteger("State", 4);

		FingerDetector.OnFingerDownEvent -= OnFingerDown;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;
		MenuController.OnPanelClosed += BackToCenter;
		
		gameObject.SetActive(false);
	}
	
	private IEnumerator StopSpinning()
	{
		if(Global.IsSoundOn)
			myAudioSource.Stop();
		
		if(spinningCoroutine != null)
		{
			StopCoroutine(spinningCoroutine);
			spinningCoroutine = null;
			
			myAnimator.SetInteger("State", 4);
			yield return new WaitForSeconds(0.5f);
			
			StopAllCoroutines();
			BackToCenter();
		}
		
		yield return null;
	}
	
	private void BackToCenter()
	{
		gameObject.SetActive(true);
		
		waypoint = initialPosition;
		StartCoroutine(FollowWaypoint());
		StartCoroutine(BackToNormalScale());
		
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerUpEvent += OnFingerUp;
		MenuController.OnPanelClosed -= BackToCenter;
	}
	
	private IEnumerator BackToNormalScale()
	{
		while(Mathf.Abs(myTransform.localScale.x - originalScale.x) < 0.05f)
		{
			myTransform.localScale += Vector3.one * Time.deltaTime;
			yield return null;
		}
		
		myTransform.localScale = originalScale;
	}
}
