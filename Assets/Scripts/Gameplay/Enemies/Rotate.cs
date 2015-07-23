using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
	public float rotVel;

	[HideInInspector]
	public float originalVel;

	void Start()
	{
		originalVel = rotVel;
	}

	// Update is called once per frame
	void Update () 
	{
		transform.Rotate (Vector3.forward * rotVel);
	}

	public void Stop()
	{
		rotVel = 0;
	}

	public void StopSmooth()
	{
		StartCoroutine (StopSpinning ());
	}

	private IEnumerator StopSpinning()
	{
		float maxRotVel = rotVel;
		
		while(rotVel > 0)
		{
			rotVel -= Time.deltaTime * EnemyLife.deathTime * maxRotVel;
			
			yield return null;
		}
	}
}
