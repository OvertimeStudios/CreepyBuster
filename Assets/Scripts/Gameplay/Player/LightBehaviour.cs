using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightBehaviour : MonoBehaviour 
{
	private ParticleRenderer[] particleRenderers;

	//public Color[] colors;

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
		MenuController.OnPanelClosed += Reset;
	}

	void OnDisable()
	{
		MenuController.OnPanelClosed -= Reset;
	}

	// Use this for initialization
	void Start () 
	{
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
