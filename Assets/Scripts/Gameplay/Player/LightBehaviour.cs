using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightBehaviour : MonoBehaviour 
{
	private ParticleRenderer[] particleRenderers;
	private SpriteRenderer outter;

	public Transform plasmette;

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
		outter = transform.FindChild("Outter").GetComponent<SpriteRenderer>();
		particleRenderers = GetComponentsInChildren<ParticleRenderer> ();

		UpdateColor ();
	}

	private void UpdateColor()
	{
		foreach (ParticleRenderer pr in particleRenderers)
			pr.material.SetColor ("_TintColor", LevelDesign.CurrentColor);

		outter.color = LevelDesign.CurrentColor;
	}

	private void Reset()
	{
		UpdateColor ();
	}
	
	void OnFingerMove(FingerMotionEvent e)
	{
		Vector3 pos = Camera.main.ScreenToWorldPoint ((Vector3)e.Position);
		pos.z = 0;
		transform.position = pos;
	}
}
