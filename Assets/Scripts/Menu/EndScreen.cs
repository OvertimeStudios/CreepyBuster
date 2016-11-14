using UnityEngine;
using System.Collections;

public class EndScreen : MonoBehaviour 
{
	public UILabel score;
	public UILabel highScore;

	private int currentMultiplier;
	public UILabel multiplierLabel;

	private int x1orbs;
	private int totalOrbs;
	public UILabel totalOrbsLabel;

	public Transform rewardButton;
	public UISprite rewardGlow;

	public GameObject general;
	public GameObject battleStats;
	public GameObject watchReplay;
	public GameObject shopButton;

	public UILabel kills;
	public UILabel orbsCollected;
	public UILabel timeInGame;
	public UILabel powerUpsCollected;
	public UILabel hitsTaken;
	public UILabel longestKillStreak;

	void OnEnable()
	{
		#if UNITY_WEBGL
		rewardButton.gameObject.SetActive(false);
		watchReplay.SetActive(false);
		shopButton.SetActive(false);
		#endif

		foreach(UIButton button in rewardButton.GetComponents<UIButton>())
			button.isEnabled = true;

		rewardGlow.enabled = true;

		score.text = GameController.Score.ToString();
		highScore.text = Global.HighScore.ToString();

		//get total orbs
		int totalOrbsFromPoints = (int)Mathf.Floor (GameController.Score / GameController.Instance.pointsPerOrb);
		x1orbs = GameController.orbsCollected + totalOrbsFromPoints;

		currentMultiplier = Global.OrbsMultiplier;
		multiplierLabel.gameObject.SetActive(currentMultiplier != 1);
		multiplierLabel.text = currentMultiplier + "x " + x1orbs;

		totalOrbs = x1orbs * currentMultiplier;

		totalOrbsLabel.text = totalOrbs.ToString();

		Global.TotalOrbs += totalOrbs;
		Global.OrbsCollected += totalOrbs;

		//fill game stats
		kills.text = GameController.KillCount.ToString();
		orbsCollected.text = GameController.orbsCollected.ToString();
		timeInGame.text = string.Format("{0} s", (int)GameController.matchTime);
		powerUpsCollected.text = (GameController.frozenCollected + GameController.deathRayCollected + 
		                          GameController.invencibilityCollected + GameController.levelUpCollected).ToString();
		hitsTaken.text = (GameController.hitsByBasic + GameController.hitsByBoomerang + GameController.hitsByZigZag +
		                  GameController.hitsByCharger + GameController.hitsByLegion + GameController.hitsByFollower +
		                  GameController.hitsByBoss1 + GameController.hitsByBoss2 + GameController.hitsByBoss3).ToString();
		longestKillStreak.text = GameController.maxStreak.ToString();
	}

	public void AskDoubleOrbs()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		Popup.ShowYesNo(Localization.Get("DOUBLE_ORBS_QUESTION"), PlayVideo, null);
	}

	private void PlayVideo()
	{
		RewardedVideoPlayer.ShowAd(GiveDoubleOrbs);
	}

	private void GiveDoubleOrbs()
	{
		GameController.watchedDoubleOrbs = true;

		Global.TotalOrbs += totalOrbs;
		Global.OrbsCollected += totalOrbs;

		multiplierLabel.gameObject.SetActive(true);

		currentMultiplier *= 2;
		totalOrbs = x1orbs * currentMultiplier;

		multiplierLabel.text = currentMultiplier + "x " + x1orbs;
		totalOrbsLabel.text = totalOrbs.ToString();

		foreach(UIButton button in rewardButton.GetComponents<UIButton>())
			button.isEnabled = false;

		rewardGlow.enabled = false;
	}

	public void OpenBattleStats()
	{
		general.SetActive(false);
		battleStats.SetActive(true);
	}

	public void CloseBattleStats()
	{
		general.SetActive(true);
		battleStats.SetActive(false);
	}
}
