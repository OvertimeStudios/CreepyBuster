using UnityEngine;
using System.Collections;

public class ItemSpin : MonoBehaviour 
{
	public Item.Type type;
	public float spinVel = 200f;
	public float approachVel = -1.5f;
	public Color particleColor;

	private Transform myTransform;
	private Transform iconTransform;

	void Start()
	{
		myTransform = transform;
		iconTransform = transform.FindChild("icon");
	}

	void FixedUpdate()
	{
		myTransform.Rotate(0, 0, spinVel * Time.deltaTime);
		iconTransform.Rotate(0, 0, -spinVel * Time.deltaTime);

		float angle = -Mathf.Atan2(iconTransform.position.y - myTransform.position.y, iconTransform.position.x - myTransform.position.x);
		iconTransform.Translate(approachVel * Mathf.Cos(angle) * Time.deltaTime, approachVel * Mathf.Cos(angle) * Time.deltaTime, 0, Space.World);

		if(Vector3.Distance(Vector3.zero, iconTransform.localPosition) < 0.3f)
		{
			GameController.Instance.UseItem(type);
			(Instantiate(ConsumablesController.Instance.particlePrefab, myTransform.position, Quaternion.identity) as GameObject).GetComponent<ParticleSystem>().startColor = particleColor;
			Destroy(gameObject);
		}
	}

}
