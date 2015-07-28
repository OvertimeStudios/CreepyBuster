using UnityEngine;
using System.Collections;

public class ChangeLanguage : MonoBehaviour 
{
	public Transform brazilButton;
	public Transform euaButton;
	public TweenPosition selection;

	void Start()
	{
		if (LocalizationController.CurrentLanguage == LocalizationController.Language.English)
			selection.transform.position = euaButton.position;
		else if (LocalizationController.CurrentLanguage == LocalizationController.Language.Portuguese)
			selection.transform.position = brazilButton.position;
	}

	public void Change()
	{
		if (UIButton.current == brazilButton.GetComponent<UIButton>())
			LocalizationController.CurrentLanguage = LocalizationController.Language.Portuguese;
		else if (UIButton.current == euaButton.GetComponent<UIButton>())
			LocalizationController.CurrentLanguage = LocalizationController.Language.English;

		Vector3 from = selection.transform.localPosition;
		
		selection.ResetToBeginning ();

		selection.from = from;
		selection.to = UIButton.current.transform.localPosition;
		
		selection.PlayForward ();
	}
}
