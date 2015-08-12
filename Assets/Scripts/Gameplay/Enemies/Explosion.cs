using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	ParticleSystem particleSystem;

	// Use this for initialization
	void Start () 
	{
		particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(particleSystem.particleCount == 0)
			Destroy(gameObject);
	}
}
