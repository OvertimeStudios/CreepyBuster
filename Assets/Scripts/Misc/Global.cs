using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour 
{
	#region keys
	public const string TOTAL_ORBS = "totalOrbs";
	public const string HIGH_SCORE = "highScore";
	#endregion

	private static bool isLoaded;

	#region game variables
	private static int highScore;
	private static int sessionsScore;
	private static int totalOrbs;
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

	public static int TotalOrbs
	{
		get
		{
			if(!isLoaded)
				Load ();
			
			return totalOrbs;
		}
		
		set
		{
			totalOrbs = value;
			
			Save ();
		}
	}

	#endregion

	private static void Load()
	{
		isLoaded = true;

		sessionsScore = 0;

		if(PlayerPrefs.HasKey(HIGH_SCORE))
		{
			highScore = PlayerPrefs.GetInt(HIGH_SCORE);
			totalOrbs = PlayerPrefs.GetInt(TOTAL_ORBS);
		}
		else
		{
			//initialize
			highScore = 0;
			totalOrbs = 0;

			Save();
		}
	}

	private static void Save()
	{
		PlayerPrefs.SetInt (HIGH_SCORE, highScore);
		PlayerPrefs.SetInt (TOTAL_ORBS, totalOrbs);

		PlayerPrefs.Save ();
	}
}
