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

	public GameObject tapAndHold;
	public GameObject hud;
	private TweenPosition wallTop;
	private TweenPosition wallBottom;
	private UILabel highScore;

	public float timeToStartGame = 3f;
	private float timeCounter;
	private float initialTapAndHoldRotation;
	public float maxTapAndHoldRotation;

	private GameObject trailRenderer;

	[Header("Telas Tween")]
	public TweenPosition menuTween;
	public Transform mainScreen;
	public Transform shopScreen;
	
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
		wallTop = mainScreen.FindChild ("WallTop").GetComponent<TweenPosition> ();
		wallBottom = mainScreen.FindChild ("WallBottom").GetComponent<TweenPosition> ();
		highScore = wallTop.transform.FindChild ("High Score").FindChild ("Score").GetComponent<UILabel> ();

		initialTapAndHoldRotation = tapAndHold.GetComponent<Rotate> ().rotVel;

		timeCounter = timeToStartGame;

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

	void Update()
	{
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.Escape))
		{
			Global.ClearPurchasedOnly();
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
		if(!GameController.isGameRunning && !wallTop.enabled && timeCounter < timeToStartGame)
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

		highScore.text = Global.HighScore.ToString ();
	}

	public void MoveToMain()
	{
		Vector3 from = menuTween.transform.localPosition;
		Vector3 to = -mainScreen.localPosition;

		menuTween.ResetToBeginning ();
		
		menuTween.from = from;
		menuTween.to = to;

		menuTween.PlayForward ();
	}

	public void MoveToShop()
	{
		Vector3 from = menuTween.transform.localPosition;
		Vector3 to = -shopScreen.localPosition;
		
		menuTween.ResetToBeginning ();
		
		menuTween.from = from;
		menuTween.to = to;
		
		menuTween.PlayForward ();
	}
}
