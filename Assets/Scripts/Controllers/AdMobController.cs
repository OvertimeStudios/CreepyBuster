using UnityEngine;
using System.Collections;

public class AdMobController : MonoBehaviour 
{
	#if ADMOB_IMPLEMENTED
	void OnEnable()
	{
		MenuController.OnPanelOpened += AdMobHelper.ShowBanner;
	}

	void OnDisable()
	{
		MenuController.OnPanelOpened -= AdMobHelper.ShowBanner;
	}
	#endif
}
