using UnityEngine;
using System.Collections;

[System.Serializable]
public class RandomBetweenTwoConst
{
	public float min;
	public float max;

	public float Random()
	{
		return UnityEngine.Random.Range(min, max);
	}
}
