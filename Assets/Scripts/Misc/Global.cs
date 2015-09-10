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
	private const string RAY3 = "ray3";
	private const string RAY4 = "ray4";
	private const string RAY5 = "ray5";
	private const string RANGE_SUPER = "superRange";
	private const string RANGE_MEGA = "megaRange";
	private const string RANGE_MASTER = "masterRange";
	private const string DAMAGE_SUPER = "superDamage";
	private const string DAMAGE_MEGA = "megaDamage";
	private const string DAMAGE_ULTRA = "masterDamage";
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
	private static int ray3;
	private static int ray4;
	private static int ray5;
	private static int superRange;
	private static int megaRange;
	private static int masterRange;
	private static int superDamage;
	private static int megaDamage;
	private static int ultraDamage;

	private static int musicOn;
	private static int soundOn;
	private static int vibrate;

	private static int firstTimeTutorial;
	private static int tutorialEnabled;

	private static string language;
	#endregion

	#region session variables
	private static bool loggedIn = false;
	private static bool tutorial = true;
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

	public static bool RunTutorial
	{
		get { return tutorial; }

		set { tutorial = value; }
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

	public static bool Ray3Purchased
	{
		get 
		{
			if(!isLoaded)
				Load ();

			return ray3 == 1;
		}
		set
		{
			ray3 = (value == true) ? 1 : 0;

			PlayerPrefs.SetInt (RAY3, ray3);
			PlayerPrefs.Save ();
		}
	}

	public static bool Ray4Purchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return ray4 == 1;
		}
		set
		{
			ray4 = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (RAY4, ray4);
			PlayerPrefs.Save ();
		}
	}

	public static bool Ray5Purchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return ray5 == 1;
		}
		set
		{
			ray5 = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (RAY5, ray5);
			PlayerPrefs.Save ();
		}
	}

	public static bool SuperRangePurchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return superRange == 1;
		}
		set
		{
			superRange = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (RANGE_SUPER, superRange);
			PlayerPrefs.Save ();
		}
	}

	public static bool MegaRangePurchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return megaRange == 1;
		}
		set
		{
			megaRange = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (RANGE_MEGA, megaRange);
			PlayerPrefs.Save ();
		}
	}

	public static bool MasterRangePurchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return masterRange == 1;
		}
		set
		{
			masterRange = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (RANGE_MASTER, masterRange);
			PlayerPrefs.Save ();
		}
	}

	public static bool SuperDamagePurchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return superDamage == 1;
		}
		set
		{
			superDamage = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (DAMAGE_SUPER, superDamage);
			PlayerPrefs.Save ();
		}
	}

	public static bool MegaDamagePurchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return megaDamage == 1;
		}
		set
		{
			megaDamage = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (DAMAGE_MEGA, megaDamage);
			PlayerPrefs.Save ();
		}
	}

	public static bool UltraDamagePurchased
	{
		get 
		{
			if(!isLoaded)
				Load ();
			
			return ultraDamage == 1;
		}
		set
		{
			ultraDamage = (value == true) ? 1 : 0;
			
			PlayerPrefs.SetInt (DAMAGE_ULTRA, ultraDamage);
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
			ray3 = PlayerPrefs.GetInt(RAY3);
			ray4 = PlayerPrefs.GetInt(RAY4);
			ray5 = PlayerPrefs.GetInt(RAY5);
			superRange = PlayerPrefs.GetInt(RANGE_SUPER);
			megaRange = PlayerPrefs.GetInt(RANGE_MEGA);
			masterRange = PlayerPrefs.GetInt(RANGE_MASTER);
			superDamage = PlayerPrefs.GetInt(DAMAGE_SUPER);
			megaDamage = PlayerPrefs.GetInt(DAMAGE_MEGA);
			ultraDamage = PlayerPrefs.GetInt(DAMAGE_ULTRA);
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
			ray3 = 0;
			ray4 = 0;
			ray5 = 0;
			superRange = 0;
			megaRange = 0;
			masterRange = 0;
			superDamage = 0;
			megaDamage = 0;
			ultraDamage = 0;
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
		ray3 = 0;
		ray4 = 0;
		ray5 = 0;
		superRange = 0;
		megaRange = 0;
		masterRange = 0;
		superDamage = 0;
		megaDamage = 0;
		ultraDamage = 0;

		SaveAll ();

		if(OnPurchasesCleared != null)
			OnPurchasesCleared();
	}

	private static void SaveAll()
	{
		PlayerPrefs.SetInt (HIGH_SCORE, highScore);
		PlayerPrefs.SetInt (TOTAL_ORBS, totalOrbs);
		PlayerPrefs.SetInt (RAY3, ray3);
		PlayerPrefs.SetInt (RAY4, ray4);
		PlayerPrefs.SetInt (RAY5, ray5);
		PlayerPrefs.SetInt (RANGE_SUPER, superRange);
		PlayerPrefs.SetInt (RANGE_MEGA, megaRange);
		PlayerPrefs.SetInt (RANGE_MASTER, masterRange);
		PlayerPrefs.SetInt (DAMAGE_SUPER, superDamage);
		PlayerPrefs.SetInt (DAMAGE_MEGA, megaDamage);
		PlayerPrefs.SetInt (DAMAGE_ULTRA, ultraDamage);
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

	public static void LogIn()
	{
		loggedIn = true;

		if(OnLoggedIn != null)
			OnLoggedIn();
	}

	public static void LogOut()
	{
		loggedIn = false;

		if(OnLoggedOut != null)
			OnLoggedOut();
	}
}
