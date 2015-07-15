using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemShop : MonoBehaviour 
{

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

	public List<ItemShop> itensDependency;

	void Start()
	{
		transform.FindChild ("Price").GetComponent<UILabel> ().text = string.Format("{0:0,0}", price);
		transform.FindChild ("Description").GetComponent<UILabel> ().text = description;

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
			Debug.Log("You don't have enough orbs, but you are cheating, who cares?");
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
	}
}
