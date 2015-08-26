using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	ParticleSystem myParticleSystem;

	// Use this for initialization
	void Start () 
	{
		myParticleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if(myParticleSystem.particleCount == 0)
			Destroy(gameObject);
	}
}
