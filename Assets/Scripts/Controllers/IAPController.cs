using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class IAPController : MonoBehaviour 
{
	#region event
	public static event Action<IAPProduct> OnProductReceived;
	public static event Action<string> OnProductRestored;
	#endregion

	public string androidKey;
	public string[] productIDs;

	private List<IAPProduct> productsReceived;
	private List<string> productsRestored;

	private static IAPController instance;
	public static IAPController Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType<IAPController>();

				if(instance == null)
				{
					GameObject go = new GameObject();
					go.name = "IAPController";
					instance = go.AddComponent<IAPController>();
				}
			}

			return instance;
		}
	}

	void Awake()
	{
		instance = this;

		productsReceived = new List<IAPProduct>();
		productsRestored = new List<string>();

		IAP.init(androidKey);

		IAP.requestProductData(productIDs, productIDs, ProductDataReceived);
	}

	private void ProductDataReceived(List<IAPProduct> products)
	{
		foreach(IAPProduct product in products)
		{
			Debug.Log("Received " + product.title + " - " + product.description);
			
			productsReceived.Add(product);

			if(OnProductReceived != null)
				OnProductReceived(product);
		}
	}

	public static void PurchaseConsumable(string productID, Action<bool,string> completionHandler)
	{
		IAP.purchaseConsumableProduct(productID, completionHandler);
	}

	public static void PurchaseNonConsumable(string productID, Action<bool,string> completionHandler)
	{
		IAP.purchaseNonconsumableProduct(productID, completionHandler);
	}

	public static void RestorePurchases()
	{
		#if UNITY_IOS
		IAP.restoreCompletedTransactions(Instance.CompleteRestorePurchases);
		#elif UNITY_ANDROID
		GoogleIAB.queryInventory(Instance.productIDs);
		GoogleIABManager.queryInventorySucceededEvent += Instance.AndroidRestoreCompleted;
		GoogleIABManager.queryInventoryFailedEvent += Instance.AndroidRestoreFailed;

		Popup.ShowBlank("Processing");
		#endif
	}

	private void AndroidRestoreCompleted(List<GooglePurchase> purchases, List<GoogleSkuInfo> info)
	{
		foreach(GooglePurchase purchase in purchases)
			CompleteRestorePurchases(purchase.productId);

		Popup.ShowBlank("All itens retored", 2f);

		GoogleIABManager.queryInventorySucceededEvent -= AndroidRestoreCompleted;
		GoogleIABManager.queryInventoryFailedEvent -= AndroidRestoreFailed;
	}

	private void AndroidRestoreFailed(string errmsg)
	{
		Popup.ShowBlank("Restoration failed", 2f);

		GoogleIABManager.queryInventorySucceededEvent -= AndroidRestoreCompleted;
		GoogleIABManager.queryInventoryFailedEvent -= AndroidRestoreFailed;
	}

	public void CompleteRestorePurchases(string product)
	{
		Debug.Log("Restored " + product);

		productsRestored.Add(product);

		if(OnProductRestored != null)
			OnProductRestored(product);
	}

	public static bool IsProductRestored(string productID)
	{
		return Instance.productsRestored.Contains(productID);
	}

	public static IAPProduct GetProduct(string productID)
	{
		if(Instance.productsReceived == null)
			return null;

		foreach(IAPProduct product in Instance.productsReceived)
		{
			if(product.productId == productID)
				return product;
		}

		return null;
	}
}
