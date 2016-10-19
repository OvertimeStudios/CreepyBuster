using UnityEngine;
using System.Collections;

public class Plasmette : MonoBehaviour 
{
	public float vel;
	public float spinningVel;
	public float timeSpinning;
	public float finalScale;
	public Transform plasmetteTransform;
	public Transform rangeTransform;
	public SpriteRenderer rangeSprite;

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
		rangeTransform.gameObject.SetActive(false);
		myAudioSource = GetComponent<AudioSource>();
		myTransform = transform;
		myAnimator = GetComponentInChildren<Animator>();
		myCollider = GetComponent<Collider2D>();

		rangeTween = rangeSprite.GetComponent<TweenScale>();
		originalRangeAlpha = rangeSprite.color.a;

		originalScale = plasmetteTransform.localScale;
		originalPosition = plasmetteTransform.position;

		MenuController.OnPanelClosed += Reactivate;

		GameController.OnGameStart += ChangeColor;
		LevelDesign.OnPlayerLevelUp += ChangeColor;
		GameController.OnLoseStacks += ChangeColor;
	}

	void OnFingerHover(FingerHoverEvent e)
	{
		if(GameController.isGameRunning || MenuController.activeMenu != MenuController.Menus.Main || Popup.IsActive || DailyRewardController.IsActive) return;

		if(e.Phase == FingerHoverPhase.Enter)
			spinningCoroutine = StartCoroutine(StartSpinning());

		//if(e.Phase == FingerHoverPhase.Exit)
			//StopSpinning();
	}

	void Update()
	{
		//position plasmette under finger
		if(GameController.isGameRunning)
		{
			Vector3 pos;
			if(Input.touches.Length > 0)
			{
				pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, 5));
			}
			else
			{
				pos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 5));
			}

			rangeTransform.position = new Vector3(pos.x, pos.y, pos.z);
		}

		//animate alpha on range
		if(Mathf.Abs(rangeTween.value.x) > Mathf.Abs(rangeTween.to.x - rangeTween.from.x) * 0.8f)
		{
			float total = Mathf.Abs(rangeTween.to.x - rangeTween.from.x) - (Mathf.Abs(rangeTween.to.x - rangeTween.from.x) * 0.8f);
			float part = Mathf.Abs(rangeTween.value.x) - (Mathf.Abs(rangeTween.to.x - rangeTween.from.x) * 0.8f);
			float percent = 1 - (part / total);

			Color c = rangeSprite.color;
			c.a = originalRangeAlpha * percent;
			rangeSprite.color = c;
		}
		else
		{
			Color c = rangeSprite.color;
			c.a = originalRangeAlpha;
			rangeSprite.color = c;
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
		//myAnimator.SetInteger("State", 4);

		plasmetteTransform.gameObject.SetActive(false);
		rangeTransform.gameObject.SetActive(true);

		rangeTransform.localScale = new Vector3(LevelDesign.CurrentRange, LevelDesign.CurrentRange, LevelDesign.CurrentRange);
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
		rangeTransform.gameObject.SetActive(false);
		myCollider.enabled = true;
		plasmetteTransform.gameObject.SetActive(true);

		StartCoroutine(BackToNormalScale());
	}

	private void ChangeColor()
	{
		Color c = LevelDesign.CurrentColor;
		c.a = 0.6f;
		rangeSprite.color = c;
	}
}
