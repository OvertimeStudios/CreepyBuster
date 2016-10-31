using UnityEngine;
using System.Collections;
using UnityEditor;

//[CustomPropertyDrawer(typeof(ItemPercent))]
public class ItemPercentDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		label.text = "";
		
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);
		
		// Draw label
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		// Calculate rects
		Rect pointsRect = new Rect (position.x, position.y, position.width * 0.6f, position.height);
		Rect streakRect = new Rect (pointsRect.x + pointsRect.width, position.y, position.width * 0.3f, position.height);
		
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUIUtility.labelWidth = pointsRect.width * 0.5f;
		EditorGUI.PropertyField (pointsRect, property.FindPropertyRelative ("item"), GUIContent.none);
		EditorGUIUtility.labelWidth = streakRect.width * 0.3f;
		EditorGUI.PropertyField (streakRect, property.FindPropertyRelative ("percent"), new GUIContent("%"));
		
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}
}

