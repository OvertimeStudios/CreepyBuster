using UnityEngine;
using System.Collections;

public class TutorialController : MonoBehaviour 
{
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

	// Use this for initialization
	void OnEnable () 
	{
		EnemyLife.OnDied += EnemyDied;
		EnemyMovement.OnOutOfScreen += EnemyOutOfScreen;
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerUpEvent += OnFingerUp;

		enemyCounter = 0;
		textsNumber = 0;
		doubleEnemy = false;

		tutorial.gameObject.SetActive (true);
		tutorialText = tutorial.FindChild("Text").GetComponent<UILabel> ();

		if(Debug.isDebugBuild && !runTutorial)
		{
			Global.RunTutorial = false;
			gameObject.SetActive (false);
		}
		else
			StartCoroutine (Run ());
	}

	void OnDisable()
	{
		EnemyLife.OnDied -= EnemyDied;
		EnemyMovement.OnOutOfScreen -= EnemyOutOfScreen;
		FingerDetector.OnFingerDownEvent -= OnFingerDown;
		FingerDetector.OnFingerUpEvent -= OnFingerUp;

		if(tutorial != null)
			tutorial.gameObject.SetActive (false);

		StopAllCoroutines ();
	}

	void Awake()
	{
		instance = this;
	}

	void Update()
	{
		tutorialText.enabled = !AttackTargets.IsAttacking;
	}

	private void OnFingerDown(FingerDownEvent e)
	{
		Popup.Hide ();
		Time.timeScale = 1f;
	}

	private void OnFingerUp(FingerUpEvent e)
	{
		Popup.ShowBlank ("Put your finger back into screen!");
		Time.timeScale = 0f;
	}

	private IEnumerator Run()
	{
		//first rule
		yield return new WaitForSeconds(ShowNextText());

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

		//One more thing: got hit
		yield return new WaitForSeconds(ShowNextText());

		Global.RunTutorial = false;
		gameObject.SetActive (false);
	}

	private float ShowNextText()
	{
		TutorialText tText = texts [textsNumber];

		tutorialText.text = tText.text;

		textsNumber++;

		return tText.time;
	}

	private void SpawnEnemy(GameObject enemy)
	{
		SpawnController.SpawnEnemy (enemy);

		enemyCounter++;
	}

	private void EnemyDied(GameObject enemy)
	{
		enemyCounter--;
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

		enemyCounter--;

		if(!tutorialText.text.Contains("Try again"))
			tutorialText.text = "Try again! \n" + tutorialText.text;
	}
}

[System.Serializable]
public class TutorialText
{
	public string text;
	public float time;
}
