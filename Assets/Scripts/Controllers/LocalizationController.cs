using UnityEngine;
using System;
using System.Collections;

public class LocalizationController : MonoBehaviour 
{
	#region Actions
	public static event Action OnChanged;
	#endregion

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

			if(OnChanged != null)
				OnChanged();
		}
	}

	// Use this for initialization
	void Start () 
	{
		if(Global.Language == "")
			CurrentLanguage = (Application.systemLanguage == SystemLanguage.Portuguese) ? Language.Portuguese : Language.English;
		else
			CurrentLanguage = (Language)System.Enum.Parse (typeof(Language), Global.Language);
	}
}
