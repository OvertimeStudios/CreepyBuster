using UnityEngine;
using System.Collections;

public class ItemShopConsumable : MonoBehaviour 
{
	public Item.Type itemType;
	public int price;
	public int maxStock;

	private UILabel stockLabel;
	private UILabel priceLabel;

	#region get / set
	public bool IsMaxStock
	{
		get
		{
			return CurrentStock == maxStock;
		}
	}

	public int CurrentStock
	{
		get
		{
			if(itemType == Item.Type.DeathRay)
				return Global.DeathRayConsumable;

			if(itemType == Item.Type.LevelUp)
				return Global.LevelUpConsumable;

			if(itemType == Item.Type.Invencibility)
				return Global.InvencibleConsumable;

			if(itemType == Item.Type.Frozen)
				return Global.FrozenConsumable;

			if(itemType == Item.Type.Shield)
				return Global.ShieldConsumable;

			return 0;
		}
	}
	#endregion

	void OnEnable()
	{
		if(stockLabel != null)
		{
			stockLabel.text = Localization.Get("STOCK") + ": " + ((IsMaxStock) ? "MAX" : CurrentStock.ToString());
			priceLabel.text = (IsMaxStock) ? "-----" : string.Format("{0:0,0}", price);
		}
	}

	void Start()
	{
		stockLabel = transform.FindChild("Stock").GetComponent<UILabel>();
		priceLabel = transform.FindChild ("Price").FindChild("Label").GetComponent<UILabel> ();

		stockLabel.text = Localization.Get("STOCK") + ": " + ((IsMaxStock) ? "MAX" : CurrentStock.ToString());
		priceLabel.text = (IsMaxStock) ? "-----" : string.Format("{0:0,0}", price);
	}

	public void Purchase()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);
		
		Debug.Log("Trying to buy: " + itemType.ToString() + " for " + string.Format("{0:0,0}", price) + " orbs.");
		
		if(IsMaxStock) 
		{
			Popup.ShowOk(Localization.Get("MAX_STOCK"));
			return;
		}
		
		#if INFINITY_ORBS
		if (Global.TotalOrbs >= price)
			Popup.ShowYesNo (string.Format(Localization.Get("BUY_ITEM"),description.text, string.Format ("{0:0,0}", price[CurrentLevel])), PurchaseAccepted, PurchaseDeclined);
		else
			Popup.ShowYesNo ("You may don't have enough orbs, but you are cheating, who cares? Wanna buy?", PurchaseAccepted, PurchaseDeclined);
		#else
		if (Global.TotalOrbs >= price)
			Popup.ShowYesNo (string.Format(Localization.Get("BUY_ITEM"),Localization.Get(itemType.ToString()), string.Format ("{0:0,0}", price)), PurchaseAccepted, PurchaseDeclined);
		else
			Popup.ShowOk (string.Format(Localization.Get("STORE_NOT_ENOUGH_ORBS"), string.Format ("{0:0,0}", (price - Global.TotalOrbs))), null);
		#endif
	}

	private void PurchaseAccepted()
	{
		Global.OrbsSpent += price;
		Global.TotalOrbs -= price;

		UnlockProperty();
	}

	private void PurchaseDeclined()
	{
		
	}

	private void UnlockProperty()
	{
		if(itemType == Item.Type.DeathRay)
			Global.DeathRayConsumable++;
		
		if(itemType == Item.Type.LevelUp)
			Global.LevelUpConsumable++;
		
		if(itemType == Item.Type.Invencibility)
			Global.InvencibleConsumable++;
		
		if(itemType == Item.Type.Frozen)
			Global.FrozenConsumable++;

		if(itemType == Item.Type.Shield)
			Global.ShieldConsumable++;

		stockLabel.text = Localization.Get("STOCK") + ": " + ((IsMaxStock) ? "MAX" : CurrentStock.ToString());
		priceLabel.text = (IsMaxStock) ? "-----" : string.Format("{0:0,0}", price);
	}
}
