using UnityEngine;
using System.Collections;

public class OnOffBehaviour : MonoBehaviour 
{
	public enum Type
	{
		Music,
		SoundFX,
		Tutorial,
	}

	private enum State
	{
		OFF,
		ON,
	}

	private Transform onButton;
	private Transform offButton;
	private TweenPosition selection;
	public Type type;

	private State state;

	// Use this for initialization
	void Start () 
	{
		onButton = transform.FindChild ("On");
		offButton = transform.FindChild ("Off");
		selection = transform.FindChild ("Selection").GetComponent<TweenPosition>();

		if (type == Type.Music)
			state = (State)Global.IsMusicOn.GetHashCode();

		else if (type == Type.SoundFX)
			state = (State)Global.IsSoundOn.GetHashCode();

		else if(type == Type.Tutorial)
			state = (State)Global.IsTutorialEnabled.GetHashCode();

		//turn selection to 'off' or 
		if (state == State.OFF)
				selection.transform.localPosition = offButton.localPosition;

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

	private void ToggleOnOff ()
	{
		if (type == Type.Music)
			Global.IsMusicOn = (state == State.ON);

		if (type == Type.SoundFX)
			Global.IsSoundOn = (state == State.ON);

		if(type == Type.Tutorial)
			Global.IsTutorialEnabled = (state == State.ON);
	}
}
