using UnityEngine;
using System.Collections;

public class FadePosition : MonoBehaviour 
{
	public float time = 1.0f;
	public Vector3 vel = new Vector3(0, -1.0f, 0);
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
			uiWidget.transform.position -= Time.deltaTime * vel;
			
			yield return null;
		}

		if (destroyOnFinish)
			Destroy (gameObject);
	}
}
