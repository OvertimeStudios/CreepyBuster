using UnityEngine;
using System.Collections;

public class Creepypedia : MonoBehaviour 
{
	#region singleton
	private static Creepypedia instance;
	public static Creepypedia Instance
	{
		get
		{
			if(instance == null)
				instance = GameObject.FindObjectOfType<Creepypedia>();

			return instance;
		}
	}
	#endregion

	public GameObject opened;
	public GameObject closed;
	public UILabel nome;
	public UILabel altura;
	public UILabel peso;
	public UILabel descricao;

	public Transform creeps;

	public void Open(CreepData.CreepType type, string name, float height, float weight, string description)
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		//deactive all sprite
		foreach(Transform child in creeps)
			child.gameObject.SetActive(false);

		//active correct sprite
		creeps.FindChild(type.ToString()).gameObject.SetActive(true);

		nome.text = name;
		altura.text = height.ToString();
		peso.text = weight.ToString();
		descricao.text = description;

		opened.SetActive(true);
		closed.SetActive(false);
	}

	public void Close()
	{
		SoundController.Instance.PlaySoundFX(SoundController.SoundFX.Click);

		opened.SetActive(false);
		closed.SetActive(true);
	}
}
