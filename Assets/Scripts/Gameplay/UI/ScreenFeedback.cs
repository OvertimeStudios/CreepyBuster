using UnityEngine;
using System.Collections;

public class ScreenFeedback : MonoBehaviour 
{
	public CameraShake cameraShake;

	#region singleton
	private static ScreenFeedback instance;
	public static ScreenFeedback Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<ScreenFeedback>();

			return instance;
		}
	}
	#endregion

	#region get /set
	public static bool IsDamageActive
	{
		get { return Instance.damage.enabled; }
	}
	#endregion

	private UISprite frozen;
	private UISprite damage;
	private UISprite invencibility;

	private Coroutine frozenCoroutine;
	private Coroutine damageCoroutine;
	private Coroutine invencibilityCoroutine;

	// Use this for initialization
	void Start () 
	{
		instance = this;

		frozen = transform.FindChild ("Frozen").GetComponent<UISprite> ();
		damage = transform.FindChild ("Damage").GetComponent<UISprite> ();
		invencibility = transform.FindChild ("Invencibility").GetComponent<UISprite> ();

		frozen.enabled = false;
		damage.enabled = false;
		invencibility.enabled = false;
	}

	public static void ShowDamage(float time)
	{
		Instance.damage.enabled = true;
		Instance.damage.alpha = 1;

		if(Instance.cameraShake != null)
			Instance.cameraShake.Shake ();

		if (Instance.damageCoroutine != null)
			Instance.StopCoroutine (Instance.damageCoroutine);
		
		Instance.damageCoroutine = Instance.StartCoroutine (FadeOut (Instance.damage, time));
	}

	public static void ShowFrozen(float time)
	{
		Instance.frozen.enabled = true;
		Instance.frozen.alpha = 1;

		if (Instance.frozenCoroutine != null)
			Instance.StopCoroutine (Instance.frozenCoroutine);

		Instance.frozenCoroutine = Instance.StartCoroutine (FadeOut (Instance.frozen, time));
	}

	public static void ShowInvencibility(float time)
	{
		Instance.invencibility.enabled = true;

		if (Instance.invencibilityCoroutine != null)
			Instance.StopCoroutine (Instance.invencibilityCoroutine);

		Instance.invencibilityCoroutine = Instance.StartCoroutine (Blink (Instance.invencibility, time, time * 0.75f, 3));
	}

	private static IEnumerator FadeOut(UISprite sprite, float time)
	{
		while(sprite.alpha > 0)
		{
			sprite.alpha -= Time.deltaTime / time;

			yield return null;
		}

		sprite.enabled = false;
		sprite.alpha = 1;
	}

	private static IEnumerator Blink(UISprite sprite, float time, float timeToStartBlink, int blinkRepetition)
	{
		yield return new WaitForSeconds(timeToStartBlink);

		float timeEachBlink = (time - timeToStartBlink) / (blinkRepetition * 2);

		for(byte i = 0; i < blinkRepetition; i++)
		{
			sprite.enabled = false;

			yield return new WaitForSeconds(timeEachBlink);

			sprite.enabled = true;

			yield return new WaitForSeconds(timeEachBlink);
		}

		sprite.enabled = false;
	}
}
