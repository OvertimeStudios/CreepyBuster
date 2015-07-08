using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(PlayerLevelUpCondition))]
public class PlayerLevelUpConditionDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		label.text = label.text.Replace ("Element", "Level");
		int element = int.Parse(label.text.Substring (label.text.Length - 1));
		label.text = label.text.Substring (0, label.text.Length - 1) + (element + 1);

		EditorGUIUtility.labelWidth = position.width * 0.3f;

		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);
		
		// Draw label
		position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		// Calculate rects
		Rect streakRect = new Rect (position.x, position.y, 70, position.height);
		Rect raysRect = new Rect (position.x + 75, position.y, 80, position.height);
		Rect colorRect = new Rect (position.x + 160, position.y, 100, position.height);
		
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUIUtility.labelWidth = 40f;
		EditorGUI.PropertyField (streakRect, property.FindPropertyRelative ("killStreak"), new GUIContent("streak"));
		EditorGUIUtility.labelWidth = 60f;
		EditorGUI.PropertyField (raysRect, property.FindPropertyRelative ("maxRays"), new GUIContent("Max Rays"));
		EditorGUIUtility.labelWidth = 40;
		EditorGUI.PropertyField (colorRect, property.FindPropertyRelative ("color"), new GUIContent("Color"));
		
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}
}
