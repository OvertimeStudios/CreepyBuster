﻿using UnityEngine;
using System.Collections;
using Prime31;

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

	private UILabel price;
	private UILabel currency;

	void OnEnable()
	{
		UpdatePrices();
	}

	void Start()
	{
		currency = transform.FindChild("Price").FindChild("currency").GetComponent<UILabel>();
		price = transform.FindChild("Price").FindChild("Label").GetComponent<UILabel>();
	}

	private void UpdatePrices()
	{
		IAPProduct product = IAPHelper.GetProduct(productID);

		if(product != null)
		{
			string priceString = product.price.ToString();

			currency.text = priceString.Substring(0, priceString.IndexOf("$") + 1);
			price.text = priceString.Substring(priceString.IndexOf("$") + 1);
		}
	}

	public void Purchase()
	{
		Debug.Log("Trying to purchase: " + productID);
		if(type == Type.Consumable)
			IAPHelper.PurchaseConsumableProduct(productID, Callback);
		else
			IAPHelper.PurchaseNonconsumableProduct(productID, Callback);

		#if UNITY_EDITOR
		Popup.ShowBlank("Purchase not possible on Unity Editor", 2f);
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

	private void Restore(IAPProduct product)
	{
		if(type == Type.Consumable) return;

		if(productID == product.productId)
			Unlock();
	}

	private void Unlock()
	{
		switch(pack)
		{
			case Pack.OrbsPack:
				Global.TotalOrbs += value;
				Popup.ShowOk (Localization.Get("YOU_RECEIVED") + " " + value + " orbs.");
				break;
				
			case Pack.MultiplierOrbs:
				Global.OrbsMultiplier = value;
				Popup.ShowOk("Orbs gain doubled");
				break;
		}

		Global.IsAdFree = true;
		AdMobHelper.HideBanner();
	}
}
