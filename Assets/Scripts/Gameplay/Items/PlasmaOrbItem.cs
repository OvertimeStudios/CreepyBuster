using UnityEngine;
using System.Collections;

public class PlasmaOrbItem : Item
{
	public int orbs;
	public GameObject label;
	public Color textColor;

	protected override void OnTriggerEnter2D (Collider2D col)
	{
		if(col.gameObject.tag == "Player" && !col.isTrigger)
		{
			GameObject obj = NGUITools.AddChild(NGUITools.GetRoot(UICamera.mainCamera.gameObject), label);
			
			Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
			pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
			
			obj.transform.position = pos;
			obj.GetComponent<UILabel>().color = textColor;
			obj.GetComponent<UILabel>().text = "+" + orbs + " orb" + ((orbs > 1) ? "s" : "");
		}

		base.OnTriggerEnter2D (col);
	}
}
