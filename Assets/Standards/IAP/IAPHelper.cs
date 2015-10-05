﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;

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
	public bool autoRequestProducts = true;

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
	}

	// Initializes the billing system. Call this at app launch to prepare the IAP system.
	public static void Init()
	{
		#if UNITY_ANDROID
		GoogleIAB.init( Instance.androidPublicKey );
		#endif

		if(Instance.autoRequestProducts)
			RequestProductData(null);
	}

	public static void Init( string androidPublicKey, string[] productsID)
	{
		Instance.productsID = productsID;
		Instance.androidPublicKey = androidPublicKey;

		#if UNITY_ANDROID
		GoogleIAB.init( androidPublicKey );
		#endif

		if(Instance.autoRequestProducts)
			RequestProductData(null);
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
	private void PurchaseSuccessful(GooglePurchase purchase)
	{
		if(_callback != null)
			_callback(IAPState.Success, "");
	}

	private void PurchaseFailed(string errmsg, int response)
	{
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
	}

	private void QueryInventoryFailedEvent(string errmsg)
	{
		if(_callback != null)
			_callback(IAPState.Failed, errmsg);
	}
#endif
}

