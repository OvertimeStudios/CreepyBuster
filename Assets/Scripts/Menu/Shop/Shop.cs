using UnityEngine;
using System.Collections;

public class Shop : Singleton<Shop> 
{
	[Header("Shop Achievement")]
	public Achievement achievement;

	[Header("Filter")]
	public UIProgressBar progressBar;
	public float upgradePosition;
	public float packPosition;
	public float consumablesPosition;
	private TweenPosition tween;

	[Header("Web")]
	public GameObject goToUpgade;
	public GameObject goToPacks;

	// Use this for initialization
	void Start () 
	{
		tween = GetComponentInChildren<TweenPosition>();
		ItemShop.OnItemBought += VerifyAchievement;

		#if UNITY_WEBGL
		goToUpgade.SetActive(false);
		goToPacks.SetActive(false);
		#endif
	}

	private void VerifyAchievement()
	{
		Global.UpgradesBought++;

		if(!achievement.unlocked && Global.UpgradesBought == achievement.value)
		{
			achievement.Unlock();
			MenuController.Instance.ShowAchievements();
		}
	}

	public void GoToUpgrade()
	{
		tween.ResetToBeginning();
		tween.from = new Vector3(0, progressBar.value, 0);
		tween.to = new Vector3(0, upgradePosition, 0);

		tween.PlayForward();

		StartCoroutine(UpdateProgressBarPosition());
	}

	public void GoToPack()
	{
		tween.ResetToBeginning();
		tween.from = new Vector3(0, progressBar.value, 0);
		tween.to = new Vector3(0, packPosition, 0);
		
		tween.PlayForward();

		StartCoroutine(UpdateProgressBarPosition());
	}

	public void GoToConsumables()
	{
		tween.ResetToBeginning();
		tween.from = new Vector3(0, progressBar.value, 0);
		tween.to = new Vector3(0, consumablesPosition, 0);
		
		tween.PlayForward();
		
		StartCoroutine(UpdateProgressBarPosition());
	}

	private IEnumerator UpdateProgressBarPosition()
	{
		while(true)
		{
			progressBar.value = tween.value.y;
			yield return null;
		}
	}

	public void OnTweenFinished()
	{
		StopAllCoroutines();
	}
}
