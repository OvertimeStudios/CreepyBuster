using UnityEngine;
using System.Collections;

public class ForwardMinhoco : MonoBehaviour 
{
	private enum Direction
	{
		Left,
		Down,
		Right,
		Up,
	}

	public float minVel;
	public float maxVel;

	private SpriteRenderer tailSprite;
	private Rigidbody2D myRigidbody;
	private Direction dir;
	private BossMinhoco bossMinhoco;

	#region get / set
	public float CurrentVel
	{
		get 
		{ 
			float difference = maxVel - minVel;
			float percent = (float)bossMinhoco.corpoQty - ((float)bossMinhoco.corpoPieces.Count - 2);
			percent /= (float)bossMinhoco.corpoQty;

			return minVel + (difference * percent); 
		}
	}
	#endregion

	void OnEnable()
	{
		EnemyLifeMinhocoPiece.OnPieceDied += PieceDied;
	}
	
	void OnDisable()
	{
		EnemyLifeMinhocoPiece.OnPieceDied -= PieceDied;
	}

	// Use this for initialization
	void Start () 
	{
		StartCoroutine(WaitForBossMinhoco());
	}

	private IEnumerator WaitForBossMinhoco()
	{
		bossMinhoco = GetComponent<BossMinhoco>();

		while(!bossMinhoco.IsBuild)
			yield return null;

		myRigidbody = GetComponent<Rigidbody2D>();
		tailSprite = bossMinhoco.corpoPieces[bossMinhoco.corpoPieces.Count - 1].GetComponentInChildren<SpriteRenderer>();

		Reposition();
	}
	
	// Update is called once per frame
	void Update () 
	{
		myRigidbody.velocity = transform.right * CurrentVel;

		CheckOutOfScreen();
	}

	private void Reposition()
	{
		int rnd = (int)(Random.Range(0, 4));
		float angle = rnd * (Mathf.PI / 2);

		dir = (Direction)rnd;

		float posx = (angle % Mathf.PI / 2 == 0) ? 0.5f + Mathf.Cos(angle) * 0.8f : Random.Range(0.2f, 0.8f);
		float posy = (angle % Mathf.PI / 2 == 0) ? Random.Range(0.2f, 0.8f) : 0.5f + Mathf.Sin(angle) * 0.8f ;
		
		Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(posx, posy, 0));
		pos.z = 0;
		
		transform.position = pos;
		
		//get some point on the other side to go towards
		angle = angle + Mathf.PI;
		
		posx = (angle % Mathf.PI / 2 == 0) ? 0.5f + Mathf.Cos(angle) * 0.8f : Random.Range(0.2f, 0.8f);
		posy = (angle % Mathf.PI / 2 == 0) ? Random.Range(0.2f, 0.8f) : 0.5f + Mathf.Sin(angle) * 0.8f ;
		
		pos = Camera.main.WorldToViewportPoint(pos);
		
		angle = Mathf.Atan2(posy - pos.y, posx - pos.x);
		
		transform.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);
	}

	private void CheckOutOfScreen()
	{
		if(tailSprite == null) return;

		Bounds tailBounds = tailSprite.bounds;
		
		if(dir == Direction.Down || dir == Direction.Left)
		{
			Vector3 max = Camera.main.WorldToViewportPoint(tailBounds.max);
			
			if(max.x < 0 || max.y < 0)
				Reposition();
		}
		else
		{
			Vector3 min = Camera.main.WorldToViewportPoint(tailBounds.min);
			
			if(min.x > 1 || min.y > 1)
				Reposition();
		}
	}

	private void PieceDied(Transform piece)
	{
		myRigidbody.velocity = transform.right * CurrentVel;
	}
}
