using UnityEngine;
using System.Collections;

public class OrbCounter : MonoBehaviour 
{
	private UILabel orbs;

	void OnEnable()
	{
		Global.OnOrbUpdated += UpdateLabel;
	}

	void OnDisable()
	{
		Global.OnOrbUpdated -= UpdateLabel;
	}

	// Use this for initialization
	void Start () 
	{
		UpdateLabel ();
	}
	
	void UpdateLabel()
	{
		orbs = transform.FindChild ("Orb Label").GetComponent<UILabel> ();
		
		orbs.text = Global.TotalOrbs.ToString ();
	}
}
