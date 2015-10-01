using UnityEngine;
using System.Collections;
using System;

public class Global : MonoBehaviour 
{
	#region Action
	public static event Action OnOrbUpdated;
	public static event Action OnPurchasesCleared;
	public static event Action OnLoggedIn;
	public static event Action OnLoggedOut;
	#endregion

	#region keys
	private const string FIRST_PLAY = "firstPlay";
	public const string REWARDED_VIDEO_COOLDOWN = "rewardedVideoCooldown";
	private const string TOTAL_ORBS = "totalOrbs";
	private const string HIGH_SCORE = "highScore";
	private const string RAY_LEVEL = "rayLevel";
	private const string DAMAGE_LEVEL = "damageLevel";
	private const string RANGE_LEVEL = "rangeLevel";
	private const string MUSIC_ON = "musicOn";
	private const string SOUNDFX_ON = "soundFX";
	private const string LANGUAGE = "language";
	private const string VIBRATE = "vibrate";
	private const string FIRST_TIME_TUTORIAL = "firstTimeTutorial";
	private const string TUTORIAL_ENABLED = "tutorialEnabled";
	#endregion

	private static bool isLoaded;

	#region game variables
	private static int highScore;
	private static int sessionsScore;
	private static int totalOrbs;
	private static int rayLevel;
	private static int damageLevel;
	private static int rangeLevel;
	private static int musicOn;
	private static int soundOn;
	private static int vibrate;

	private static int firstTimeTutorial;
	private static int tutorialEnabled;

	private static string language;
	#endregion

	#region session variables
	private static bool loggedIn = false;
	#endregion

	#region get/set
	public static FacebookUser user
	{
		get { return FacebookController.User; }
	}

	public static bool IsLoaded
	{
		get 
		{
			return isLoaded;
		}
	}

	public static bool IsLoggedIn
	{
		get { return loggedIn; }
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

			PlayerPrefs.SetInt (HIGH_SCORE, highScore);
			PlayerPrefs.Save ();
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

	/// <summary>
	/// Gets or sets the total orbs. Use this method to increment and Update orbs on game.
	/// </summary>
	/// <value>The total orbs of player.</value>
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
			
			PlayerPrefs.SetInt (TOTAL_ORBS, totalOrbs);
			PlayerPrefs.Save ();

			if(OnOrbUpdated != null)
				OnOrbUpdated();
		}
	}

	public static int RayLevel
	{
		get 
		{
			if(!isLoaded)
				Load ();

			return rayLevel;
		}
		set
		{
			rayLevel = value;

			PlayerPrefs.SetInt (RAY_LEVEL, rayLevel);
			PlayerPrefs.Save ();
		}
	}

	public static int RangeLevel
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return rangeLevel;
		}
		set
		{
			rangeLevel = value;
			
			PlayerPrefs.SetInt (RANGE_LEVEL, rangeLevel);
			PlayerPrefs.Save ();
		}
	}

	public static int DamageLevel
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return damageLevel;
		}
		set
		{
			damageLevel = value;
			
			PlayerPrefs.SetInt (DAMAGE_LEVEL, damageLevel);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsMusicOn
	{
		get
		{
			if(!isLoaded)
				Load ();

			return musicOn == 1;
		}

		set
		{
			musicOn = (value == true) ? 1 : 0;

			PlayerPrefs.SetInt (MUSIC_ON, musicOn);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsSoundOn
	{
		get
		{
			if(!isLoaded)
				Load ();

			return soundOn == 1;
		}
		
		set
		{
			soundOn = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (SOUNDFX_ON, soundOn);
			PlayerPrefs.Save ();
		}
	}

	public static string Language
	{
		get
		{
			if(!isLoaded)
				Load();

			return language;
		}

		set
		{
			language = value;

			PlayerPrefs.SetString(LANGUAGE, language);
			PlayerPrefs.Save();
		}
	}

	public static bool CanVibrate
	{
		get
		{
			if(!isLoaded)
				Load ();
			
			return vibrate == 1;
		}
		
		set
		{
			vibrate = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (VIBRATE, vibrate);
			PlayerPrefs.Save ();
		}
	}

	public static bool FirstPlay
	{
		get { return !PlayerPrefs.HasKey (FIRST_PLAY); }
	}

	public static bool IsFirstTimeTutorial
	{
		get
		{
			if(!isLoaded)
				Load ();

			return firstTimeTutorial == 1;
		}

		set
		{
			firstTimeTutorial = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (FIRST_TIME_TUTORIAL, firstTimeTutorial);
			PlayerPrefs.Save ();
		}
	}

	public static bool IsTutorialEnabled
	{
		get
		{
			if(!isLoaded)
				Load ();
			
			return tutorialEnabled == 1;
		}
		
		set
		{
			tutorialEnabled = (value == true) ? 1 : 0;

			Debug.Log("IsTutorialEnabled? " + IsTutorialEnabled);

			PlayerPrefs.SetInt (TUTORIAL_ENABLED, tutorialEnabled);
			PlayerPrefs.Save ();
		}
	}

	#endregion

	private static void Load()
	{
		if(isLoaded) return;

		sessionsScore = 0;

		if(!FirstPlay)
		{
			highScore = PlayerPrefs.GetInt(HIGH_SCORE);
			totalOrbs = PlayerPrefs.GetInt(TOTAL_ORBS);
			rayLevel = PlayerPrefs.GetInt(RAY_LEVEL);
			rangeLevel = PlayerPrefs.GetInt(RANGE_LEVEL);
			damageLevel = PlayerPrefs.GetInt(DAMAGE_LEVEL);
			musicOn = PlayerPrefs.GetInt(MUSIC_ON);
			soundOn = PlayerPrefs.GetInt(SOUNDFX_ON);
			vibrate = PlayerPrefs.GetInt(VIBRATE);
			firstTimeTutorial = PlayerPrefs.GetInt(FIRST_TIME_TUTORIAL);
			tutorialEnabled = PlayerPrefs.GetInt(TUTORIAL_ENABLED);

			language = PlayerPrefs.GetString(LANGUAGE);
		}
		else
		{
			PlayerPrefs.SetInt(FIRST_PLAY, 1);

			//initialize
			highScore = 0;
			totalOrbs = 0;
			rayLevel = 0;
			rangeLevel = 0;
			damageLevel = 0;
			musicOn = 1;
			soundOn = 1;
			vibrate = 1;
			firstTimeTutorial = 1;
			tutorialEnabled = 1;

			if(Application.systemLanguage == SystemLanguage.Portuguese)
				language = LocalizationController.Language.Portuguese.ToString();
			else
				language = LocalizationController.Language.English.ToString();

			SaveAll();
		}

		isLoaded = true;
	}

	public static void ClearPurchasedOnly()
	{
		damageLevel = 0;
		rangeLevel = 0;
		rayLevel = 0;

		SaveAll ();

		if(OnPurchasesCleared != null)
			OnPurchasesCleared();
	}

	private static void SaveAll()
	{
		PlayerPrefs.SetInt (HIGH_SCORE, highScore);
		PlayerPrefs.SetInt (TOTAL_ORBS, totalOrbs);
		PlayerPrefs.SetInt (RAY_LEVEL, rayLevel);
		PlayerPrefs.SetInt (RANGE_LEVEL, rangeLevel);
		PlayerPrefs.SetInt (DAMAGE_LEVEL, damageLevel);
		PlayerPrefs.SetInt (MUSIC_ON, musicOn);
		PlayerPrefs.SetInt (SOUNDFX_ON, soundOn);
		PlayerPrefs.SetString (LANGUAGE, language);
		PlayerPrefs.SetInt (VIBRATE, vibrate);
		PlayerPrefs.SetInt(FIRST_TIME_TUTORIAL, firstTimeTutorial);
		PlayerPrefs.SetInt(TUTORIAL_ENABLED, tutorialEnabled);

		PlayerPrefs.Save ();
	}

	private static void Reset()
	{
		PlayerPrefs.DeleteAll ();
		PlayerPrefs.Save ();
	}
}
