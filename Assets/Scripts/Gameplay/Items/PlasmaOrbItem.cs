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

			string size = gameObject.name.Substring(("Plasma Orb ").Length, gameObject.name.Length - ("Plasma Orb ").Length - ("(Clone)").Length);

			if(size == "PP")
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.OrbPP);
			else if(size == "P")
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.OrbP);
			else if(size == "M")
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.OrbM);
			else if(size == "G")
				SoundController.Instance.PlaySoundFX(SoundController.SoundFX.OrbG);
		}

		base.OnTriggerEnter2D (col);
	}
}
