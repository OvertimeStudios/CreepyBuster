using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4 : MonoBehaviour 
{
    [SerializeField]
    private GameObject[] m_Pieces;

    [SerializeField]
    private Transform m_PiecesPivot;

	// Use this for initialization
	void Start () 
    {
        for (byte i = 0; i < m_Pieces.Length; i++)
        {
            GameObject piece = m_Pieces[i];

            piece.SetActive(false);
        }

        StartCoroutine(GoToCenterScreen());
	}

    private IEnumerator GoToCenterScreen()
    {
        Vector3 to = new Vector3(0.5f, 0.5f, 0);
        to = Camera.main.ViewportToWorldPoint(to);
        to.z = transform.position.z;

        Vector3 from = transform.position;

        float counter = 0;
        float totalTime = 3f;
        while(counter < totalTime)
        {
            counter += Time.deltaTime;

            transform.position = Vector3.Lerp(from, to, counter / totalTime);

            yield return null;
        }

        StartCoroutine(ShowPieces());
    }

    private IEnumerator ShowPieces()
    {
        for (byte i = 0; i < m_Pieces.Length; i++)
        {
            GameObject piece = m_Pieces[i];

            Vector3 to = piece.transform.position;

            //position out of screen
            Vector3 from = to * 10f;

            piece.transform.position = from;

            piece.SetActive(true);

            float counter = 0;
            float totalTime = 1f;

            while(counter < totalTime)
            {
                counter += Time.deltaTime;

                piece.transform.position = Vector3.Lerp(from, to, counter / totalTime);

                yield return null;
            }
        }
    }
}
