using UnityEngine;
using System.Collections;
using Prime31;
using System;

public class ItemShopHardCurrency : MonoBehaviour 
{
	public enum Type
	{
		Consumable,
		NonConsumable,
	}

	public enum Pack
	{
		OrbsPack,
		MultiplierOrbs,
	}

	public string productID;
	public Type type;
	public Pack pack;
	public int value;
	
	[Header("For Developer")]
	public UILabel price;
	public UILabel currency;

	private IAPProduct product;

	void Start()
	{
		#if IAP_IMPLEMENTED
		//currency = transform.FindChild("Price").FindChild("currency").GetComponent<UILabel>();
		//price = transform.FindChild("Price").FindChild("Label").GetComponent<UILabel>();
		#endif

		#if IAP_IMPLEMENTED
		Settings.OnProductRestored += Restore;
		IAPHelper.OnProductReceived += UpdatePrices;
		#endif

		#if UNITY_WEBPLAYER
		gameObject.SetActive(false);
		#endif
	}

	#if UNITY_ANDROID || UNITY_IOS
	private void UpdatePrices(IAPProduct product)
	{
		Debug.Log("Product Received: " + product.ToString());
		#if IAP_IMPLEMENTED
		if(product.productId == productID)
		{
			this.product = product;

			string priceString = product.price.ToString();

			currency.text = CurrencyTools.GetCurrencySymbol(product.currencyCode);
			price.text = priceString.Substring(priceString.IndexOf("$") + 1);
		}
		#endif
	}
	#endif

	public void Purchase()
	{
		#if IAP_IMPLEMENTED
		Debug.Log("*************Trying to purchase: " + productID);
		if(type == Type.Consumable)
			IAPHelper.PurchaseConsumableProduct(productID, Callback);
		else
			IAPHelper.PurchaseNonconsumableProduct(productID, Callback);
		#endif

		#if UNITY_EDITOR
		//Popup.ShowBlank("Purchase not possible on Unity Editor", 2f);
		#endif
	}

	private void Callback(IAPState state, string errmsg)
	{
		if(state == IAPState.Processing)
			Popup.ShowBlank(Localization.Get("PROCESSING"));
		else if(state == IAPState.Failed)
			Popup.ShowBlank(Localization.Get("PURCHASE_FAILED"), 2f);
		else if(state == IAPState.Cancelled)
			Popup.ShowBlank(Localization.Get("PURCHASE_FAILED"), 2f);
		else if(state == IAPState.Success)
			PurchaseComplete();
	}

	private void PurchaseComplete()
	{
		#if UNITY_EDITOR
		Debug.Log("Running on Unity Editor");
		#else
		if(!Debug.isDebugBuild)
			UnityAnalyticsHelper.Transaction(product.productId, decimal.Parse(price.text), product.currencyCode);
		#endif

		Unlock();
	}

	#if IAP_IMPLEMENTED
	private void Restore(IAPProduct product)
	{
		if(type == Type.Consumable) return;

		if(productID == product.productId)
		{
			Debug.Log("**** RESTORING PRODUCT: " + product.productId);
			Unlock(false);
		}
	}
	#endif

	private void Unlock()
	{
		Unlock(true);
	}

	private void Unlock(bool showSuccessMessage)
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.ShopBuy);

		switch(pack)
		{
			case Pack.OrbsPack:
				Global.OrbsCollected += value;
				Global.TotalOrbs += value;
				if(showSuccessMessage)
					Popup.ShowOk (string.Format(Localization.Get("YOU_RECEIVED"), value));
				break;
				
			case Pack.MultiplierOrbs:
				Global.OrbsMultiplier = value;
				Global.IsAdFree = true;
				#if ADMOB_IMPLEMENTED
				AdsHelper.HideBannerAd();
				#endif
				if(showSuccessMessage)
					Popup.ShowOk(Localization.Get("PURCHASE_SUCCESS"));
				break;
		}
	}
}
