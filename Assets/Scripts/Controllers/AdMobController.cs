using UnityEngine;
using System.Collections;

public class AdMobController : MonoBehaviour 
{
	#if ADMOB_IMPLEMENTED
	void OnEnable()
	{
		if(!Global.IsAdFree)
			MenuController.OnPanelOpened += AdMobHelper.ShowBanner;
	}

	void OnDisable()
	{
		MenuController.OnPanelOpened -= AdMobHelper.ShowBanner;
	}
	#endif
}
