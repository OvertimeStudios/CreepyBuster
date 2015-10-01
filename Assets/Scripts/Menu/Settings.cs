using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour 
{
	private GameObject logout;
	//private UILabel greeting;

	void Awake()
	{
		//logout = transform.FindChild ("Logout").gameObject;
		//greeting = transform.FindChild ("FB - Login").GetComponent<UILabel>();
	}

	void OnEnable()
	{
		HandleLoginSection ();

		Global.OnLoggedIn += HandleLoginSection;
		Global.OnLoggedOut += HandleLoginSection;
	}

	void OnDisable()
	{
		Global.OnLoggedIn -= HandleLoginSection;
		Global.OnLoggedOut -= HandleLoginSection;

		ChangeLanguage changeLanguage = transform.FindChild("Language").GetComponent<ChangeLanguage>();
		if(changeLanguage.opened)
			changeLanguage.OpenClose();
	}

	private void HandleLoginSection()
	{
		if(Global.IsLoggedIn)
		{
			//greeting.text = "Hello" + ", " + Global.user.firstname;
			//greeting.gameObject.GetComponent<Collider2D>().enabled = false;
			//greeting.gameObject.GetComponent<UIButton>().enabled = false;
			//logout.SetActive(true);
		}
		else
		{
			//greeting.text = "Login";
			//greeting.gameObject.GetComponent<Collider2D>().enabled = true;
			//greeting.gameObject.GetComponent<UIButton>().enabled = true;
			//logout.SetActive(false);
		}
	}
}
