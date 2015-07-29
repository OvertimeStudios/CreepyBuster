using UnityEngine;
using System.Collections;

public class FadeAlpha : MonoBehaviour 
{
	public float time = 1.0f;
	public bool destroyOnFinish = true;

	// Use this for initialization
	void Start () 
	{
		StartCoroutine (FadeOut ());
	}

	private IEnumerator FadeOut()
	{
		UIWidget uiWidget = GetComponent<UIWidget> ();

		while(time > 0)
		{
			uiWidget.alpha -= Time.deltaTime / time;

			yield return null;
		}

		if (destroyOnFinish)
			Destroy (gameObject);
	}
}
