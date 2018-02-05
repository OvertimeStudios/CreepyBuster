using UnityEngine;
using System.Collections;

public class WorldSelect : MonoBehaviour 
{
	public enum World
	{
		World1,
		World2,
		World3,
		World4,
	}
	public World world;

	// Use this for initialization
	void Start () 
	{
	
	}

	public void Select()
	{
		Debug.Log("Selected: " + world);
		GameController.gameMode = GameController.GameMode.Story;
		MenuController.Instance.OpenPanel();
	}
}
