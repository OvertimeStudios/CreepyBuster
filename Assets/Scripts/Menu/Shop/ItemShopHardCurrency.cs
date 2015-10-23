﻿using UnityEngine;
using System.Collections;
#if IAP_IMPLEMENTED
using Prime31;
#endif

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

	void OnEnable()
	{
		UpdatePrices();

		#if IAP_IMPLEMENTED
		Settings.OnProductRestored += Restore;
		#endif
	}

	void OnDisable()
	{
		#if IAP_IMPLEMENTED
		Settings.OnProductRestored -= Restore;
		#endif
	}

	void Start()
	{
		#if IAP_IMPLEMENTED
		//currency = transform.FindChild("Price").FindChild("currency").GetComponent<UILabel>();
		//price = transform.FindChild("Price").FindChild("Label").GetComponent<UILabel>();
		#endif
	}

	private void UpdatePrices()
	{
		#if IAP_IMPLEMENTED
		IAPProduct product = IAPHelper.GetProduct(productID);

		if(product != null)
		{
			string priceString = product.price.ToString();

			currency.text = priceString.Substring(0, priceString.IndexOf("$") + 1);
			price.text = priceString.Substring(priceString.IndexOf("$") + 1);
		}
		#endif
	}

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
			Popup.ShowBlank("Processing");
		else if(state == IAPState.Failed)
			Popup.ShowBlank("Purchase failed: \n" + errmsg, 2f);
		else if(state == IAPState.Cancelled)
			Popup.ShowBlank("Purchase cancelled", 2f);
		else if(state == IAPState.Success)
			PurchaseComplete();
	}

	private void PurchaseComplete()
	{
		Unlock();
	}

	#if IAP_IMPLEMENTED
	private void Restore(IAPProduct product)
	{
		if(type == Type.Consumable) return;

		if(productID == product.productId)
			Unlock();
	}
	#endif

	private void Unlock()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.ShopBuy);

		switch(pack)
		{
			case Pack.OrbsPack:
				Global.OrbsCollected += value;
				Global.TotalOrbs += value;
				Popup.ShowOk (string.Format(Localization.Get("YOU_RECEIVED"), value));
				break;
				
			case Pack.MultiplierOrbs:
				Global.OrbsMultiplier = value;
				Global.IsAdFree = true;
				#if ADMOB_IMPLEMENTED
				AdMobHelper.HideBanner();
				#endif
				Popup.ShowOk("Orbs gain doubled");
				break;
		}
	}
}
