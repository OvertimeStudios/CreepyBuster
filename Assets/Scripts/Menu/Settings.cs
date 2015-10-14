using UnityEngine;
using System;
using System.Collections;
using Prime31;

public class Settings : MonoBehaviour 
{
	#region Actions
	public static event Action<IAPProduct> OnProductRestored;
	#endregion

	private GameObject logout;
	//private UILabel greeting;

	void Awake()
	{
		//logout = transform.FindChild ("Logout").gameObject;
		//greeting = transform.FindChild ("FB - Login").GetComponent<UILabel>();
	}

	void OnEnable()
	{
		HandleLoginSection ();

		Global.OnLoggedIn += HandleLoginSection;
		Global.OnLoggedOut += HandleLoginSection;
	}

	void OnDisable()
	{
		Global.OnLoggedIn -= HandleLoginSection;
		Global.OnLoggedOut -= HandleLoginSection;

		ChangeLanguage changeLanguage = transform.FindChild("Language").GetComponent<ChangeLanguage>();
		if(changeLanguage.opened)
			changeLanguage.OpenClose();
	}

	private void HandleLoginSection()
	{
		if(Global.IsLoggedIn)
		{
			//greeting.text = "Hello" + ", " + Global.user.firstname;
			//greeting.gameObject.GetComponent<Collider2D>().enabled = false;
			//greeting.gameObject.GetComponent<UIButton>().enabled = false;
			//logout.SetActive(true);
		}
		else
		{
			//greeting.text = "Login";
			//greeting.gameObject.GetComponent<Collider2D>().enabled = true;
			//greeting.gameObject.GetComponent<UIButton>().enabled = true;
			//logout.SetActive(false);
		}
	}

	public void RestorePurchases()
	{
		IAPHelper.RestoreCompletedTransactions(Callback);
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

			foreach(IAPProduct product in IAPHelper.ProductsRestored)
			{
				if(OnProductRestored != null)
					OnProductRestored(product);
			}
		}
	}
}
