using UnityEngine;
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

	void OnEnable()
	{
		UpdatePrices();
	}

	void Start()
	{
		IAPController.OnProductRestored += Restore;

		price = transform.FindChild("Price").FindChild("Label").GetComponent<UILabel>();
	}

	private void UpdatePrices()
	{
		IAPProduct product = IAPController.GetProduct(productID);

		if(product != null)
			price.text = product.currencyCode + product.price;
	}

	public void Purchase()
	{
		if(type == Type.Consumable)
			IAPController.PurchaseConsumable(productID, PurchaseComplete);
		else
			IAPController.PurchaseNonConsumable(productID, PurchaseComplete);

		Popup.ShowBlank("Processing");

		#if UNITY_EDITOR
		Popup.ShowBlank("Purchase not possible on Unity Editor", 2f);
		#endif
	}

	private void PurchaseComplete(bool success, string error)
	{
		if(success)
			Unlock();
		else
			Popup.ShowBlank("Error: " + error, 2f);
	}

	private void Restore(string pID)
	{
		if(productID == pID)
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
	}
}
