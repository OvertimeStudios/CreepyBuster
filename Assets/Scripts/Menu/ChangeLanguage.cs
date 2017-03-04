using UnityEngine;
using System.Collections;

public class ChangeLanguage : MonoBehaviour 
{
	public UIButton portuguese;
	public UIButton english;
	public UIButton italian;
	public UIButton russian;
	public TweenPosition tween;

	[HideInInspector]
	public bool opened;

	private Vector3 closedPosition;
	private Vector3 openPosition;

	void OnEnable()
	{
		if(opened)
		{
			tween.transform.localPosition = closedPosition;
			opened = false;
		}
	}

	void Start()
	{
		openPosition = tween.to;
		closedPosition = tween.from;
	}

	public void Change()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		if (UIButton.current == portuguese)
			LocalizationController.CurrentLanguage = LocalizationController.Language.Portuguese;
		else if (UIButton.current == english)
			LocalizationController.CurrentLanguage = LocalizationController.Language.English;
		else if(UIButton.current == italian)
			LocalizationController.CurrentLanguage = LocalizationController.Language.Italian;
		else if(UIButton.current == russian)
			LocalizationController.CurrentLanguage = LocalizationController.Language.Russian;
	}

	public void OpenClose()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		tween.ResetToBeginning();

		if(opened)
		{
			tween.from = openPosition;
			tween.to = closedPosition;
		}
		else
		{
			tween.from = closedPosition;
			tween.to = openPosition;
		}

		tween.PlayForward();

		opened = !opened;
	}
}
