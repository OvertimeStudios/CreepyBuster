using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemShop : MonoBehaviour 
{
	#region Action
	public static event Action OnItemBought;
	#endregion

	#region get / set
	private int CurrentLevel
	{
		get
		{
			int level = 0;

			if(type == Type.Ray)
				level = Global.RayLevel;

			if(type == Type.Damage)
				level = Global.DamageLevel;

			if(type == Type.Range)
				level = Global.RangeLevel;

			return level;
		}
	}

	private bool IsMaxLevel
	{
		get	{ return CurrentLevel == price.Count; }
	}
	#endregion

	public enum Type
	{
		Ray,
		Range,
		Damage,
		PackOrbsSmall,
		PackOrbsMedium,
	}

	public Type type;
	public List<int> price;
	public int level = 0;

	private UILabel priceLabel;
	private UILabel levelLabel;
	private UILabel description;

	void OnEnable()
	{
		Global.OnPurchasesCleared += ClearPurchase;

		if(levelLabel != null)
			levelLabel.text = Localization.Get("LEVEL") + " " + ((IsMaxLevel) ? "MAX" : (CurrentLevel + 1).ToString());
	}

	void OnDisable()
	{
		Global.OnPurchasesCleared -= ClearPurchase;
	}

	void Start()
	{
		//verify if it is already purchased
		switch(type)
		{
			case Type.Ray:
				level = Global.RayLevel;
			break;
				
			case Type.Range:
				level = Global.RangeLevel;
			break;
				
			case Type.Damage:
				level = Global.DamageLevel;
			break;
		}

		description = transform.FindChild ("Description").GetComponent<UILabel> ();
		priceLabel = transform.FindChild ("Price").FindChild("Label").GetComponent<UILabel> ();
		levelLabel = transform.FindChild ("Level").GetComponent<UILabel> ();

		priceLabel.text = (IsMaxLevel) ? "-----" : string.Format("{0:0,0}", price[CurrentLevel]);
		levelLabel.text = Localization.Get("LEVEL") + " " + ((IsMaxLevel) ? "MAX" : (CurrentLevel + 1).ToString());
	}

	public void Purchase()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		Debug.Log("Trying to buy: " + type.ToString() + " for " + string.Format("{0:0,0}", price[CurrentLevel]) + " orbs.");

		if(IsMaxLevel) 
		{
			Debug.Log("Already Purchased");
			return;
		}

		#if INFINITY_ORBS
		if (Global.TotalOrbs >= price[CurrentLevel])
			Popup.ShowYesNo (string.Format(Localization.Get("BUY_ITEM"),description.text, string.Format ("{0:0,0}", price[CurrentLevel])), PurchaseAccepted, PurchaseDeclined);
		else
			Popup.ShowYesNo ("You may don't have enough orbs, but you are cheating, who cares? Wanna buy?", PurchaseAccepted, PurchaseDeclined);
		#else
		if (Global.TotalOrbs >= price[CurrentLevel])
			Popup.ShowYesNo (string.Format(Localization.Get("BUY_ITEM"),description.text, string.Format ("{0:0,0}", price[CurrentLevel])), PurchaseAccepted, PurchaseDeclined);
		else
			Popup.ShowOk (string.Format(Localization.Get("STORE_NOT_ENOUGH_ORBS"), string.Format ("{0:0,0}", (price[CurrentLevel] - Global.TotalOrbs))), null);
		#endif
	}

	public void PurchaseAccepted()
	{
		if(Global.TotalOrbs >= price[CurrentLevel])
		{
			Debug.Log("You spent " + price[CurrentLevel] + " on " + type.ToString());
			Global.OrbsSpent += price[CurrentLevel];
			Global.TotalOrbs -= price[CurrentLevel];

			UnlockProperty();
		}

		#if INFINITY_ORBS
		Popup.ShowOk("Bad, bad cheating boy. Here is your 'purchase'. Humpf.");
		UnlockProperty();
		#endif
	}

	public void PurchaseDeclined()
	{

	}

	private void UnlockProperty()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.ShopBuy);

		level++;

		switch(type)
		{
			case Type.Ray:
				Global.RayLevel++;
				break;
				
			case Type.Range:
				Global.RangeLevel++;
				break;
				
			case Type.Damage:
				Global.DamageLevel++;
				break;
		}

		priceLabel.text = (IsMaxLevel) ? "-----" : string.Format("{0:0,0}", price[CurrentLevel]);
		levelLabel.text = Localization.Get("LEVEL") + " " + ((IsMaxLevel) ? "MAX" : (CurrentLevel + 1).ToString());

		if (OnItemBought != null)
			OnItemBought ();
	}

	private void ClearPurchase()
	{
		level = 0;

		priceLabel.text = (IsMaxLevel) ? "-----" : string.Format("{0:0,0}", price[CurrentLevel]);
		levelLabel.text = Localization.Get("LEVEL") + " " + ((IsMaxLevel) ? "MAX" : (CurrentLevel + 1).ToString());
	}
}
