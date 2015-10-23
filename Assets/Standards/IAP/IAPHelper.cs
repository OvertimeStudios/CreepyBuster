using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if IAP_IMPLEMENTED
using Prime31;
#endif

public enum IAPState
{
	Processing,
	Success,
	Failed,
	Cancelled,
}

public class IAPHelper : MonoBehaviour 
{
	private const string CONSUMABLE_PAYLOAD = "consume";
	private const string NON_CONSUMABLE_PAYLOAD = "nonconsume";

	public string androidPublicKey;
	public string[] productsID;

	#if IAP_IMPLEMENTED
	private static bool isRestoring;

	private static List<IAPProduct> productsReceived;
	private static List<IAPProduct> productsRestored;

	#region Action
	private static event Action<IAPState, string> _callback;
	#endregion

	#region get / set
	public static List<IAPProduct> ProductsData
	{
		get 
		{
			if(productsReceived == null)
			{
				if(!(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
					Debug.LogWarning("productsReceived not populated. Please call IAPHelper.RequestProductData() first");

				return null;
			}

			return productsReceived; 
		}
	}

	public static List<IAPProduct> ProductsRestored
	{
		get 
		{
			if(productsRestored == null)
			{
				if(!(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
					Debug.LogWarning("productsRestored not populated. Please call IAPHelper.RestoreCompletedTransactions() first");

				return null;
			}
			
			return productsRestored; 
		}
	}
	#endregion

	#region get / set (HELPERS)
	public static IAPProduct GetProduct(string productID)
	{
		if(ProductsData == null)
			return null;
		
		foreach(IAPProduct product in ProductsData)
		{
			if(product.productId == productID)
				return product;
		}
		
		return null;
	}

	public static bool IsProductRestored(IAPProduct product)
	{
		if(ProductsRestored == null)
			return false;

		return ProductsRestored.Contains(product);
	}

	public static bool IsProductRestored(string productID)
	{
		if(ProductsRestored == null)
			return false;
		
		foreach(IAPProduct product in ProductsRestored)
		{
			if(product.productId == productID)
				return true;
		}
		
		return false;
	}
	#endregion

	#region Singleton implementation
	private static IAPHelper instance = null;
	private static IAPHelper Instance 
	{
		get 
		{
			if (instance == null) 
				instance = GameObject.FindObjectOfType<IAPHelper>();

			return instance;
		}
	}
	#endregion

	void Awake()
	{
		#if UNITY_ANDROID
		GoogleIABManager.billingSupportedEvent += BillingSupported;
		GoogleIABManager.billingNotSupportedEvent += BillingNotSupported;
		GoogleIABManager.purchaseSucceededEvent += PurchaseSuccessful;
		GoogleIABManager.purchaseFailedEvent += PurchaseFailed;
		GoogleIABManager.queryInventorySucceededEvent += QueryInventorySucceededEvent;
		GoogleIABManager.queryInventoryFailedEvent += QueryInventoryFailedEvent;
		#elif UNITY_IPHONE && IAP_INTEGRATED
		StoreKitManager.purchaseSuccessfulEvent += PurchaseSuccessful;
		StoreKitManager.purchaseCancelledEvent += PurchaseCancelled;
		StoreKitManager.purchaseFailedEvent += PurchaseFailed;
		StoreKitManager.productListReceivedEvent += productListReceivedEvent;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
		StoreKitManager.restoreTransactionsFinishedEvent += restoreTransactionsFinishedEvent;
		StoreKitManager.restoreTransactionsFailedEvent += restoreTransactionsFailedEvent;
		#endif

		if(!string.IsNullOrEmpty(androidPublicKey))
			Init();
	}

	// Initializes the billing system. Call this at app launch to prepare the IAP system.
	public static void Init()
	{
		#if UNITY_ANDROID
		if(Debug.isDebugBuild)
			GoogleIAB.enableLogging(true);

		Debug.Log("**************GoogleIAB.init*********");

		GoogleIAB.init( Instance.androidPublicKey );
		#endif
	}

	public static void Init( string androidPublicKey, string[] productsID)
	{
		Instance.productsID = productsID;
		Instance.androidPublicKey = androidPublicKey;

		#if UNITY_ANDROID
		if(Debug.isDebugBuild)
			GoogleIAB.enableLogging(true);

		GoogleIAB.init( androidPublicKey );
		#endif
	}
	
	// After callback == IAPState.Success the IAPHelper.ProductsData will be populated.
	public static void RequestProductData(Action<IAPState,string> callback)
	{
		if(Debug.isDebugBuild && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
		{
			callback(IAPState.Failed, "Not supported on Unity. Please run into build");
			return;
		}

		_callback = callback;

		if(productsReceived != null)
		{
			callback(IAPState.Success, "");
			return;
		}

		isRestoring = false;

		if(_callback != null)
			_callback(IAPState.Processing, "");

		#if UNITY_ANDROID
		GoogleIAB.queryInventory( Instance.productsID );
		#elif UNITY_IOS
		StoreKitBinding.requestProductData( iosProductIdentifiers );
		#elif UNITY_EDITOR

		#endif
	}

	// Purchases the given product and quantity. completionHandler provides if the purchase succeeded (bool) and an error message which will be populated if
	// the purchase failed.
	public static void PurchaseConsumableProduct( string productId, Action<IAPState,string> callback )
	{
		if(Debug.isDebugBuild && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
		{
			callback(IAPState.Success, "");
			return;
		}

		_callback = callback;

		if(_callback != null)
			_callback(IAPState.Processing, "");

		#if UNITY_ANDROID
		Debug.Log(string.Format("******GoogleIAB.purchaseProduct({0}, {1})****", productId, CONSUMABLE_PAYLOAD));
		GoogleIAB.purchaseProduct( productId, CONSUMABLE_PAYLOAD );
		#elif UNITY_IOS
		StoreKitBinding.purchaseProduct( productId, 1 );
		#endif
	}
	
	
	// Purchases the given product and quantity. completionHandler provides if the purchase succeeded (bool) and an error message which will be populated if
	// the purchase failed.
	public static void PurchaseNonconsumableProduct( string productId, Action<IAPState,string> callback )
	{
		if(Debug.isDebugBuild && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
		{
			callback(IAPState.Success, "");
			return;
		}

		_callback = callback;

		if(_callback != null)
			_callback(IAPState.Processing, "");

		#if UNITY_ANDROID
		GoogleIAB.purchaseProduct( productId, NON_CONSUMABLE_PAYLOAD );
		#elif UNITY_IOS
		StoreKitBinding.purchaseProduct( productId, 1 );
		#endif
	}
	
	
	// Restores all previous transactions. This is used when a user gets a new device and they need to restore their old purchases.
	// DO NOT call this on every launch. It will prompt the user for their password.
	// After callback == IAPState.Success the IAPHelper.ProductsRestored will be populated.
	public static void RestoreCompletedTransactions(Action<IAPState,string> callback)
	{
		if(Debug.isDebugBuild && (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor))
		{
			callback(IAPState.Failed, "Not supported on Unity. Please run into build");
			return;
		}

		_callback = callback;

		isRestoring = true;

		#if UNITY_IOS
		StoreKitBinding.restoreCompletedTransactions();
		#elif UNITY_ANDROID
		GoogleIAB.queryInventory(Instance.productsID);
		#endif
	}

#if UNITY_ANDROID
	public static void ConsumeProduct(string productID)
	{
		Debug.Log("Consuming item " + productID);
		GoogleIAB.consumeProduct( productID );
	}

	private void BillingSupported() 
	{
		Debug.Log("************Billing is Supported************");		

		Debug.Log("************IAPHelper.RequestProductData(null)************");

		//HACK: This saves some time
		IAPHelper.RequestProductData(null);
	}
	
	private void BillingNotSupported(string error) 
	{
		Debug.Log("Billing NOT Supported: " + error);
	}

	private void PurchaseSuccessful(GooglePurchase purchase)
	{
		Debug.Log("****PURCHASE SUCCESSFUL");
		if(purchase.developerPayload == CONSUMABLE_PAYLOAD)
			ConsumeProduct(purchase.productId);

		if(_callback != null)
			_callback(IAPState.Success, "");
	}

	private void PurchaseFailed(string errmsg, int response)
	{
		Debug.Log("****PURCHASE FAILED");
		if(_callback != null)
			_callback(IAPState.Failed, errmsg);
	}

	private void QueryInventorySucceededEvent(List<GooglePurchase> purchases, List<GoogleSkuInfo> skus)
	{
		List<IAPProduct> products = new List<IAPProduct>();

		if(isRestoring)
		{
			foreach(GooglePurchase purchase in purchases)
			{
				if(purchase.purchaseState == GooglePurchase.GooglePurchaseState.Purchased)
				{
					foreach(GoogleSkuInfo info in skus)
					{
						if(info.productId == purchase.productId)
						{
							products.Add(new IAPProduct(info));
							break;
						}
					}
				}
			}

			productsRestored = products;
		}
		else
		{
			foreach(GoogleSkuInfo info in skus)
				products.Add(new IAPProduct(info));

			productsReceived = products;
		}

		if(_callback != null)
			_callback(IAPState.Success, "");

		Debug.Log("************QueryInventorySucceededEvent************");
		Debug.Log("************Products received: " + skus.Count);
		foreach(GoogleSkuInfo info in skus)
			Debug.Log(info.productId);

		//Prime31.Utils.logObject( skus );
	}

	private void QueryInventoryFailedEvent(string errmsg)
	{
		if(_callback != null)
			_callback(IAPState.Failed, errmsg);

		Debug.Log("************QueryInventoryFailedEvent************ " + errmsg);
	}
#endif
#endif
}

