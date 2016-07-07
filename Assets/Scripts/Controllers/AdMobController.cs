using UnityEngine;
using System.Collections;

public class AdMobController : MonoBehaviour 
{
	#if ADMOB_IMPLEMENTED
	void OnEnable()
	{
		if(!Global.IsAdFree)
			MenuController.OnPanelOpened += AdsHelper.ShowBannerAd;
	}

	void OnDisable()
	{
		MenuController.OnPanelOpened -= AdsHelper.ShowBannerAd;
	}

	void OnDestroy()
	{
		MenuController.OnPanelOpened -= AdsHelper.ShowBannerAd;
	}
	#endif
}
