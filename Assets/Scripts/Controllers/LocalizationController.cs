using UnityEngine;
using System.Collections;

public class LocalizationController : MonoBehaviour 
{
	#region singleton
	private static LocalizationController instance;
	private static LocalizationController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<LocalizationController>();

			return instance;
		}
	}
	#endregion

	public enum Language
	{
		English,
		Portuguese,
	}

	/// <summary>
	/// To set new language, use Language.CurrentLanguage instead
	/// </summary>
	private Language language;

	public static Language CurrentLanguage
	{
		get { return Instance.language; }

		set	
		{ 
			Instance.language = value; 
			Localization.language = Instance.language.ToString();
			Global.Language = Instance.language.ToString();
		}
	}

	// Use this for initialization
	void Start () 
	{
		CurrentLanguage = (Language)System.Enum.Parse (typeof(Language), Global.Language);
	}
}
