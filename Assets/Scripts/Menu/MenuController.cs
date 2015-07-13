﻿using UnityEngine;
using System.Collections;
using System;

public class MenuController : MonoBehaviour 
{
	/// <summary>
	/// Occurs when on panel fully open.
	/// </summary>
	public static event Action OnPanelOpened;

	/// <summary>
	/// Occurs when on panel starts to open.
	/// </summary>
	public static event Action OnPanelOpening;

	/// <summary>
	/// Occurs when on panel is fully closed.
	/// </summary>
	public static event Action OnPanelClosed;

	/// <summary>
	/// Occurs when on panel starts to close.
	/// </summary>
	public static event Action OnPanelClosing;

	public GameObject tapAndHold;
	public GameObject hud;
	public TweenPosition wallTop;
	public TweenPosition wallBottom;
	public UILabel highScore;
	public UILabel sessionsScore;
	public UILabel currentScore;

	public float timeToStartGame = 3f;
	private float timeCounter;
	private float initialTapAndHoldRotation;
	public float maxTapAndHoldRotation;

	private GameObject trailRenderer;

	#region singleton
	private static MenuController instance;
	public static MenuController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<MenuController>();

			return instance;
		}
	}
	#endregion

	void OnEnable()
	{
		GameController.OnGameOver += ClosePanel;
		GameController.OnGameOver += UpdateScore;
	}

	void OnDisable()
	{
		GameController.OnGameOver -= ClosePanel;
		GameController.OnGameOver -= UpdateScore;
	}

	// Use this for initialization
	void Start () 
	{
		initialTapAndHoldRotation = tapAndHold.GetComponent<Rotate> ().rotVel;

		hud.SetActive (false);
		UpdateScore ();
	}

	void OnFingerDown(FingerDownEvent e)
	{
		if(!GameController.isGameRunning && !wallTop.enabled)
		{
			if(e.Selection)
			{
				StopCoroutine("CountdownAborted");
				StartCoroutine("CountdownBeginGame", e.Selection);
			}
		}
	}

	IEnumerator CountdownBeginGame(GameObject selection)
	{
		timeCounter = timeToStartGame;
		//float maxY = selection.transform.GetChild (0).localPosition.y;
		trailRenderer = selection;

		Rotate rotate = tapAndHold.GetComponent<Rotate> ();

		while(timeCounter > 0)
		{
			timeCounter -= Time.deltaTime;

			rotate.rotVel = initialTapAndHoldRotation + ((maxTapAndHoldRotation - initialTapAndHoldRotation) * ((timeToStartGame - timeCounter) / timeCounter));

			yield return null;
		}

		rotate.rotVel = initialTapAndHoldRotation;
		trailRenderer.SetActive(false);

		OpenPanel();
		
		if(OnPanelOpening != null)
			OnPanelOpening();
	}

	void OnFingerUp(FingerUpEvent e)
	{
		if(!GameController.isGameRunning && !wallTop.enabled)
		{
			StopCoroutine("CountdownBeginGame");
			StartCoroutine("CountdownAborted");
		}
	}

	private IEnumerator CountdownAborted()
	{
		Rotate rotate = tapAndHold.GetComponent<Rotate> ();

		while(timeCounter < timeToStartGame)
		{
			timeCounter += Time.deltaTime;
			
			rotate.rotVel = initialTapAndHoldRotation + ((maxTapAndHoldRotation - initialTapAndHoldRotation) * ((timeToStartGame - timeCounter) / timeCounter));
			
			yield return null;
		}
	}

	public void TweenFinished()
	{
		wallTop.enabled = wallBottom.enabled = false;

		if(wallTop.direction == AnimationOrTween.Direction.Forward)
		{
			GameController.Instance.StartGame ();

			hud.SetActive (true);

			if(OnPanelOpened != null)
				OnPanelOpened();
		}
		else
		{
			trailRenderer.SetActive(true);

			if(OnPanelClosed != null)
				OnPanelClosed();
		}
	}

	private void OpenPanel()
	{
		wallTop.enabled = wallBottom.enabled = true;
		
		wallTop.PlayForward();
		wallBottom.PlayForward();
	}

	private void ClosePanel()
	{
		wallTop.enabled = wallBottom.enabled = true;
		
		wallTop.PlayReverse();
		wallBottom.PlayReverse();

		hud.SetActive (false);

		if (OnPanelClosing != null)
			OnPanelClosing ();
	}

	private void UpdateScore()
	{
		if (GameController.Score > Global.HighScore)
			Global.HighScore = GameController.Score;

		if (GameController.Score > Global.SessionScore)
			Global.SessionScore = GameController.Score;

		currentScore.text = GameController.Score.ToString ();
		sessionsScore.text = Global.SessionScore.ToString();
		highScore.text = Global.HighScore.ToString ();
	}
}
