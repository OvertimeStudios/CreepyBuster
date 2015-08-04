using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(BossLevelUpCondition))]
public class BossLevelUpConditionDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		label.text = label.text.Replace ("Element", "Boss");
		int element = int.Parse(label.text.Substring (label.text.Length - 1));
		label.text = label.text.Substring (0, label.text.Length - 1) + (element + 1);
		
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);
		
		// Draw label
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		// Calculate rects
		Rect killsRect = new Rect (position.x, position.y, position.width * 0.4f, position.height);
		Rect bossRect = new Rect (killsRect.x + killsRect.width + 5f, position.y, position.width * 0.5f, position.height);
		
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUIUtility.labelWidth = killsRect.width * 0.3f;
		EditorGUI.PropertyField (killsRect, property.FindPropertyRelative ("kills"), new GUIContent("Kills"));
		EditorGUIUtility.labelWidth = bossRect.width * 0.25f;
		EditorGUI.PropertyField (bossRect, property.FindPropertyRelative ("boss"), new GUIContent("Boss"));

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}
}
