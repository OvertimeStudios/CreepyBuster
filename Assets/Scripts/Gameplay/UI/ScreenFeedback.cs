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

	private UI2DSprite frozen;
	private UI2DSprite damage;
	private UI2DSprite invencibility;
	private UISprite blank;
	private UI2DSprite shield;

	private static AudioSource myAudioSource;

	private Coroutine frozenCoroutine;
	private Coroutine damageCoroutine;
	private Coroutine invencibilityCoroutine;
	private Coroutine blankCoroutine;

	void OnDisable()
	{
		StopAllCoroutines ();
	}

	// Use this for initialization
	void Start () 
	{
		instance = this;

		frozen = transform.FindChild ("Frozen").GetComponent<UI2DSprite> ();
		damage = transform.FindChild ("Damage").GetComponent<UI2DSprite> ();
		invencibility = transform.FindChild ("Invencibility").GetComponent<UI2DSprite> ();
		blank = transform.FindChild ("Blank").GetComponent<UISprite> ();
		shield = transform.FindChild("Shield").GetComponent<UI2DSprite>();

		myAudioSource = GetComponent<AudioSource>();

		frozen.enabled = false;
		damage.enabled = false;
		invencibility.enabled = false;
		blank.enabled = false;
		shield.enabled = false;

		GameController.OnReset += Reset;
	}

	public static void ShowDamage(float time)
	{
		Instance.damage.enabled = true;
		Instance.damage.alpha = 1;

		#if !UNITY_WEBGL
		if(Global.CanVibrate)
			Handheld.Vibrate ();
		#endif

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
		if(Global.IsSoundOn)
		{
			myAudioSource.Stop();
			myAudioSource.Play();
		}
		GameController.OnPause += PauseSound;

		Instance.invencibility.enabled = true;

		if (Instance.invencibilityCoroutine != null)
			Instance.StopCoroutine (Instance.invencibilityCoroutine);

		Instance.invencibilityCoroutine = Instance.StartCoroutine (Blink (Instance.invencibility, time, time * 0.75f, 3));
	}

	public static void ShowShield()
	{
		Instance.shield.enabled = true;
	}

	public static void HideShield()
	{
		Instance.shield.enabled = false;
	}

	private static void PauseSound()
	{
		GameController.OnPause -= PauseSound;
		GameController.OnResume += ResumeSound;

		if(Global.IsSoundOn)
			myAudioSource.Pause();
	}

	private static void ResumeSound()
	{
		GameController.OnPause += PauseSound;
		GameController.OnResume -= ResumeSound;

		if(Global.IsSoundOn)
			myAudioSource.UnPause();
	}

	public static void ShowBlank(float fadeInTime, float fadeOutTime)
	{
		if(Instance.blankCoroutine != null)
			Instance.StopCoroutine(Instance.blankCoroutine);

		Instance.blankCoroutine = Instance.StartCoroutine(FadeInOut(Instance.blank, fadeInTime, fadeOutTime));
	}

	private static IEnumerator FadeOut(UIBasicSprite sprite, float time)
	{
		while(sprite.alpha > 0)
		{
			sprite.alpha -= Time.deltaTime / time;

			yield return null;
		}

		sprite.enabled = false;
		sprite.alpha = 1;
	}

	private static IEnumerator Blink(UIBasicSprite sprite, float time, float timeToStartBlink, int blinkRepetition)
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

	private static IEnumerator FadeInOut(UIBasicSprite sprite, float fadeInTime, float fadeOutTime)
	{
		sprite.enabled = true;
		sprite.alpha = 0;

		#region fade in
		while(sprite.alpha < 1)
		{
			sprite.alpha += Time.deltaTime / fadeInTime;
			
			yield return null;
		}

		sprite.alpha = 1;
		#endregion

		#region fade out
		while(sprite.alpha > 0)
		{
			sprite.alpha -= Time.deltaTime / fadeInTime;
			
			yield return null;
		}
		
		sprite.alpha = 0;
		sprite.enabled = false;
		#endregion
	}

	private void Reset()
	{
		frozen.enabled = false;
		damage.enabled = false;
		invencibility.enabled = false;

		frozen.alpha = 0;
		damage.alpha = 0;
	}
}
