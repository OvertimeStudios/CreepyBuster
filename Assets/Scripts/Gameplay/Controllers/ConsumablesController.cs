using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ConsumablesController : Singleton<ConsumablesController> 
{
	#region Action
	public static Action OnAnyItemUsed;
	public static Action OnAllItensUsed;
	#endregion

	public static List<Item.Type> itensToUse = new List<Item.Type>();

	public static bool IsUsingConsumables
	{
		get { return itensToUse.Count > 0; }
	}

	public GameObject deathRaySpinPrefab;
	public GameObject InvincibilitySpinPrefab;
	public GameObject frozenSpinPrefab;
	public GameObject levelUpSpinPrefab;
	public GameObject shieldSpinPrefab;
	public GameObject particlePrefab;
	public GameObject blackScreen;

	public void UseItens()
	{
		StartCoroutine(UseItensWithDelay());

		if(itensToUse.Count > 0)
		{
			blackScreen.SetActive(true);

			if(OnAnyItemUsed != null)
				OnAnyItemUsed();
		}
	}

	private IEnumerator UseItensWithDelay()
	{
		foreach(Item.Type itemType in itensToUse)
		{
			//GameController.Instance.UseItem(itemType);
			GameController.Instance.SpawnItem(itemType);

			if(itemType == Item.Type.DeathRay)
				Global.DeathRayConsumable--;
			if(itemType == Item.Type.Invencibility)
				Global.InvencibleConsumable--;
			if(itemType == Item.Type.Frozen)
				Global.FrozenConsumable--;
			if(itemType == Item.Type.LevelUp)
				Global.LevelUpConsumable--;
			if(itemType == Item.Type.Shield)
				Global.ShieldConsumable--;

			yield return new WaitForSeconds(0.3f);
		}
	}

	public static void ItemUsed(Item.Type itemUsed)
	{
		itensToUse.Remove(itemUsed);

		if(itensToUse.Count == 0)
		{
			Instance.blackScreen.SetActive(false);

			if(OnAllItensUsed != null)
				OnAllItensUsed();
		}
	}
}
