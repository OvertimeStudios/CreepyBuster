using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ItemShop : MonoBehaviour 
{
	#region Action
	public static event Action OnItemBought;
	public static event Action<ItemShop> OnItemLoaded;
	#endregion

	public enum Type
	{
		Ray3,
		Ray4,
		Ray5,
		SuperRange,
		MegaRange,
		MasterRange,
		SuperDamage,
		MegaDamage,
		UltraDamage,
	}

	public Type type;
	public int price;
	public string description;

	public bool purchased;
	public bool hidePricesIfDisabled = false;

	private int itensLoaded = 0;
	public List<ItemShop> itensDependency;

	void OnEnable()
	{
		ItemShop.OnItemBought += VerifyDependancies;
		ItemShop.OnItemLoaded += HandleOnItemLoaded;
		Global.OnPurchasesCleared += ClearPurchase;
	}

	void OnDisable()
	{
		ItemShop.OnItemBought -= VerifyDependancies;
		ItemShop.OnItemLoaded -= HandleOnItemLoaded;
		Global.OnPurchasesCleared -= ClearPurchase;
	}

	void Start()
	{
		//verify if it is already purchased
		switch(type)
		{
			case Type.Ray3:
				purchased = Global.Ray3Purchased;
			break;
				
			case Type.Ray4:
				purchased = Global.Ray4Purchased;
			break;
				
			case Type.Ray5:
				purchased = Global.Ray5Purchased;
			break;
				
			case Type.SuperRange:
				purchased = Global.SuperRangePurchased;
			break;
				
			case Type.MegaRange:
				purchased = Global.MegaRangePurchased;
			break;
				
			case Type.MasterRange:
				purchased = Global.MasterRangePurchased;
			break;
				
			case Type.SuperDamage:
				purchased = Global.SuperDamagePurchased;
			break;
				
			case Type.MegaDamage:
				purchased = Global.MegaDamagePurchased;
			break;
				
			case Type.UltraDamage:
				purchased = Global.UltraDamagePurchased;
			break;
		}

		Debug.Log (type.ToString() + " - " + purchased);

		transform.FindChild ("Price").GetComponent<UILabel> ().text = (purchased) ? "SOLD OUT" : string.Format("{0:0,0}", price);
		transform.FindChild ("Description").GetComponent<UILabel> ().text = description;

		if (OnItemLoaded != null)
			OnItemLoaded (this);
	}

	private void VerifyDependancies()
	{
		foreach(UIButton button in GetComponents<UIButton>())
			button.isEnabled = true;

		transform.FindChild ("Price").GetComponent<UILabel> ().enabled = true;

		foreach(ItemShop itemShop in itensDependency)
		{
			if(!itemShop.purchased)
			{
				foreach(UIButton button in GetComponents<UIButton>())
					button.isEnabled = false;
				
				if(hidePricesIfDisabled)
					transform.FindChild ("Price").GetComponent<UILabel> ().enabled = false;
				
				break;
			}
		}
	}

	public void Purchase()
	{
		Debug.Log("Trying to buy: " + type.ToString() + " for " + price + " orbs.");
		if(Global.TotalOrbs >= price)
		{
			Debug.Log("You spent " + price + " on " + type.ToString());
			Global.TotalOrbs -= price;

			UnlockProperty();
		}
		else
		{
			#if INFINITY_ORBS
			Debug.Log("You may don't have enough orbs, but you are cheating, who cares?");
			UnlockProperty();
			#else
			Debug.Log("You don't have enough money. You must have " + (price - Global.TotalOrbs) + " more orbs to but this item.");
			#endif
		}
	}

	private void UnlockProperty()
	{
		switch(type)
		{
			case Type.Ray3:
				Global.Ray3Purchased = true;
				break;
				
			case Type.Ray4:
				Global.Ray4Purchased = true;
				break;
				
			case Type.Ray5:
				Global.Ray5Purchased = true;
				break;
				
			case Type.SuperRange:
				Global.SuperRangePurchased = true;
				break;
				
			case Type.MegaRange:
				Global.MegaRangePurchased = true;
				break;
				
			case Type.MasterRange:
				Global.MasterRangePurchased = true;
				break;
				
			case Type.SuperDamage:
				Global.SuperDamagePurchased = true;
				break;
				
			case Type.MegaDamage:
				Global.MegaDamagePurchased = true;
				break;
				
			case Type.UltraDamage:
				Global.UltraDamagePurchased = true;
				break;
		}

		purchased = true;
		transform.FindChild ("Price").GetComponent<UILabel> ().text = "SOLD OUT";

		if (OnItemBought != null)
			OnItemBought ();
	}

	private void ClearPurchase()
	{
		purchased = false;

		transform.FindChild ("Price").GetComponent<UILabel> ().text = string.Format("{0:0,0}", price);
		transform.FindChild ("Description").GetComponent<UILabel> ().text = description;
		
		VerifyDependancies ();
	}

	private void HandleOnItemLoaded(ItemShop itemShop)
	{
		foreach(ItemShop item in itensDependency)
		{
			if(item == itemShop)
			{
				itensLoaded++;

				if(itensLoaded >= itensDependency.Count)
				{
					VerifyDependancies();
					itensLoaded = 0;
				}

			}
		}
	}
}
