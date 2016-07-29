using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;



#pragma warning disable 0162, 0649

namespace Prime31
{
	public class P31Prefs
	{
		public static bool iCloudDocumentStoreAvailable { get { return _iCloudDocumentStoreAvailable; } }
		private static bool _iCloudDocumentStoreAvailable;


#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
		static P31Prefs()
		{
			_iCloudDocumentStoreAvailable = iCloudBinding.documentStoreAvailable();
		}
#endif


		public static bool synchronize()
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.synchronize();
#endif
			PlayerPrefs.Save();
			return true;
		}


		public static bool hasKey( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.hasKey( key );
#else
			return PlayerPrefs.HasKey( key );
#endif
		}


		public static List<object> allKeys()
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.allKeys();
#else
			return new List<object>();
#endif
		}


		public static void removeObjectForKey( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.removeObjectForKey( key );
#endif
			PlayerPrefs.DeleteKey( key );
		}


		public static void removeAll()
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.removeAll();
#endif
			PlayerPrefs.DeleteAll();
		}


		public static void setInt( string key, int val )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.setInt( val, key );
#endif
			PlayerPrefs.SetInt( key, val );
		}


		public static int getInt( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.intForKey( key );
#else
			return PlayerPrefs.GetInt( key );
#endif
		}


		public static void setFloat( string key, float val )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.setDouble( val, key );
#endif
			PlayerPrefs.SetFloat( key, val );
		}


		public static float getFloat( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.doubleForKey( key );
#else
			return PlayerPrefs.GetFloat( key );
#endif
		}


		public static void setString( string key, string val )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.setString( val, key );
#endif
			PlayerPrefs.SetString( key, val );
		}


		public static string getString( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.stringForKey( key );
#else
			return PlayerPrefs.GetString( key );
#endif
		}


		public static void setBool( string key, bool val )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.setBool( val, key );
#endif
			PlayerPrefs.SetInt( key, val ? 1 : 0 );
		}


		public static bool getBool( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.boolForKey( key );
#else
			return PlayerPrefs.GetInt( key, 0 ) == 1;
#endif
		}


		public static void setDictionary( string key, Dictionary<string,object> val )
		{
			var json = Prime31.Json.encode( val );

#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			iCloudBinding.setDictionary( json, key );
#endif
			PlayerPrefs.SetString( key, json );
		}


		public static IDictionary getDictionary( string key )
		{
#if ( UNITY_IOS || UNITY_TVOS ) && !UNITY_EDITOR
			return iCloudBinding.dictionaryForKey( key );
#else
			var str = PlayerPrefs.GetString( key );
			return str.dictionaryFromJson();
#endif
		}

	}

}
#pragma warning restore 0162, 0649
