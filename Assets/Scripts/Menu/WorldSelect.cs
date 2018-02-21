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

    private void OnDestroy()
    {
        MenuController.OnPanelClosing -= UpdateHUD;
    }

    // Use this for initialization
    void Start () 
	{
        MenuController.OnPanelClosing += UpdateHUD;
        UpdateHUD();
	}

    private void UpdateHUD()
    {
        Transform levels = transform.Find("Levels");

        int levelsCompleted = Global.GetWorldLevelCompletion((int)world);
        if(levelsCompleted < 0)
        {
            if(world == World.World1 || (world != World.World1 && Global.GetWorldLevelCompletion((int)world - 1) == 5))
                levels.GetChild(0).GetComponent<UISprite>().color = Color.cyan;
        }
        else
        {
            for (byte i = 0; i < levels.childCount; i++)
            {
                if(i <= levelsCompleted + 1)
                {
                    levels.GetChild(i).GetComponent<UISprite>().color = (i <= levelsCompleted) ? Color.green : Color.red;
                }
            }

            if (levelsCompleted == 4)
            {
                transform.Find("Boss").GetComponent<UISprite>().color = Color.red;
            }
            if(levelsCompleted == 5)
            {
                transform.Find("Boss").GetComponent<UISprite>().color = Color.green;
            }

        }
    }

	public void Select()
	{
        GameController.world = (int)world;
		GameController.gameMode = GameController.GameMode.Story;
		MenuController.Instance.OpenPanel();
	}
}
