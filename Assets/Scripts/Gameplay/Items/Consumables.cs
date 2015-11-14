using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Consumables : MonoBehaviour 
{
	private List<Item.Type> itensToUse;

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

	private bool IsInUse(Transform t)
	{
		Item.Type itemType = (Item.Type) System.Enum.Parse(typeof(Item.Type), t.name);

		if(itemType == Item.Type.Shield && GameController.IsShieldActive)
			return true;

		return false;
	}

	// Use this for initialization
	void OnEnable () 
	{
		GameController.OnResume += UseItens;

		foreach(Transform t in transform)
		{
			t.GetComponentInChildren<UILabel>().enabled = false;

			Item.Type itemType = (Item.Type) System.Enum.Parse(typeof(Item.Type), t.name);
			int consumable = GetQuantity(itemType);

			t.FindChild("count").GetComponent<UILabel>().text = consumable.ToString();

			if(IsInUse(t))
			{
				InUse(t);
			}
			else if(consumable > 0)
			{
				t.FindChild("status").GetComponent<UILabel>().text = Localization.Get("USED");
				t.GetComponent<UIButton>().enabled = true;
				t.GetComponent<TweenAlpha>().PlayReverse();
				t.GetComponent<TweenScale>().PlayReverse();
			}
			else
				OutOfStock(t);
		}
	}

	void OnDisable()
	{
		GameController.OnResume -= UseItens;
	}

	void Start()
	{
		itensToUse = new List<Item.Type>();
	}

	private void OutOfStock(Transform t)
	{
		t.GetComponent<UIButton>().enabled = false;
		t.GetComponent<TweenAlpha>().PlayForward();
		t.GetComponent<TweenScale>().PlayForward();
		t.GetComponentInChildren<UILabel>().enabled = true;
		t.FindChild("status").GetComponent<UILabel>().text = Localization.Get("OUT_OF_STOCK");
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

			itensToUse.Remove((Item.Type) System.Enum.Parse(typeof(Item.Type), UIButton.current.name));

			UIButton.current.transform.FindChild("count").GetComponent<UILabel>().text = consumable.ToString();
		}
		else
		{
			UIButton.current.GetComponent<TweenAlpha>().PlayForward();
			UIButton.current.GetComponent<TweenScale>().PlayForward();

			itensToUse.Add((Item.Type) System.Enum.Parse(typeof(Item.Type), UIButton.current.name));

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

	private void UseItens()
	{
		foreach(Item.Type itemType in itensToUse)
		{
			GameController.Instance.UseItem(itemType);

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
		}

		itensToUse.Clear();
	}
}
