using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Creepies : MonoBehaviour 
{
	public List<CreepyStats> creepies;
	public GameObject creepyStatsPrefab;

	// Use this for initialization
	void Start () 
	{
		Transform grid = transform.FindChild ("Scroll View").FindChild("Grid");
		
		foreach(CreepyStats creepy in creepies)
		{
			Transform newCreepy = (Instantiate(creepyStatsPrefab) as GameObject).transform;
			newCreepy.parent = grid;
			newCreepy.localScale = Vector3.one;
			
			newCreepy.FindChild("Height").FindChild("Value").GetComponent<UILabel>().text = (creepy.hidden && !creepy.unlocked) ? "???????" : creepy.height;
			newCreepy.FindChild("Weight").FindChild("Value").GetComponent<UILabel>().text = (creepy.hidden && !creepy.unlocked) ? " ??????" : creepy.weight;
			newCreepy.FindChild("Description").FindChild("Value").GetComponent<UILabel>().text = (creepy.hidden && !creepy.unlocked) ? "?????? \n ??????" : Localization.Get(creepy.description);
			newCreepy.FindChild("Name").GetComponent<UILabel>().text = (creepy.hidden && !creepy.unlocked) ? " ??????" : creepy.name;
			newCreepy.FindChild("Sprite").GetComponent<UISprite>().spriteName = creepy.imageName;
			newCreepy.FindChild("Sprite").GetComponent<UISprite>().MakePixelPerfect();


			if(!creepy.unlocked)
				newCreepy.FindChild("Sprite").GetComponent<UISprite>().color = Color.black;
		}
		
		grid.GetComponent<UIGrid> ().Reposition ();
	}
}

[System.Serializable]
public class CreepyStats
{
	public string imageName;
	public string name;
	public string height;
	public string weight;
	public string description;
	public bool unlocked;
	public bool hidden;
}