using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
	public float rotVel;

	[HideInInspector]
	public float originalVel;

	private EnemyLife enemyLife;

	void Start()
	{
		originalVel = rotVel;

		enemyLife = GetComponent<EnemyLife>();

		if(enemyLife == null)
			enemyLife = transform.parent.GetComponent<EnemyLife>();
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
			rotVel -= Time.deltaTime * enemyLife.deathTime * maxRotVel;
			
			yield return null;
		}
	}
}
