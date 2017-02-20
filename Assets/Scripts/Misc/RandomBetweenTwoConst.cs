using UnityEngine;
using System.Collections;

[System.Serializable]
public class RandomBetweenTwoConst
{
	public float min;
	public float max;

	public RandomBetweenTwoConst()
	{
		min = max = 0;
	}

	public RandomBetweenTwoConst(float _min, float _max)
	{
		min = _min;
		max = _max;
	}

	public float Random()
	{
		return UnityEngine.Random.Range((float)min, (float)max);
	}
}
