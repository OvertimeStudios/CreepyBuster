using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour 
{
	public float duration = 1.0f;
	public float magnitude = 2.5f;

	public void Shake()
	{
		StartCoroutine (DoShake (duration, magnitude));
	}

	public void Shake(float d)
	{
		StartCoroutine (DoShake (d, magnitude));
	}

	public void Shake(float d, float m)
	{
		StartCoroutine (DoShake (d, m));
	}

	IEnumerator DoShake(float duration, float magnitude) 
	{
		float elapsed = 0.0f;
		
		Vector3 originalPos = transform.localPosition;

		float endShakeTime = Time.realtimeSinceStartup + duration;

		while (Time.realtimeSinceStartup < endShakeTime) 
		{
			elapsed += Time.deltaTime;          
			
			float percentComplete = elapsed / duration;         
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
			
			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;
			
			transform.localPosition = originalPos + new Vector3(x, y, 0f);
			
			yield return null;
		}
		
		transform.localPosition = originalPos;
	}
}
