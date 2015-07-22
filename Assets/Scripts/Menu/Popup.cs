using UnityEngine;
using System;
using System.Collections;

public class Popup : MonoBehaviour 
{
	#region Action
	public static event Action YesClicked;
	public static event Action NoClicked;
	public static event Action OkClicked;
	#endregion

	private static GameObject yes;
	private static GameObject no;
	private static GameObject ok;
	private static GameObject video;
	private static UILabel descricao;

	#region singleton
	private static Popup instance;
	public static Popup Instance
	{
		get 
		{ 
			if(instance == null)
				instance = GameObject.FindObjectOfType<Popup>();

			return instance; 
		}
	}
	#endregion

	#region get / set
	public static bool IsActive
	{
		get { return Instance.gameObject.activeInHierarchy; }
	}
	#endregion

	// Use this for initialization
	void Start () 
	{
		instance = this;

		yes = transform.FindChild ("Yes").gameObject;
		no = transform.FindChild ("No").gameObject;
		ok = transform.FindChild ("Ok").gameObject;
		video = transform.FindChild ("Video").gameObject;
		descricao = transform.FindChild ("Descricao").GetComponent<UILabel>();

		Hide ();
	}

	public static void ShowYesNo(string description, Action yesAction, Action noAction)
	{
		Instance.gameObject.SetActive (true);

		ok.SetActive (false);
		video.SetActive (false);

		yes.SetActive (true);
		no.SetActive (true);

		YesClicked += yesAction;
		NoClicked += noAction;

		descricao.text = description;
	}

	public static void ShowVideoNo(string description, Action videoAction, Action noAction)
	{
		Instance.gameObject.SetActive (true);
		
		ok.SetActive (false);
		yes.SetActive (false);
		
		video.SetActive (true);
		no.SetActive (true);
		
		YesClicked += videoAction;
		NoClicked += noAction;
		
		descricao.text = description;
	}

	public static void ShowOk(string description)
	{
		ShowOk (description, null);
	}

	public static void ShowOk(string description, Action okAction)
	{
		Instance.gameObject.SetActive (true);

		ok.SetActive (true);
		yes.SetActive (false);
		no.SetActive (false);
		video.SetActive (false);

		OkClicked += okAction;

		descricao.text = description;
	}

	public void YesAnswer()
	{
		Hide ();

		if (YesClicked != null)
			YesClicked ();

		YesClicked = null;
		NoClicked = null;
	}

	public void NoAnswer()
	{
		Hide ();

		if (NoClicked != null)
			NoClicked ();

		YesClicked = null;
		NoClicked = null;
	}

	public void OkAnswer()
	{
		Hide ();

		if (OkClicked != null)
			OkClicked ();
		
		OkClicked = null;
	}

	public void Hide()
	{
		gameObject.SetActive (false);
	}
}
