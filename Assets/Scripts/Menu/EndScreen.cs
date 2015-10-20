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

	void OnEnable()
	{
		foreach(UIButton button in rewardButton.GetComponents<UIButton>())
			button.isEnabled = true;

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
	}

	public void AskDoubleOrbs()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		Popup.ShowYesNo("Wanna watch a video to double your orbs earned from this game?", PlayVideo, null);
	}

	private void PlayVideo()
	{
		RewardedVideoPlayer.ShowAd(GiveDoubleOrbs);
	}

	private void GiveDoubleOrbs()
	{
		Global.TotalOrbs += totalOrbs;
		Global.OrbsCollected += totalOrbs;

		multiplierLabel.gameObject.SetActive(true);

		currentMultiplier *= 2;
		totalOrbs = x1orbs * currentMultiplier;

		multiplierLabel.text = currentMultiplier + "x " + x1orbs;
		totalOrbsLabel.text = totalOrbs.ToString();

		foreach(UIButton button in rewardButton.GetComponents<UIButton>())
			button.isEnabled = false;
	}
}
