using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace Prime31
{
	public class iCloudEventListener : MonoBehaviour
	{
#if UNITY_IOS
		void OnEnable()
		{
			// Listen to all events for illustration purposes
			iCloudManager.keyValueStoreDidChangeEvent += keyValueStoreDidChangeEvent;
			iCloudManager.ubiquityIdentityDidChangeEvent += ubiquityIdentityDidChangeEvent;
			iCloudManager.entitlementsMissingEvent += entitlementsMissingEvent;
			iCloudManager.documentStoreUpdatedEvent += documentStoreUpdatedEvent;
		}
	
	
		void OnDisable()
		{
			// Remove all event handlers
			iCloudManager.keyValueStoreDidChangeEvent -= keyValueStoreDidChangeEvent;
			iCloudManager.ubiquityIdentityDidChangeEvent -= ubiquityIdentityDidChangeEvent;
			iCloudManager.entitlementsMissingEvent -= entitlementsMissingEvent;
			iCloudManager.documentStoreUpdatedEvent -= documentStoreUpdatedEvent;
		}
	
	
	
		void keyValueStoreDidChangeEvent( List<object> keys )
		{
			Debug.Log( "keyValueStoreDidChangeEvent.  changed keys: " );
			foreach( var key in keys )
				Debug.Log( key );
		}
		
		
		void ubiquityIdentityDidChangeEvent()
		{
			Debug.Log( "ubiquityIdentityDidChangeEvent" );
		}
		
		
		void entitlementsMissingEvent()
		{
			Debug.Log( "entitlementsMissingEvent" );
		}
		
		
		void documentStoreUpdatedEvent( List<iCloudManager.iCloudDocument> files )
		{
			foreach( var doc in files )
				Debug.Log( doc );
		}
#endif
	}

}
	
	
