using UnityEngine;
using System.Collections;

public class AdMobController : MonoBehaviour 
{
	#if ADMOB_IMPLEMENTED
	void Start()
	{
		if(!Global.IsAdFree)
			ShowBannerAd();
	}

	private void ShowBannerAd()
	{
		AdMobHelper.ShowBanner(GoogleMobileAds.Api.AdSize.Banner, GoogleMobileAds.Api.AdPosition.Bottom);
	}
	#endif
}
