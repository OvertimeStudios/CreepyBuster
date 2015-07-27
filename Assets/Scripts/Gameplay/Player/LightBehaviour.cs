﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightBehaviour : MonoBehaviour 
{
	private ParticleRenderer[] particleRenderers;

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
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
		FingerDetector.OnFingerMotionEvent -= OnFingerMove;
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;
		particleRenderers = GetComponentsInChildren<ParticleRenderer> ();

		UpdateParticleColor ();
	}

	private void UpdateParticleColor()
	{
		foreach (ParticleRenderer pr in particleRenderers)
			pr.material.SetColor ("_TintColor", LevelDesign.CurrentColor);
	}

	private void Reset()
	{
		UpdateParticleColor ();
	}
	
	void OnFingerMove(FingerMotionEvent e)
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint ((Vector3)e.Position);
		pos.z = 0;
		transform.position = pos;
	}
}
