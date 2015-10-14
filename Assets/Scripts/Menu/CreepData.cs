using UnityEngine;
using System.Collections;

public class CreepData : MonoBehaviour 
{
	public enum CreepType
	{
		Basic,
		Boomerang,
		Legion,
		Follower,
		ZigZag,
		Charger,
		Meteor,
		Illusion,
		Twins,
	}

	public CreepType type;
	public string nome;
	public float height;
	public float weight;
	public string description;
	private bool unlocked;

	private GameObject unlockedSprite;
	private GameObject lockedSprite;

	void Awake()
	{
		unlockedSprite = transform.FindChild("Unlocked").gameObject;
		lockedSprite = transform.FindChild("Locked").gameObject;
	}

	void OnEnable()
	{
		unlocked = Global.IsCreepUnlocked(type);

		unlockedSprite.SetActive(unlocked);
		lockedSprite.SetActive(!unlocked);
	}

	public void OpenBigData()
	{
		if(unlocked)
			Creepypedia.Instance.Open(type, nome, height, weight, description);
	}
}
