using UnityEngine;
using System;
using System.Collections;

public class TouchPressure : Singleton<TouchPressure> 
{
	#region Action
	public static Action On3DTouchStart;
	public static Action On3DTouchEnd;
	#endregion

	public float energyConsumption;
	public float bonusDamage;

	private static float currentPressure;
	private static float maxPressure;

	/// <summary>
	/// Internal control
	/// </summary>
	private static bool is3DTouchActive;

	//simulation
	private static bool IsKeyDown;

	#region get/set
	public static float PressurePercent
	{
		get { return 
			(Input.touchPressureSupported) ? (currentPressure - 1f) / (maxPressure - 1f) : 
			(IsKeyDown) ? 1f : 0f; } 
	}

	public static bool IsPressureSupported
	{
		get { return Input.touchSupported; }
	}

	public static float TouchDamage
	{
		get { return PressurePercent * Instance.bonusDamage; }
	}

	public static bool IsUsing3DTouch
	{
		get { return PressurePercent > 0; }
	}

	public static bool IsUsingBonusDamage
	{
		get { return is3DTouchActive; }
	}
	#endregion

	void Start () 
	{
		is3DTouchActive = false;
		//if(!Input.touchPressureSupported)
			//this.enabled = false;
	}

	// Update is called once per frame
	void Update () 
	{
		float lastPressure = currentPressure;

		if(Input.touchSupported)
		{
			Touch touch = Input.GetTouch(0);
			currentPressure = touch.pressure;
			maxPressure = touch.maximumPossiblePressure;
		}
		else
		{
			if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftCommand))
				IsKeyDown = true;

			if(Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.LeftCommand))
				IsKeyDown = false;
		}

		if(PressurePercent > 0)
		{
			if(GameController.EnergyCount > LevelDesign.CurrentPlayerLevelUnlockStreak)
			{
				GameController.EnergyCount = Mathf.Max(GameController.EnergyCount - (energyConsumption * PressurePercent * Time.deltaTime), LevelDesign.CurrentPlayerLevelUnlockStreak);

				if(!is3DTouchActive)
				{
					is3DTouchActive = true;

					ScreenFeedback.Show3DTouch();

					if(On3DTouchStart != null)
						On3DTouchStart();
				}
			}
			else
			{
				if(is3DTouchActive)
				{
					is3DTouchActive = false;

					ScreenFeedback.Hide3DTouch();

					if(On3DTouchEnd != null)
						On3DTouchEnd();
				}
			}
		}
		else
		{
			if(is3DTouchActive)
			{
				is3DTouchActive = false;

				ScreenFeedback.Hide3DTouch();

				if(On3DTouchEnd != null)
					On3DTouchEnd();
			}
		}
	}
}
