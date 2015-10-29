using UnityEngine;
using System.Collections;

public class Plasmette : MonoBehaviour 
{
	public float vel;
	public float spinningVel;
	public float timeSpinning;
	public float finalScale;
	
	private Animator myAnimator;
	private Transform myTransform;
	private Coroutine spinningCoroutine;
	private AudioSource myAudioSource;
	
	private Vector3 waypoint;
	private float angle;
	private Vector3 originalScale;
	
	// Use this for initialization
	void Start () 
	{
		myAudioSource = GetComponent<AudioSource>();
		myTransform = transform;
		myAnimator = GetComponent<Animator>();

		originalScale = myTransform.localScale;

		MenuController.OnPanelClosed += Reactivate;
	}

	void OnFingerHover(FingerHoverEvent e)
	{
		if(MenuController.activeMenu != MenuController.Menus.Main) return;

		if(e.Phase == FingerHoverPhase.Enter)
			spinningCoroutine = StartCoroutine(StartSpinning());

		if(e.Phase == FingerHoverPhase.Exit)
			StopSpinning();
	}
	
	private IEnumerator StartSpinning()
	{
		if(Global.IsSoundOn)
			myAudioSource.Play();
		
		myAnimator.SetInteger("State", 3);
		
		float time = 0;
		float rotVel = 0;
		float scale = originalScale.x;
		
		while(time < timeSpinning)
		{
			time += Time.deltaTime;
			
			rotVel = (time / timeSpinning) * spinningVel;
			myTransform.Rotate(0, 0, rotVel);
			
			scale = originalScale.x - ((time / timeSpinning) * Mathf.Abs(finalScale - originalScale.x));
			myTransform.localScale = new Vector3(scale, scale, scale);
			
			yield return null;
		}
		
		spinningCoroutine = null;
		MenuController.Instance.OpenPanel();
		myAnimator.SetInteger("State", 4);
		
		gameObject.SetActive(false);
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
		while(Mathf.Abs(myTransform.localScale.x - originalScale.x) > 0.05f)
		{
			myTransform.localScale += Vector3.one * Time.deltaTime;
			yield return null;
		}
		
		myTransform.localScale = originalScale;
	}

	private void Reactivate()
	{
		gameObject.SetActive(true);

		StartCoroutine(BackToNormalScale());
	}
}
