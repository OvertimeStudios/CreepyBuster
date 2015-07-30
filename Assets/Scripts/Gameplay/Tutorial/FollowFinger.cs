
using UnityEngine;
using System.Collections;

/// <summary>
/// Selectable sprite that follows the mouse.
/// </summary>

[RequireComponent(typeof(UISprite))]
public class FollowFinger : MonoBehaviour
{
	static public FollowFinger instance;
	
	// Camera used to draw this cursor
	public Camera uiCamera;
	public UIWidget widget;

	Transform mTrans;
	UISprite mSprite;
	
	UIAtlas mAtlas;
	string mSpriteName;
	
	/// <summary>
	/// Keep an instance reference so this class can be easily found.
	/// </summary>
	
	void Awake () { instance = this; }
	void OnDestroy () { instance = null; }
	
	/// <summary>
	/// Cache the expected components and starting values.
	/// </summary>
	
	void Start ()
	{
		mTrans = transform;
		mSprite = GetComponentInChildren<UISprite>();
		
		if (uiCamera == null)
			uiCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		
		if (mSprite != null)
		{
			mAtlas = mSprite.atlas;
			mSpriteName = mSprite.spriteName;
			if (mSprite.depth < 100) mSprite.depth = 100;
		}

		Reposition ();
	}
	
	/// <summary>
	/// Reposition the widget.
	/// </summary>
	
	void Update ()
	{
		Reposition ();
	}
	
	private void Reposition()
	{
		Vector3 pos;
		if (Input.touches.Length > 0)//mobile
			pos = (Vector3)Input.GetTouch (0).position;
		else
			pos = Input.mousePosition;
		
		if (uiCamera != null)
		{
			Bounds widgetBounds = widget.CalculateBounds(uiCamera.transform);
			
			// Since the screen can be of different than expected size, we want to convert
			// mouse coordinates to view space, then convert that to world position.
			pos.x = Mathf.Clamp01(pos.x / Screen.width);
			pos.y = Mathf.Clamp01(pos.y / Screen.height);
			
			Vector3 cameraPoint = uiCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
			cameraPoint = uiCamera.transform.InverseTransformPoint(cameraPoint);
			
			Bounds cameraBounds = new Bounds(Vector3.zero, cameraPoint * 2f);
			
			pos = uiCamera.ViewportToWorldPoint(pos);
			pos = uiCamera.transform.InverseTransformPoint(pos);
			
			pos.x = Mathf.Clamp(pos.x, cameraBounds.min.x + widgetBounds.extents.x, cameraBounds.max.x - widgetBounds.extents.x);
			pos.y = Mathf.Clamp(pos.y, cameraBounds.min.y + widget.transform.localPosition.y, cameraBounds.max.y - widgetBounds.size.y - widget.transform.localPosition.y);
			
			mTrans.position = uiCamera.transform.TransformPoint(pos);
			
			// For pixel-perfect results
			#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
			if (uiCamera.isOrthoGraphic)
				#else
				if (uiCamera.orthographic)
					#endif
			{
				Vector3 lp = mTrans.localPosition;
				lp.x = Mathf.Round(lp.x);
				lp.y = Mathf.Round(lp.y);
				mTrans.localPosition = lp;
			}
		}
		else
		{
			// Simple calculation that assumes that the camera is of fixed size
			pos.x -= Screen.width * 0.5f;
			pos.y -= Screen.height * 0.5f;
			pos.x = Mathf.Round(pos.x);
			pos.y = Mathf.Round(pos.y);
			mTrans.localPosition = pos;
		}
	}
	
	/// <summary>
	/// Clear the cursor back to its original value.
	/// </summary>
	
	static public void Clear ()
	{
		if (instance != null && instance.mSprite != null)
			Set(instance.mAtlas, instance.mSpriteName);
	}
	
	/// <summary>
	/// Override the cursor with the specified sprite.
	/// </summary>
	
	static public void Set (UIAtlas atlas, string sprite)
	{
		if (instance != null && instance.mSprite)
		{
			instance.mSprite.atlas = atlas;
			instance.mSprite.spriteName = sprite;
			instance.mSprite.MakePixelPerfect();
			instance.Update();
		}
	}
}
