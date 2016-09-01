using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class AutomateBuild : Editor
{
	[MenuItem("Overtime Studios/Build/Android/Sign Application")]
	public static void SignApplication() 
	{
		PlayerSettings.Android.keyaliasPass = "Overt!m3";
		PlayerSettings.Android.keystorePass = "Overt!m3";
	}

	//[MenuItem("Overtime Studios/Build/Android/Sign and Build")]
	public static void SignAndBuildApplication() 
	{
		SignApplication();

		string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");

		List<string> levels = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			if(scene.enabled)
				levels.Add(scene.path);
		}
			
		BuildPipeline.BuildPlayer(levels.ToArray(), path, BuildTarget.Android, BuildOptions.None);

		/*
		// Run the game (Process class from System.Diagnostics).
		Process proc = new Process();
		proc.StartInfo.FileName = path + "BuiltGame.exe";
		proc.Start();*/
	}
}
