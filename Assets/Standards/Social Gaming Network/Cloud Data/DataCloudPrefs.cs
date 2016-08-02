using UnityEngine;
using System.Collections;

#if CLOUDDATA_IMPLEMENTED
using Prime31;
#endif

public class DataCloudPrefs
{
	public static bool HasKey(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.hasKey(key);
			#elif UNITY_ANDROID

			#endif

		#endif

		return PlayerPrefs.HasKey(key);
	}

	public static void SetInt(string key, int value)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.setInt(key, value);
			#elif UNITY_ANDROID

			#endif

		#endif

		PlayerPrefs.SetInt(key, value);
		PlayerPrefs.Save();
	}

	public static void SetFloat(string key, float value)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.setFloat(key, value);
			#elif UNITY_ANDROID

			#endif

		#endif

		PlayerPrefs.SetFloat(key, value);
		PlayerPrefs.Save();
	}

	public static void SetString(string key, string value)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.setString(key, value);
			#elif UNITY_ANDROID

			#endif

		#endif

		PlayerPrefs.SetString(key, value);
		PlayerPrefs.Save();
	}


	public static int GetInt(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getInt(key);
			#elif UNITY_ANDROID
			
			#endif

		#endif

		return PlayerPrefs.GetInt(key);
	}

	public static float GetFloat(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getFloat(key);
			#elif UNITY_ANDROID

			#endif

		#endif

		return PlayerPrefs.GetFloat(key);
	}

	public static string GetString(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getString(key);
			#elif UNITY_ANDROID

			#endif

		#endif

		return PlayerPrefs.GetString(key);
	}

	public static void DeleteKey(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.removeObjectForKey(key);
			#elif UNITY_ANDROID

			#endif

		#endif

		PlayerPrefs.DeleteKey(key);
		PlayerPrefs.Save();
	}

	public static void DeleteAll()
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.removeAll();
			#elif UNITY_ANDROID

			#endif

		#endif

		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
