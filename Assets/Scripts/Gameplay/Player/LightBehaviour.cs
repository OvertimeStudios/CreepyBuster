using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightBehaviour : MonoBehaviour 
{
	enum Side
	{
		Right,
		Left,
	}

	private ParticleRenderer[] particleRenderers;
	private SpriteRenderer outter;
	private TrailRenderer trailRenderer;
	private Transform myTransform;

	public Transform plasmette;

	[Header("Left/Right Achievement")]
	public Achievement achievement;
	private Side side;
	private float timeOnSide;
	public static float maxTimeOnSide;

	#region singleton
	private static LightBehaviour instance;
	public static LightBehaviour Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<LightBehaviour>();
			
			return instance;
		}
	}
	#endregion

	void OnEnable()
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint ((Vector3)FingerDetector.FingerPosition);
		pos.z = 0;
		transform.position = pos;

		MenuController.OnPanelClosed += Reset;
		FingerDetector.OnFingerMotionEvent += OnFingerMove;

		AttackTargets.OnSpecialStarted += UpdateColor;
		AttackTargets.OnSpecialEnded += UpdateColor;
		LevelDesign.OnPlayerLevelUp += UpdateColor;
		GameController.OnLoseStacks += UpdateColor;
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
		FingerDetector.OnFingerMotionEvent -= OnFingerMove;

		AttackTargets.OnSpecialStarted -= UpdateColor;
		AttackTargets.OnSpecialEnded -= UpdateColor;
		LevelDesign.OnPlayerLevelUp -= UpdateColor;
		GameController.OnLoseStacks -= UpdateColor;
	}

	// Use this for initialization
	void Start () 
	{
		myTransform = transform;
		trailRenderer = transform.FindChild("Trail Renderer").GetComponent<TrailRenderer>();
		outter = transform.FindChild("Outter").GetComponent<SpriteRenderer>();
		particleRenderers = GetComponentsInChildren<ParticleRenderer> ();

		UpdateColor ();

		side = Side.Left;
	}

	private void UpdateColor()
	{
		foreach (ParticleRenderer pr in particleRenderers)
			pr.material.SetColor ("_TintColor", LevelDesign.CurrentColor);

		Color c = LevelDesign.CurrentColor;
		c.a = 0.2f;
		trailRenderer.material.SetColor("_TintColor", c);

		outter.color = LevelDesign.CurrentColor;
	}

	void FixedUpdate()
	{
		//movement
		Vector3 pos;
		if (Input.touches.Length > 0)//mobile
			pos = (Vector3)Input.GetTouch (0).position;
		else
			pos = Input.mousePosition;
		
		float posz = myTransform.position.z;
		pos = Camera.main.ScreenToWorldPoint(pos);
		pos.z = posz;
		myTransform.position = pos;
	}

	void Update()
	{
		//side left - right
		Vector3 pos = Camera.main.WorldToViewportPoint(myTransform.position);
		Side newSide;
		if(pos.x < 0.5f)
			newSide = Side.Left;
		else
			newSide = Side.Right;

		if(side != newSide)
		{
			side = newSide;
			timeOnSide = 0;
		}

		timeOnSide += Time.deltaTime;

		if(timeOnSide > maxTimeOnSide)
			maxTimeOnSide = timeOnSide;

		if(!achievement.unlocked && maxTimeOnSide >= achievement.value)
			achievement.Unlock();

		if(side == Side.Left)
			GameController.leftTime += Time.deltaTime;
		else
			GameController.rightTime += Time.deltaTime;
	}

	private void Reset()
	{
		timeOnSide = 0f;

		UpdateColor ();
	}
	
	void OnFingerMove(FingerMotionEvent e)
	{
		/*Vector3 pos = Camera.main.ScreenToWorldPoint ((Vector3)e.Position);
		pos.z = 0;
		transform.position = pos;*/
	}
}
