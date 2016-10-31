using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Prime31;


#if UNITY_IOS || UNITY_TVOS

namespace Prime31
{
	public class iCloudBinding
	{
		#region Key/Value Store
		
		[DllImport("__Internal")]
		private static extern bool _iCloudSynchronize();
	
		// Synchronizes the values to disk. This gets called automatically at launch and at shutdown so you should rarely ever need to call it.
		public static bool synchronize()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudSynchronize();
	
			return false;
		}
		
		
		[DllImport("__Internal")]
		private static extern string _iCloudUbiquityIdentityToken();
	
		// iOS 6+ only! Gets either ubiquityIdentityToken if it exists or returns null if it doesn't or the device is on an older version of iOS
		public static string getUbiquityIdentityToken()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudUbiquityIdentityToken();
	
			return string.Empty;
		}
		
		
		[DllImport("__Internal")]
		private static extern void _iCloudRemoveObjectForKey( string key );
	
		// Removes the object from iCloud storage
		public static void removeObjectForKey( string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudRemoveObjectForKey( aKey );
		}
	
	
		[DllImport("__Internal")]
		private static extern bool _iCloudHasKey( string key );
	
		// Checks to see if the given key exists
		public static bool hasKey( string key )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudHasKey( key );
	
			return false;
		}
	
	
		[DllImport("__Internal")]
		private static extern string _iCloudStringForKey( string key );
	
		// Gets the string value for the key
		public static string stringForKey( string key )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudStringForKey( key );
	
			return string.Empty;
		}
	
	
		[DllImport("__Internal")]
		private static extern string _iCloudAllKeys();
	
		// Gets all the keys currently stored in iCloud
		public static List<object> allKeys()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudAllKeys().listFromJson();
	
			return new List<object>();
		}
	
	
		[DllImport("__Internal")]
		private static extern void _iCloudRemoveAll();
	
		// Removes all data stored in the key/value store
		public static void removeAll()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudRemoveAll();
		}
	
	
		[DllImport("__Internal")]
		private static extern void _iCloudSetString( string aString, string aKey );
	
		// Sets the string value for the key
		public static void setString( string aString, string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudSetString( aString, aKey );
		}
	
	
		[DllImport("__Internal")]
		private static extern string _iCloudDictionaryForKey( string aKey );
	
		// Gets the dictionary value for the key
		public static IDictionary dictionaryForKey( string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudDictionaryForKey( aKey ).dictionaryFromJson();
	
			return new Hashtable();
		}
	
	
		[DllImport("__Internal")]
		private static extern void _iCloudSetDictionary( string dict, string aKey );
	
		// Sets the dictionary for the key
		public static void setDictionary( string dict, string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudSetDictionary( dict, aKey );
		}
	
	
		[DllImport("__Internal")]
		private static extern float _iCloudDoubleForKey( string aKey );
	
		// Gets the float value for the key
		public static float doubleForKey( string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudDoubleForKey( aKey );
	
			return 0;
		}
	
	
		[DllImport("__Internal")]
		private static extern void _iCloudSetDouble( double value, string aKey );
	
		// Sets the float value for the key
		public static void setDouble( double value, string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudSetDouble( value, aKey );
		}
		
		
		[DllImport("__Internal")]
		private static extern int _iCloudIntForKey( string aKey );
	
		// Gets the int value for the key
		public static int intForKey( string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudIntForKey( aKey );
	
			return 0;
		}
	
	
		[DllImport("__Internal")]
		private static extern void _iCloudSetInt( int value, string aKey );
	
		// Sets the int value for the key
		public static void setInt( int value, string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudSetInt( value, aKey );
		}
	
	
		[DllImport("__Internal")]
		private static extern bool _iCloudBoolForKey( string aKey );
	
		// Gets the bool value for the key
		public static bool boolForKey( string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudBoolForKey( aKey );
	
			return false;
		}
	
	
		[DllImport("__Internal")]
		private static extern void _iCloudSetBool( bool value, string aKey );
	
		// Sets the string value for the key
		public static void setBool( bool value, string aKey )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudSetBool( value, aKey );
		}
		
		#endregion
		
		
		#region Document Store
		
		[DllImport("__Internal")]
		private static extern bool _iCloudDocumentStoreAvailable();
	
		// Checks to see if the iCloud document store is available on the current device
		public static bool documentStoreAvailable()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudDocumentStoreAvailable();
	
			return false;
		}
		
		
		[DllImport("__Internal")]
		private static extern string _iCloudDocumentsDirectory();
	
		// Gets the iCloud document store directory
		public static string documentsDirectory()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudDocumentsDirectory();
	
			return string.Empty;
		}
		
		
		[DllImport("__Internal")]
		private static extern string _iCloudDocumentsDirectoryForContainerId( string containerId );
	
		// Gets the iCloud document store directory for a specific container identifier
		public static string documentsDirectoryForContainerId( string containerId )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudDocumentsDirectoryForContainerId( containerId );
	
			return string.Empty;
		}
		
		
		[DllImport("__Internal")]
		private static extern bool _iCloudIsFileInCloud( string file );
	
		// Checks to see if a particular file is stored in iCloud
		public static bool isFileInCloud( string file )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudIsFileInCloud( file );
	
			return false;
		}
		
		
		[DllImport("__Internal")]
		private static extern bool _iCloudIsFileDownloaded( string file );
	
		// Check to see if a file is downloaded from iCloud. If it is not it will trigger a download.
		public static bool isFileDownloaded( string file )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudIsFileDownloaded( file );
	
			return false;
		}
		
		
		[DllImport("__Internal")]
		private static extern bool _iCloudAddFile( string file );
	
		// Adds a file to iCloud document storage
		public static bool addFile( string file )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				return _iCloudAddFile( file );
	
			return false;
		}
		
		
		[DllImport("__Internal")]
		private static extern void _iCloudEvictFile( string file );
	
		// Evicts the local file but does not remove it from iCloud
		public static void evictFile( string file )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer || (int)Application.platform == 31 )
				_iCloudEvictFile( file );
		}
	
		#endregion
	
	}

}
#endif
