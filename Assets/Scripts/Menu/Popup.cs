﻿using UnityEngine;
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

	private static bool autoHide;

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

	void Update () 
	{
		if(Input.GetButtonDown("Cancel"))
		{
			if(no.activeInHierarchy)
				NoAnswer();
		}
	}

	public static void ShowBlank(string description)
	{
		ShowBlank (description, 0f);
	}

	public static void ShowBlank(string description, float hideInTime)
	{
		Instance.gameObject.SetActive (true);
		
		ok.SetActive (false);
		video.SetActive (false);
		yes.SetActive (false);
		no.SetActive (false);

		descricao.text = description;

		if(hideInTime > 0)
			Instance.StartCoroutine(Hide (hideInTime));
	}

	public static void ShowYesNo(string description)
	{
		ShowYesNo (description, true);
	}

	public static void ShowYesNo(string description, bool autoHide)
	{
		ShowYesNo (description, null, null, autoHide);
	}

	public static void ShowYesNo(string description, Action yesAction, Action noAction)
	{
		ShowYesNo (description, yesAction, noAction, true);
	}

	public static void ShowYesNo(string description, Action yesAction, Action noAction, bool autoHide)
	{
		Instance.gameObject.SetActive (true);

		ok.SetActive (false);
		video.SetActive (false);

		yes.SetActive (true);
		no.SetActive (true);

		YesClicked += yesAction;
		NoClicked += noAction;

		descricao.text = description;

		Popup.autoHide = autoHide;
	}

	public static void ShowVideoNo(string description)
	{
		ShowVideoNo (description, true);
	}

	public static void ShowVideoNo(string description, bool autoHide)
	{
		ShowVideoNo (description, null, null, autoHide);
	}

	public static void ShowVideoNo(string description, Action videoAction, Action noAction)
	{
		ShowVideoNo (description, videoAction, noAction, true);
	}

	public static void ShowVideoNo(string description, Action videoAction, Action noAction, bool autoHide)
	{
		Instance.gameObject.SetActive (true);
		
		ok.SetActive (false);
		yes.SetActive (false);
		
		video.SetActive (true);
		no.SetActive (true);
		
		YesClicked += videoAction;
		NoClicked += noAction;
		
		descricao.text = description;

		Popup.autoHide = autoHide;
	}

	public static void ShowOk(string description)
	{
		ShowOk (description, null, true);
	}

	public static void ShowOk(string description, Action okAction)
	{
		ShowOk (description, okAction, true);
	}

	public static void ShowOk(string description, bool autoHide)
	{
		ShowOk (description, null, autoHide);
	}

	public static void ShowOk(string description, Action okAction, bool autoHide)
	{
		Instance.gameObject.SetActive (true);

		ok.SetActive (true);
		yes.SetActive (false);
		no.SetActive (false);
		video.SetActive (false);

		OkClicked += okAction;

		descricao.text = description;

		Popup.autoHide = autoHide;
	}

	public void YesAnswer()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		if(autoHide)
			Hide ();

		Action _YesClicked = YesClicked;
		Action _NoCliked = NoClicked;

		if (YesClicked != null)
			YesClicked ();

		YesClicked -= _YesClicked;
		NoClicked -= _NoCliked;
	}

	public void NoAnswer()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		if(autoHide)
			Hide ();

		Action _YesClicked = YesClicked;
		Action _NoCliked = NoClicked;

		if (NoClicked != null)
			NoClicked ();

		YesClicked -= _YesClicked;
		NoClicked -= _NoCliked;
	}

	public void OkAnswer()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		if(autoHide)
			Hide ();

		Action _OkClicked = OkClicked;

		if (OkClicked != null)
			OkClicked ();
		
		OkClicked -= _OkClicked;
	}

	private static IEnumerator Hide(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);

		Hide ();
	}
	public static void Hide()
	{
		Instance.gameObject.SetActive (false);
	}
}
