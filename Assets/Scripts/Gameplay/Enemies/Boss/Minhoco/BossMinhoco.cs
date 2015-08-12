using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossMinhoco : MonoBehaviour 
{
	public enum Movement
	{
		TypeA,
		TypeB,
	}

	[Header("Builder")]
	public GameObject cabecaPrefab;
	public GameObject corpoPrefab;
	public GameObject raboPrefab;

	public int corpoQty = 5;
	public float pieceDistance = 0.45f;

	[Header("Movement")]
	public Movement movement = Movement.TypeA;
	public float oscilationVel = 5f;
	public float maxAngle = 60f;
	public float maxOscilationDistance = 1f;
	public int angleDifferenceBetweenPieces = 10;
	private float oscilationAngle;
	
	private Transform cabecaPiece;
	private GameObject cabecaBrilho;
	private EnemyLifeMinhoco enemyLife;

	[HideInInspector]
	public List<Transform> corpoPieces;

	#region get / set
	public bool IsBuild
	{
		get { return corpoPieces.Count > 0; }
	}
	#endregion

	void OnEnable()
	{
		EnemyLifeMinhocoPiece.OnPieceDied += PieceDied;
		EnemyLifeMinhoco.OnDied += HeadDied;
	}

	void OnDisable()
	{
		EnemyLifeMinhocoPiece.OnPieceDied -= PieceDied;
		EnemyLifeMinhoco.OnDied -= HeadDied;
	}

	// Use this for initialization
	void Start () 
	{
		oscilationAngle = 0;

		Build();

		enemyLife = cabecaPiece.GetComponent<EnemyLifeMinhoco>();
		cabecaBrilho = cabecaPiece.FindChild("Sprite").FindChild("Brilho").gameObject;
		cabecaBrilho.SetActive(false);
	}

	private void Build()
	{
		corpoPieces = new List<Transform>();

		GameObject go = Instantiate(cabecaPrefab) as GameObject;
		go.transform.parent = transform;
		go.transform.localPosition = Vector3.zero;

		cabecaPiece = go.transform;
		corpoPieces.Add(cabecaPiece);

		for(byte i = 0; i < corpoQty; i++)
		{
			go = Instantiate(corpoPrefab) as GameObject;
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(-pieceDistance * (i + 1), 0, 0);

			corpoPieces.Add(go.transform);
		}

		go = Instantiate(raboPrefab) as GameObject;
		go.transform.parent = transform;
		go.transform.localPosition = new Vector3(-pieceDistance * (corpoQty + 1), 0, 0);

		corpoPieces.Add(go.transform);

		cabecaPiece.GetComponent<EnemyLife>().spritesToWhite.Add(go.transform.FindChild("Sprite").GetComponent<SpriteRenderer>());
	}

	// Update is called once per frame
	void Update () 
	{
		if(!enemyLife.IsDead && corpoPieces.Count > 1)
			Animate();
	}

	private void Animate()
	{
		oscilationAngle += oscilationVel;
		
		cabecaPiece.localPosition = new Vector3(0, Mathf.Cos(oscilationAngle * Mathf.Deg2Rad), 0) * -maxOscilationDistance;
		
		//rotation cabeca
		Vector3 cabecaEuler = cabecaPiece.localEulerAngles;
		cabecaEuler.z = Mathf.Sin(oscilationAngle * Mathf.Deg2Rad) * maxAngle;
		cabecaPiece.localEulerAngles = cabecaEuler;
		
		for(byte i = 1; i < corpoPieces.Count; i++)
		{
			Transform t0 = corpoPieces[i - 1];
			Transform t1 = corpoPieces[i];
			
			if(movement == Movement.TypeA)
			{
				Vector3 pos = t1.localPosition;
				pos.y = Mathf.Cos((oscilationAngle - (oscilationVel * i * angleDifferenceBetweenPieces)) * Mathf.Deg2Rad) * -maxOscilationDistance;
				t1.localPosition = pos;
				
				Vector3 euler = t1.localEulerAngles;
				euler.z = Mathf.Atan2(t0.localPosition.y - t1.localPosition.y, t0.localPosition.x - t1.localPosition.x) * Mathf.Rad2Deg;
				t1.localEulerAngles = euler;
			}
			else
			{
				Vector3 pos;
				
				if(i == 1)
					pos = t0.localPosition;
				else
					pos = t0.localPosition + (-t0.right * pieceDistance);
				
				t1.localPosition = pos;
				
				Vector3 euler = t1.localEulerAngles;
				euler.z = Mathf.Sin((oscilationAngle - (oscilationVel * i * angleDifferenceBetweenPieces)) * Mathf.Deg2Rad) * maxAngle;
				t1.localEulerAngles = euler;
			}
		}
	}

	private void PieceDied(Transform piece)
	{
		corpoPieces.Remove(piece);

		for(byte i = 0; i < corpoPieces.Count; i++)
		{
			Transform t = corpoPieces[i];

			t.localPosition = new Vector3(-pieceDistance * i, 0, 0);
		}

		if(corpoPieces.Count == 2)
			cabecaBrilho.SetActive(true);
	}

	private void HeadDied(GameObject obj)
	{
		if(obj == cabecaPiece.gameObject && enemyLife.IsDead)
		{
			foreach(Transform t in corpoPieces)
			{
				foreach(Collider2D col in t.GetComponentsInChildren<Collider2D>())
					col.enabled = false;
			}

			StartCoroutine(Dead());
		}
	}

	private IEnumerator Dead()
	{
		EnemyLifeMinhoco[] minhocos = GameObject.FindObjectsOfType<EnemyLifeMinhoco>();
		
		bool lastOne = minhocos.Length == 1 || (minhocos[0].IsDead && minhocos[1].IsDead);
		
		if(lastOne)
		{
			Time.timeScale = 0.4f;
			ScreenFeedback.ShowBlank(enemyLife.deathTime, 0.5f);
		}
		
		CameraShake cameraShake = GetComponent<CameraShake>();
		cameraShake.Shake(enemyLife.deathTime);

		if(lastOne)
			yield return new WaitForSeconds(enemyLife.deathTime * 0.98f);

		Time.timeScale = 1f;

		if(lastOne)
		{
			GameController.BossDied();
			LevelDesign.BossLevelUp();
		}

		enabled = false;
	}
}
