using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FollowFinger))]
public class FollowFingerText : MonoBehaviour 
{
    private static FollowFingerText m_Instance;
    public static FollowFingerText Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = GameObject.FindObjectOfType<FollowFingerText>();
            
            return m_Instance;
        }
    }

    private static FollowFinger m_FollowFinger;
    private static FollowFinger FollowFinger
    {
        get
        {
            if (m_FollowFinger == null)
                m_FollowFinger = m_Instance.GetComponent<FollowFinger>();

            return m_FollowFinger;
        }
    }

    private static UILabel Label
    {
        get
        {
            return FollowFinger.widget as UILabel;
        }
    }

	// Use this for initialization
	void Awake () 
    {
        if(m_Instance != null && m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;

        Hide();
	}

    public static void Show(string text)
    {
        Show(text, 0);
    }

    public static void Show(string text, float hideTime)
    {
        m_Instance.gameObject.SetActive(true);

        Label.text = text;

        if(hideTime > 0)
        {
            m_Instance.StartCoroutine(m_Instance.HideWithTime(hideTime));
        }
    }

    private IEnumerator HideWithTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Hide();
    }

    public static void Hide()
    {
        m_Instance.gameObject.SetActive(false);
    }
}
