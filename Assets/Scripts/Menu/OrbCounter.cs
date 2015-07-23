using UnityEngine;
using System.Collections;

public class OrbCounter : MonoBehaviour 
{
	private UILabel orbs;

	void OnEnable()
	{
		Global.OnOrbUpdated += UpdateLabel;

		UpdateLabel ();
	}

	void OnDisable()
	{
		Global.OnOrbUpdated -= UpdateLabel;
	}
	
	void UpdateLabel()
	{
		orbs = transform.FindChild ("Orb Label").GetComponent<UILabel> ();
		
		orbs.text = Global.TotalOrbs.ToString ();
	}
}
