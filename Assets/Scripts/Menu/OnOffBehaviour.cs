using UnityEngine;
using System.Collections;

public class OnOffBehaviour : MonoBehaviour 
{
	public enum Type
	{
		Music,
		SoundFX,
		Tutorial,
		Vibrate,
	}

	private enum State
	{
		OFF = 0,
		ON = 1,
	}

	private Transform onButton;
	private Transform offButton;
	private TweenPosition selection;
	private GameObject disabled;
	public Type type;

	private State state;

	// Use this for initialization
	void Awake () 
	{
		onButton = transform.FindChild ("On");
		offButton = transform.FindChild ("Off");

		if(transform.FindChild ("Selection") != null)
			selection = transform.FindChild ("Selection").GetComponent<TweenPosition>();

		if(transform.FindChild("Disabled") != null)
			disabled = transform.FindChild("Disabled").gameObject;

		#if UNITY_WEBPLAYER
		if(type == Type.Vibrate)
			gameObject.SetActive(false);
		#endif
	}

	void OnEnable()
	{
		if (type == Type.Music)
			state = (State)(Global.IsMusicOn ? 1 : 0);
		
		else if (type == Type.SoundFX)
			state = (State)(Global.IsSoundOn ? 1 : 0);
		
		else if(type == Type.Tutorial)
			state = (State)Global.IsTutorialEnabled.GetHashCode();

		else if(type == Type.Vibrate)
			state = (State)(Global.CanVibrate ? 1 : 0);
		
		//turn selection to 'off' or 
		if (state == State.OFF)
		{
			if(selection != null)
				selection.transform.localPosition = offButton.localPosition;
			else
				disabled.SetActive(true);
		}

		ToggleOnOff ();
	}

	public void SelectOn()
	{
		state = State.ON;

		Vector3 from = selection.transform.localPosition;
		
		selection.ResetToBeginning ();
		
		selection.from = from;
		selection.to = onButton.localPosition;
		
		selection.PlayForward ();

		ToggleOnOff ();
	}

	public void SelectOff()
	{
		state = State.OFF;

		Vector3 from = selection.transform.localPosition;

		selection.ResetToBeginning ();
		
		selection.from = from;
		selection.to = offButton.localPosition;
		
		selection.PlayForward ();

		ToggleOnOff ();
	}

	public void Toggle()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		state = (state == State.ON) ? State.OFF : State.ON;

		disabled.SetActive(state == State.OFF);

		ToggleOnOff();
	}

	private void ToggleOnOff ()
	{
		if (type == Type.Music)
		{
			Global.IsMusicOn = (state == State.ON);

			if(state == State.ON)
				SoundController.Instance.UnmuteMusic();
			else
				SoundController.Instance.MuteMusic();
		}

		if (type == Type.SoundFX)
		{
			Global.IsSoundOn = (state == State.ON);

			if(state == State.ON)
				SoundController.Instance.UnmuteSoundFX();
			else
				SoundController.Instance.MuteSoundFX();
		}

		if(type == Type.Tutorial)
			Global.IsTutorialEnabled = (state == State.ON);

		if(type == Type.Vibrate)
			Global.CanVibrate = (state == State.ON);
	}
}
