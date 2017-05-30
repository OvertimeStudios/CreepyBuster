using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour 
{
	enum Answer
	{
		None,
		Yes,
		No,
	}

	public GameObject basicEnemy;
	public GameObject followerEnemy;
	private int enemyCounter;
	private bool doubleEnemy;

	public Camera hudCamera;
	public Transform tutorial;
	private UILabel tutorialText;

	public TutorialText[] texts;
	private int textsNumber = 0;

	public bool runTutorial = true;
	public bool runEverySession = true;
	private static bool tutorialCompleted = false;
	public static bool running;
	public static bool canTakeOffFinger;
	private static bool used3DTouch;

	private static Answer tutorialAnswer;

	#region singleton
	private static TutorialController instance;
	public static TutorialController Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<TutorialController>();

			return instance;
		}
	}
	#endregion

	#region get / set
	public static bool CanLose
	{
		get { return canTakeOffFinger; }
	}

	public static bool IsRunning
	{
		get { return running; }
	}
	#endregion

	// Use this for initialization
	void OnEnable () 
	{
		EnemyLife.OnDied += EnemyDied;
		EnemyMovement.OnOutOfScreen += EnemyOutOfScreen;
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerUpEvent += OnFingerUp;
		GameController.OnGameEnding += GameEnding;

		enemyCounter = 0;
		textsNumber = 0;
		doubleEnemy = false;
		canTakeOffFinger = false;
		used3DTouch = false;

		if(GameController.isGameRunning && runTutorial)
			StartCoroutine (Run ());

		StartCoroutine(ActivateTutorialGameObject());
	}

	private IEnumerator ActivateTutorialGameObject()
	{
		yield return new WaitForEndOfFrame();

		tutorial.gameObject.SetActive (true);
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= EnemyDied;
		EnemyMovement.OnOutOfScreen -= EnemyOutOfScreen;
		FingerDetector.OnFingerDownEvent -= OnFingerDown;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;
		GameController.OnGameEnding -= GameEnding;

		if(tutorial != null)
			tutorial.gameObject.SetActive (false);

		StopAllCoroutines ();
	}

	void Awake()
	{
		instance = this;
		tutorialText = tutorial.FindChild("Text").GetComponent<UILabel> ();

		if(runEverySession)
			Global.IsTutorialEnabled = true;
	}

	void Update()
	{
		tutorialText.enabled = !AttackTargets.IsAttacking;
	}

	private void OnFingerDown(FingerDownEvent e)
	{
		//Popup.Hide ();
		//Time.timeScale = 1f;
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		if(!canTakeOffFinger)
		{
			//Popup.ShowBlank (Localization.Get("FINGER_ON_SCREEN"));
			//Time.timeScale = 0f;
		}
	}

	private void YesTutorial()
	{
		tutorialAnswer = Answer.Yes;
	}

	private void NoTutorial()
	{
		tutorialAnswer = Answer.No;
	}

	private IEnumerator Run()
	{
		running = true;

		if(runEverySession || Global.IsFirstTimeTutorial)
		{
			tutorialAnswer = Answer.None;

			//show popup to confirm tutorial
			/*Popup.ShowYesNo(Localization.Get("WANT_TUTORIAL"), YesTutorial, NoTutorial, true);

			while(tutorialAnswer == Answer.None)
			{
				//Debug.Log("Waiting for answer");
				yield return null;
			}

			if(tutorialAnswer == Answer.No)
			{
				End ();
				yield break;
			}*/

			//first rule
			yield return new WaitForSeconds(ShowNextText());

			canTakeOffFinger = true;

			//second rule
			yield return new WaitForSeconds(ShowNextText());

			//look, an enemy!
			yield return new WaitForSeconds(ShowNextText());

			SpawnEnemy (basicEnemy);

			while (enemyCounter > 0)
				yield return null;

			//Great! 
			yield return new WaitForSeconds(ShowNextText());

			SpawnEnemy (followerEnemy);

			while (enemyCounter > 0)
				yield return null;

			//Excelent! level up
			yield return new WaitForSeconds(ShowNextText());

			while (LevelDesign.PlayerLevel < 1)
			{
				if(enemyCounter == 0)
					SpawnEnemy(followerEnemy);

				yield return null;
			}

			//2 rays
			yield return new WaitForSeconds(ShowNextText());

			doubleEnemy = true;
			SpawnEnemy (basicEnemy);
			SpawnEnemy (basicEnemy);

			while (enemyCounter > 0)
				yield return null;

			//force touch (iOS only)
			if(Input.touchPressureSupported)
			{
				yield return new WaitForSeconds(ShowNextText());

				SpawnEnemy (basicEnemy);

				while (enemyCounter > 0 || !used3DTouch)
				{
					if(!used3DTouch)
					{
						if(Input.touchCount > 0)
						{
							if(Input.GetTouch(0).pressure > 1f)
								used3DTouch = true;
						}
					}

					yield return null;
				}
			}
			else
			{
				SkipNextTutorial();
				SkipNextTutorial();
			}

			//One more thing: got hit
			yield return new WaitForSeconds(ShowNextText());
		}
		else
		{
			string tutorial = "";

			if(Global.GamesPlayed % 3 == 0)
				tutorial = Localization.Get("TUTORIAL_3DTOUCH");
			else
			{
				tutorial = Localization.Get("TUTORIAL");

				if(Global.IsFirstTimeTutorial)
					tutorial += " " + Localization.Get("TUTORIAL_ADD");
			}

			tutorialText.text = tutorial;

			yield return new WaitForSeconds(5f);
		}

		End();
	}

	private static void End()
	{
		running = false;

		Instance.runEverySession = false;
		Global.IsTutorialEnabled = false;
		Global.IsFirstTimeTutorial = false;
		Hide ();
	}

	private float ShowNextText()
	{
		Debug.Log("Show Next Text");
		TutorialText tText = texts [textsNumber];

		tutorialText.text = Localization.Get(tText.text);

		textsNumber++;

		return tText.time;
	}

	private void SkipNextTutorial()
	{
		textsNumber++;
	}

	private void SpawnEnemy(GameObject enemy)
	{
		SpawnController.SpawnEnemy (enemy);

		enemyCounter++;

		Debug.Log("SpawnEnemy " + enemyCounter);
	}

	private void EnemyDied(GameObject enemy)
	{
		enemyCounter--;
		Debug.Log("EnemyDied " + enemyCounter);
	}

	private void EnemyOutOfScreen(GameObject enemy)
	{
		if(doubleEnemy)
		{
			if(enemyCounter == 1)
			{
				SpawnEnemy (basicEnemy);
				SpawnEnemy (basicEnemy);
			}
		}
		else
		{
			SpawnEnemy (basicEnemy);
		}

		//enemyCounter--;
		Debug.Log("EnemyOutOfScreen " + enemyCounter);

		if(!tutorialText.text.Contains("Try again"))
			tutorialText.text = "Try again! \n" + tutorialText.text;
	}

	public void Stop()
	{

	}

	private void GameEnding()
	{
		running = false;
		Hide ();
	}

	public static void Hide()
	{
		Instance.gameObject.SetActive (false);
	}
}

[System.Serializable]
public class TutorialText
{
	public string alias;
	public string text;
	public float time;
}
