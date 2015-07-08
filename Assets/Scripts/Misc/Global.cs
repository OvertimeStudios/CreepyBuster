using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour 
{
	private static bool isLoaded;

	#region game variables
	private static int highScore;
	private static int sessionsScore;
	#endregion

	#region get/set
	public static bool IsLoaded
	{
		get 
		{
			return isLoaded;
		}
	}

	public static int HighScore
	{
		get
		{
			if(!isLoaded)
				Load ();

			return highScore;
		}

		set
		{
			highScore = value;

			Save ();
		}
	}

	public static int SessionScore
	{
		get
		{
			if(!isLoaded)
				Load ();

			return sessionsScore;
		}
		set
		{
			sessionsScore = value;
		}
	}
	#endregion

	private static void Load()
	{
		isLoaded = true;

		sessionsScore = 0;

		if(PlayerPrefs.HasKey("highScore"))
		{
			highScore = PlayerPrefs.GetInt("highScore");
		}
		else
		{
			//initialize
			highScore = 0;

			Save();
		}
	}

	private static void Save()
	{
		PlayerPrefs.SetInt ("highScore", highScore);

		PlayerPrefs.Save ();
	}
}
