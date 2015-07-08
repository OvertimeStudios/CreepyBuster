using UnityEngine;
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

	public TweenPosition wallTop;
	public TweenPosition wallBottom;
	public UILabel highScore;
	public UILabel sessionsScore;
	public UILabel currentScore;

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
		UpdateScore ();
	}

	void OnFingerDown(FingerDownEvent e)
	{
		if(!GameController.isGameRunning && !wallTop.enabled)
		{
			OpenPanel();

			if(OnPanelOpening != null)
				OnPanelOpening();
		}
	}

	public void TweenFinished()
	{
		wallTop.enabled = wallBottom.enabled = false;

		if(wallTop.direction == AnimationOrTween.Direction.Forward)
		{
			GameController.Instance.StartGame ();

			if(OnPanelOpened != null)
				OnPanelOpened();
		}
		else
		{
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
