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
		enemyCounter = 0;
		textsNumber = 0;
		doubleEnemy = false;

		Debug.Log ("Enabled Tutorial");

		tutorial.gameObject.SetActive (true);
		tutorialText = tutorial.FindChild("Text").GetComponent<UILabel> ();

		StartCoroutine (Run ());

		EnemyLife.OnDied += EnemyDied;
		EnemyMovement.OnOutOfScreen += EnemyOutOfScreen;
		FingerDetector.OnFingerDownEvent += OnFingerDown;
		FingerDetector.OnFingerMotionEvent += OnFingerMove;
	}

	void OnDisable()
	{
		tutorial.gameObject.SetActive (false);
		StopAllCoroutines ();

		EnemyLife.OnDied -= EnemyDied;
		EnemyMovement.OnOutOfScreen -= EnemyOutOfScreen;
		FingerDetector.OnFingerDownEvent -= OnFingerDown;
		FingerDetector.OnFingerMotionEvent -= OnFingerMove;
	}

	void Start()
	{
		instance = this;
	}

	void Update()
	{
		tutorialText.enabled = !AttackTargets.IsAttacking;
	}

	private IEnumerator Run()
	{
		//welcome!
		yield return new WaitForSeconds(ShowNextText());

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

		//Another one (follower)
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

	private void OnFingerDown(FingerDownEvent e)
	{
		RepositionText (e.Position);
	}

	private void OnFingerMove(FingerMotionEvent e)
	{
		RepositionText (e.Position);
	}

	private void RepositionText(Vector2 position)
	{
		/*Vector3 pos = Camera.main.ScreenToViewportPoint ((Vector3)position);
		pos = hudCamera.ViewportToScreenPoint(pos);
		tutorial.localPosition = position;*/

		float posX = (position.x * 2f) - Screen.width;
		float posY = (position.y * 2f) - Screen.height;

		tutorial.localPosition = new Vector3(posX, posY, 0f);
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
