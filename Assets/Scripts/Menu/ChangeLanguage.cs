using UnityEngine;
using System.Collections;

public class ChangeLanguage : MonoBehaviour 
{
	public UIButton portuguese;
	public UIButton english;
	public TweenPosition tween;

	[HideInInspector]
	public bool opened = false;

	private Vector3 closedPosition;
	private Vector3 openPosition;

	void Start()
	{
		openPosition = tween.to;
		closedPosition = tween.from;
	}

	public void Change()
	{
		if (UIButton.current == portuguese)
			LocalizationController.CurrentLanguage = LocalizationController.Language.Portuguese;
		else if (UIButton.current == english)
			LocalizationController.CurrentLanguage = LocalizationController.Language.English;
	}

	public void OpenClose()
	{
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
