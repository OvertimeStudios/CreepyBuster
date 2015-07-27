using UnityEngine;
using System.Collections;
using UnityEditor;
#if ADMOB_IMPLEMENTED
[CustomEditor(typeof(AdMobHelper))]
public class AdMobHelperEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		AdMobHelper myScript = target as AdMobHelper;

		myScript.isTest = GUILayout.Toggle (myScript.isTest, "Is Test");

		if (myScript.isTest)
		{
			EditorGUILayout.PropertyField(serializedObject.FindProperty("testDeviceIDs"), true);
			serializedObject.ApplyModifiedProperties();
		}

		EditorGUILayout.Space ();
		EditorGUILayout.LabelField ("Preloads");
		myScript.preloadBanner = GUILayout.Toggle (myScript.preloadBanner, "Preload banner");
	}
}
#endif