using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadingLegion : MonoBehaviour 
{
	private const string spriteBlack = "minion_transparente";
	private const string spriteNormal = "minion_branco";

	private List<UISprite> sprites;

	public float timeToChange = 0.4f;
	private int currentMinion;

	// Use this for initialization
	void Start () 
	{
		sprites = new List<UISprite>();

		foreach(Transform t in transform)
			sprites.Add(t.GetComponent<UISprite>());

		foreach(UISprite sprite in sprites)
			sprite.spriteName = spriteBlack;

		sprites[0].spriteName = spriteNormal;

		currentMinion = 0;

		StartCoroutine(BlinkNext());
	}

	private IEnumerator BlinkNext()
	{
		yield return new WaitForSeconds(timeToChange);

		sprites[currentMinion].spriteName = spriteBlack;

		currentMinion++;

		if(currentMinion >= sprites.Count)
			currentMinion = 0;

		sprites[currentMinion].spriteName = spriteNormal;

		StartCoroutine(BlinkNext());
	}
}
