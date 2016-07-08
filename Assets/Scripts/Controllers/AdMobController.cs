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
		AdsHelper.ShowBannerAd(AdsHelper.AdSize.Banner, AdsHelper.AdPosition.Bottom);
	}
	#endif
}
