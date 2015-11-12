using UnityEngine;
using System;
using System.Collections;
#if IAP_IMPLEMENTED
using Prime31;
#endif

#if FACEBOOK_IMPLEMENTED
using Facebook.Unity;
#endif

public class Settings : MonoBehaviour 
{
	#if IAP_IMPLEMENTED
	#region Actions
	public static event Action<IAPProduct> OnProductRestored;
	#endregion
	#endif

	public GameObject fbLogin;
	public GameObject fbLogout;
	public GameObject likeUs;

	[Header("Web")]
	public GameObject restorePurchases;

	void Awake()
	{
		//logout = transform.FindChild ("Logout").gameObject;
		//greeting = transform.FindChild ("FB - Login").GetComponent<UILabel>();
	}

	void Start()
	{
		#if UNITY_WEBPLAYER
		fbLogin.SetActive(false);
		fbLogout.SetActive(false);
		likeUs.SetActive(false);
		restorePurchases.SetActive(false);
		#endif
	}

	void OnEnable()
	{
		HandleLoginSection ();

		#if FACEBOOK_IMPLEMENTED
		FacebookController.OnLoggedIn += HandleLoginSection;
		FacebookController.OnLoggedOut += HandleLoginSection;
		#endif
	}

	void OnDisable()
	{
		#if FACEBOOK_IMPLEMENTED
		FacebookController.OnLoggedIn -= HandleLoginSection;
		FacebookController.OnLoggedOut -= HandleLoginSection;
		#endif

		ChangeLanguage changeLanguage = transform.FindChild("Language").GetComponent<ChangeLanguage>();
		if(changeLanguage.opened)
			changeLanguage.OpenClose();
	}

	private void HandleLoginSection()
	{
		#if FACEBOOK_IMPLEMENTED
		if(FB.IsLoggedIn)
		{
			fbLogin.SetActive(false);
			fbLogout.SetActive(true);
			likeUs.SetActive(true);
		}
		else
		{
			fbLogin.SetActive(true);
			fbLogout.SetActive(false);
			likeUs.SetActive(false);
		}
		#endif
	}

	public void RestorePurchases()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		#if IAP_IMPLEMENTED
		IAPHelper.RestoreCompletedTransactions(Callback);
		#endif
	}

	private void Callback(IAPState state, string errmsg)
	{
		if(state == IAPState.Processing)
			Popup.ShowBlank(Localization.Get("PROCESSING"));
		else if(state == IAPState.Failed || state == IAPState.Cancelled)
			Popup.ShowBlank(Localization.Get("RESTORE_FAILED"),2f);
		else if(state == IAPState.Success)
		{
			Popup.ShowBlank(Localization.Get("RESTORE_SUCCESS"), 2f);

			#if IAP_IMPLEMENTED
			foreach(IAPProduct product in IAPHelper.ProductsRestored)
			{
				if(OnProductRestored != null)
					OnProductRestored(product);
			}
			#endif
		}
	}
}
	