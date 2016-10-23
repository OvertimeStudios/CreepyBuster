using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if CLOUDDATA_IMPLEMENTED
using Prime31;
#endif

public class DataCloudPrefs
{
	private static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
	const string TIMESTAMP = "_timestamp";

	#region Action
	#if UNITY_ANDROID
	public static Action OnFinishedLoading;
	#endif
	#endregion

	#if UNITY_ANDROID
	private static Dictionary<string, object> snapshotKeys = new Dictionary<string, object>();
	private static string _snapshotSaveName;
	#endif

	private static bool isLoaded = false;

	/// <summary>
	/// Android Only! Load all values into a Dictionary<string, object>
	/// </summary>
	/// <param name="keys">All Keys present in PlayerPrefs.GetX(key).</param>
	public static void Load(string snapshotSaveName)
	{
		if(isLoaded)
		{
			Debug.LogWarning("DataCloudPrefs already loaded. Load just once per run");
			return;
		}

		#if CLOUDDATA_IMPLEMENTED
			#if UNITY_ANDROID
			_snapshotSaveName = snapshotSaveName;
			snapshotKeys.Clear();

			if(PlayGameServices.isSignedIn())
				LoadSnapshots();
			else
			{
				GPGManager.authenticationSucceededEvent += AuthenticationSuccess;
				GPGManager.authenticationFailedEvent += AuthenticationFailed;
				
				PlayGameServices.authenticate();
			}
			#endif
		#endif
	}

	#if UNITY_ANDROID
	private static void AuthenticationSuccess(string msg)
	{
		Debug.Log("Authentication success: " + msg);
		LoadSnapshots();
	}

	private static void AuthenticationFailed(string msg)
	{
		Debug.Log("Authentication failed: " + msg);

		if(PlayerPrefs.HasKey(_snapshotSaveName))
			snapshotKeys = Json.decode<Dictionary<string,object>>(PlayerPrefs.GetString(_snapshotSaveName));

		isLoaded = true;
		if(OnFinishedLoading != null)
			OnFinishedLoading();
	}

	private static void LoadSnapshots()
	{
		GPGManager.loadSnapshotSucceededEvent += LoadSnapshotSuccess;
		GPGManager.loadSnapshotFailedEvent += LoadSnapshotFail;
		GPGManager.saveSnapshotSucceededEvent += SaveSnapshopSuccess;
		GPGManager.saveSnapshotFailedEvent += SaveSnapshopFail;

		PlayGameServices.loadSnapshot(_snapshotSaveName);
	}


	private static void LoadSnapshotSuccess(GPGSnapshot snapshot)
	{
		if(snapshot.hasDataAvailable)
		{
			if(PlayerPrefs.HasKey(TIMESTAMP))
			{
				Debug.Log(string.Format("Local TIMESTAMP: {0} / SNAPSHOT TIMESTAMP: {1}", (double)PlayerPrefs.GetFloat(TIMESTAMP), snapshot.metadata.lastModifiedTimestamp));
				if((double)PlayerPrefs.GetFloat(TIMESTAMP) > snapshot.metadata.lastModifiedTimestamp)
				{
					Debug.Log("Loaded from PlayerPrefs: " + PlayerPrefs.GetString("_metadata"));
					snapshotKeys = Json.decode<Dictionary<string,object>>(PlayerPrefs.GetString("_metadata"));
				}
				else
				{
					Debug.Log("Loaded from Snapshot: " + System.Text.Encoding.UTF8.GetString(snapshot.snapshotData));
					snapshotKeys = Json.decode<Dictionary<string,object>>(System.Text.Encoding.UTF8.GetString(snapshot.snapshotData));	
				}
			}
			else
			{
				Debug.Log("Loaded from Snapshot: " + System.Text.Encoding.UTF8.GetString(snapshot.snapshotData));
				snapshotKeys = Json.decode<Dictionary<string,object>>(System.Text.Encoding.UTF8.GetString(snapshot.snapshotData));
			}

			if(snapshotKeys == null)
				snapshotKeys = new Dictionary<string, object>();

			isLoaded = true;
			if(OnFinishedLoading != null)
				OnFinishedLoading();
		}

		Debug.Log("Load Snapshot Success. Snapshot count: " + snapshotKeys.Count);
	}

	private static void LoadSnapshotFail(string msg)
	{
		if(PlayerPrefs.HasKey(_snapshotSaveName))
			snapshotKeys = Json.decode<Dictionary<string,object>>(PlayerPrefs.GetString(_snapshotSaveName));

		isLoaded = true;
		if(OnFinishedLoading != null)
			OnFinishedLoading();

		Debug.Log("Load Snapshot Fail");
	}

	private static void SaveSnapshopSuccess()
	{
		Debug.Log("Save Snapshot Sucess");
	}

	private static void SaveSnapshopFail(string msg)
	{
		Debug.Log("Save Snapshot Failed: " + msg);
	}
	#endif

	public static bool IsLoaded
	{
		get
		{
			#if UNITY_IOS || UNITY_EDITOR
			return true;
			#elif UNITY_ANDROID
			return isLoaded;
			#endif
		}
	}

	public static bool HasKey(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.hasKey(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return snapshotKeys.ContainsKey(key);
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
			if(snapshotKeys.ContainsKey(key))
				snapshotKeys[key] = value;
			else
				snapshotKeys.Add(key, value);
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
			if(snapshotKeys.ContainsKey(key))
				snapshotKeys[key] = value;
			else
				snapshotKeys.Add(key, value);
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
			if(snapshotKeys.ContainsKey(key))
				snapshotKeys[key] = value;
			else
				snapshotKeys.Add(key, value);
			#endif

		#endif

		PlayerPrefs.SetString(key, value);
		PlayerPrefs.Save();
	}

	public static void SetBool(string key, bool value)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.setBool(key, value);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				snapshotKeys[key] = value ? 1 : 0;
			else
				snapshotKeys.Add(key, value ? 1 : 0);
			#endif

		#endif

		PlayerPrefs.SetInt(key, value ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static int GetInt(string key, int defaultValue)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getInt(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return int.Parse(snapshotKeys[key].ToString());
			#endif

		#endif

		return PlayerPrefs.GetInt(key, defaultValue);
	}

	public static int GetInt(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getInt(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return int.Parse(snapshotKeys[key].ToString());
			#endif

		#endif

		return PlayerPrefs.GetInt(key);
	}

	public static float GetFloat(string key, float defaultValue)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getFloat(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return float.Parse(snapshotKeys[key].ToString());
			#endif

		#endif

		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	public static float GetFloat(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getFloat(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return float.Parse(snapshotKeys[key].ToString());
			#endif

		#endif

		return PlayerPrefs.GetFloat(key);
	}

	public static string GetString(string key, string defaultValue)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getString(key);
			else
				return defaultValue;
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return snapshotKeys[key].ToString();
			else
				return defaultValue;
			#endif

		#endif

		return PlayerPrefs.GetString(key, defaultValue);
	}

	public static string GetString(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getString(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return snapshotKeys[key].ToString();
			#endif

		#endif
		
		return PlayerPrefs.GetString(key);
	}

	public static bool GetBool(string key)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getBool(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return int.Parse(snapshotKeys[key].ToString()) == 1;
			#endif

		#endif

		return PlayerPrefs.GetInt(key) == 1;
	}

	public static bool GetBool(string key, bool defaultValue)
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			if(P31Prefs.hasKey(key))
				return P31Prefs.getBool(key);
			#elif UNITY_ANDROID
			if(snapshotKeys.ContainsKey(key))
				return int.Parse(snapshotKeys[key].ToString()) == 1;
			#endif

		#endif

		return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
	}

	public static void DeleteKey(string key)
	{
		#if CLOUDDATA_IMPLEMENTED
			#if UNITY_IOS
			P31Prefs.removeObjectForKey(key);
			#elif UNITY_ANDROID
			snapshotKeys.Remove(key);
			#endif
		#endif

		PlayerPrefs.DeleteKey(key);
		PlayerPrefs.Save();
	}
		
	public static void Save()
	{
		
		#if CLOUDDATA_IMPLEMENTED
			#if UNITY_IOS
			P31Prefs.synchronize();
			#elif UNITY_ANDROID
			Debug.Log("***SAVING SNAPSHOT " + _snapshotSaveName + "(" + snapshotKeys.Count + ") " + snapshotKeys.toJson());
			PlayGameServices.saveSnapshot(_snapshotSaveName, true, System.Text.Encoding.UTF8.GetBytes(snapshotKeys.toJson()), "");

			PlayerPrefs.SetString(_snapshotSaveName, snapshotKeys.toJson());
			PlayerPrefs.SetFloat(TIMESTAMP, (float)DateTime.UtcNow.Subtract(epochStart).TotalMilliseconds);
			#endif
		#endif

		PlayerPrefs.Save();
	}

	public static void DeleteAll()
	{
		#if CLOUDDATA_IMPLEMENTED

			#if UNITY_IOS
			P31Prefs.removeAll();
			#elif UNITY_ANDROID
			snapshotKeys.Clear();
			PlayGameServices.deleteSnapshot(_snapshotSaveName);
			#endif
		#endif

		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
	}
}
