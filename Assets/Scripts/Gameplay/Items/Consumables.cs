using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Consumables : Singleton<Consumables> 
{
	private int GetQuantity(Item.Type itemType)
	{
		int consumable = 0;

		if(itemType == Item.Type.DeathRay)
			consumable = Global.DeathRayConsumable;
		if(itemType == Item.Type.Invencibility)
			consumable = Global.InvencibleConsumable;
		if(itemType == Item.Type.Frozen)
			consumable = Global.FrozenConsumable;
		if(itemType == Item.Type.LevelUp)
			consumable = Global.LevelUpConsumable;
		if(itemType == Item.Type.Shield)
			consumable = Global.ShieldConsumable;

		return consumable;
	}

	private bool CanUse(Transform t)
	{
		Item.Type itemType = (Item.Type) System.Enum.Parse(typeof(Item.Type), t.name);

		if(GameController.IsBossTime)
		{
			if(itemType == Item.Type.DeathRay || itemType == Item.Type.Frozen)
				return false;
		}

		if(itemType == Item.Type.LevelUp && LevelDesign.IsPlayerMaxLevel)
			return false;

		return true;
	}

	private bool IsInUse(Transform t)
	{
		Item.Type itemType = (Item.Type) System.Enum.Parse(typeof(Item.Type), t.name);

		if(itemType == Item.Type.Shield && GameController.IsShieldActive)
			return true;

		if(itemType == Item.Type.Invencibility && GameController.IsInvencibleByItem)
			return true;;

		return false;
	}

	// Use this for initialization
	void OnEnable () 
	{
		GameController.OnResume -= ConsumablesController.Instance.UseItens;
		GameController.OnResume += ConsumablesController.Instance.UseItens;

		foreach(Transform t in transform)
		{
			t.GetComponentInChildren<UILabel>().enabled = false;

			Item.Type itemType = (Item.Type) System.Enum.Parse(typeof(Item.Type), t.name);
			int consumable = GetQuantity(itemType);

			t.FindChild("count").GetComponent<UILabel>().text = consumable.ToString();

			if(consumable <= 0)
			{
				OutOfStock(t);
			}
			else if(IsInUse(t))
			{
				InUse(t);
			}
			else if(!CanUse(t))
			{
				LockItem(t);
			}
			else
			{
				t.FindChild("status").GetComponent<UILabel>().text = Localization.Get("USED");
				t.GetComponent<UIButton>().enabled = true;
				t.GetComponent<TweenAlpha>().PlayReverse();
				t.GetComponent<TweenScale>().PlayReverse();
			}
		}
	}

	void OnDisable()
	{
		
	}

	private void OutOfStock(Transform t)
	{
		t.GetComponent<UIButton>().enabled = false;
		t.GetComponent<TweenAlpha>().PlayForward();
		t.GetComponent<TweenScale>().PlayForward();
		t.GetComponentInChildren<UILabel>().enabled = true;
		t.FindChild("status").GetComponent<UILabel>().text = Localization.Get("OUT_OF_STOCK");
	}

	private void LockItem(Transform t)
	{
		t.GetComponent<UIButton>().enabled = false;
		t.GetComponent<TweenAlpha>().PlayForward();
		t.GetComponent<TweenScale>().PlayForward();
		t.GetComponentInChildren<UILabel>().enabled = true;
		t.FindChild("status").GetComponent<UILabel>().text = Localization.Get("CANT_USE");
	}

	private void InUse(Transform t)
	{
		t.GetComponent<UIButton>().enabled = false;
		t.GetComponent<TweenAlpha>().PlayForward();
		t.GetComponent<TweenScale>().PlayForward();
		t.GetComponentInChildren<UILabel>().enabled = true;
		t.FindChild("status").GetComponent<UILabel>().text = Localization.Get("IN_USE");
	}

	public void SelectPowerUp()
	{
		bool used = UIButton.current.GetComponentInChildren<UILabel>().enabled;
		UIButton.current.GetComponentInChildren<UILabel>().enabled = !used;

		Item.Type itemType = (Item.Type) System.Enum.Parse(typeof(Item.Type), UIButton.current.transform.name);
		int consumable = GetQuantity(itemType);

		if(used)
		{
			UIButton.current.GetComponent<TweenAlpha>().PlayReverse();
			UIButton.current.GetComponent<TweenScale>().PlayReverse();

			ConsumablesController.itensToUse.Remove((Item.Type) System.Enum.Parse(typeof(Item.Type), UIButton.current.name));

			UIButton.current.transform.FindChild("count").GetComponent<UILabel>().text = consumable.ToString();
		}
		else
		{
			UIButton.current.GetComponent<TweenAlpha>().PlayForward();
			UIButton.current.GetComponent<TweenScale>().PlayForward();

			ConsumablesController.itensToUse.Add((Item.Type) System.Enum.Parse(typeof(Item.Type), UIButton.current.name));

			UIButton.current.transform.FindChild("count").GetComponent<UILabel>().text = (consumable - 1).ToString();
		}
	}

	private void BackToNormal()
	{
		foreach(Transform t in transform)
		{
			t.GetComponent<UIButton>().enabled = true;
			t.GetComponent<TweenAlpha>().PlayReverse();
			t.GetComponent<TweenScale>().PlayReverse();
		}
	}


}
