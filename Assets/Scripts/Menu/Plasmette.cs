using UnityEngine;
using System.Collections;

public class Plasmette : MonoBehaviour 
{
	public float vel;
	public float spinningVel;
	public float timeSpinning;
	public float finalScale;
	public Transform plasmetteTransform;

	private Collider2D myCollider;
	private Animator myAnimator;
	private Transform myTransform;
	private static Coroutine spinningCoroutine;
	private AudioSource myAudioSource;
	
	private Vector3 waypoint;
	private float angle;
	private Vector3 originalScale;
	private Vector3 originalPosition;

	private TweenScale rangeTween;
	private float originalRangeAlpha;

	public static bool IsSpinning
	{
		get { return spinningCoroutine != null; }
	}

	// Use this for initialization
	void Start () 
	{
		myAudioSource = GetComponent<AudioSource>();
		myTransform = transform;
		myAnimator = GetComponentInChildren<Animator>();
		myCollider = GetComponent<Collider2D>();

		originalScale = plasmetteTransform.localScale;
		originalPosition = plasmetteTransform.position;

		MenuController.OnPanelClosed += Reactivate;
	}

	void OnFingerHover(FingerHoverEvent e)
	{
		if(!(MenuController.IsMenuActive && MenuController.activeMenu == MenuController.Menus.Main) || Popup.IsActive || DailyRewardController.IsActive || DailyMissionController.Instance.IsPopupActive) return;

		if(e.Phase == FingerHoverPhase.Enter && e.Selection == gameObject)
		{
			spinningCoroutine = StartCoroutine(StartSpinning());

			GameController.gameMode = GameController.GameMode.Endless;
		}
	}
	
	private IEnumerator StartSpinning()
	{
		if(Global.IsSoundOn)
			myAudioSource.Play();

		myAnimator.SetInteger("State", 3);
		myCollider.enabled = false;

		float time = 0;
		float rotVel = 0;
		float scale = originalScale.x;
		
		while(time < timeSpinning)
		{
			time += Time.deltaTime;
			
			rotVel = (time / timeSpinning) * spinningVel;
			myTransform.Rotate(0, 0, rotVel);
			
			scale = originalScale.x - ((time / timeSpinning) * Mathf.Abs(finalScale - originalScale.x));
			plasmetteTransform.localScale = new Vector3(scale, scale, scale);
			
			yield return null;
		}
		
		spinningCoroutine = null;
		MenuController.Instance.OpenPanel();

		plasmetteTransform.gameObject.SetActive(false);
	}
	
	private void StopSpinning()
	{
		if(Global.IsSoundOn)
			myAudioSource.Stop();
		
		if(spinningCoroutine != null)
		{
			StopCoroutine(spinningCoroutine);
			spinningCoroutine = null;
			
			myAnimator.SetInteger("State", 4);

			StartCoroutine(BackToNormalScale());
		}
	}
	
	private IEnumerator BackToNormalScale()
	{
		myTransform.rotation = Quaternion.identity;

		while(plasmetteTransform.localScale.x < originalScale.x)
		{
			plasmetteTransform.localScale += Vector3.one * Time.deltaTime;
			yield return null;
		}
			
		plasmetteTransform.localScale = originalScale;
	}

	private void Reactivate()
	{
		myCollider.enabled = true;
		plasmetteTransform.gameObject.SetActive(true);

		StartCoroutine(BackToNormalScale());
	}
}
