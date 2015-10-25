using UnityEngine;
using System;
using System.Collections;
#if IAP_IMPLEMENTED
using Prime31;
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

	void Awake()
	{
		//logout = transform.FindChild ("Logout").gameObject;
		//greeting = transform.FindChild ("FB - Login").GetComponent<UILabel>();
	}

	void OnEnable()
	{
		HandleLoginSection ();

		FacebookController.OnLoggedIn += HandleLoginSection;
		FacebookController.OnLoggedOut += HandleLoginSection;
	}

	void OnDisable()
	{
		FacebookController.OnLoggedIn -= HandleLoginSection;
		FacebookController.OnLoggedOut -= HandleLoginSection;

		ChangeLanguage changeLanguage = transform.FindChild("Language").GetComponent<ChangeLanguage>();
		if(changeLanguage.opened)
			changeLanguage.OpenClose();
	}

	private void HandleLoginSection()
	{
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
			Popup.ShowBlank("Processing");
		else if(state == IAPState.Failed)
			Popup.ShowBlank("Restoration failed: " + errmsg, 2f);
		else if(state == IAPState.Cancelled)
			Popup.ShowBlank("Restoration cancelled", 2f);
		else if(state == IAPState.Success)
		{
			Popup.ShowBlank("Products successfully restored", 2f);

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
	