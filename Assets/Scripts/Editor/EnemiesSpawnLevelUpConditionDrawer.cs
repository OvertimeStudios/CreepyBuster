using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnemiesSpawnLevelUpCondition))]
public class EnemiesTypesSpawnUpConditionDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		label.text = label.text.Replace ("Element", "Level");
		int element = int.Parse(label.text.Substring (label.text.Length - 1));
		label.text = label.text.Substring (0, label.text.Length - 1) + (element + 1);

		EditorGUIUtility.labelWidth = position.width * 0.2f;
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);
		
		// Draw label
		Rect contentPosition = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		#region points
		Rect pointsRect = new Rect (contentPosition.x, contentPosition.y, contentPosition.width * 0.25f, contentPosition.height);
		EditorGUIUtility.labelWidth = pointsRect.width * 0.6f;
		EditorGUI.PropertyField (pointsRect, property.FindPropertyRelative ("points"), new GUIContent("points"));
		#endregion
		
		#region streak
		Rect streakRect = new Rect (pointsRect.x + pointsRect.width, contentPosition.y, contentPosition.width * 0.3f, contentPosition.height);
		EditorGUIUtility.labelWidth = streakRect.width * 0.6f;
		EditorGUI.PropertyField (streakRect, property.FindPropertyRelative ("killStreak"), new GUIContent("/streak"));
		#endregion

		#region quantity
		Rect qtyRect = new Rect (streakRect.x + streakRect.width + 3, contentPosition.y, contentPosition.width * 0.2f, contentPosition.height);
		EditorGUIUtility.labelWidth = qtyRect.width * 0.4f;
		EditorGUI.PropertyField (qtyRect, property.FindPropertyRelative ("quantity"), new GUIContent("qty"));
		#endregion

		#region time
		Rect timeRect = new Rect (qtyRect.x + qtyRect.width + 3, contentPosition.y, contentPosition.width * 0.2f, contentPosition.height);
		EditorGUIUtility.labelWidth = timeRect.width * 0.5f;
		EditorGUI.PropertyField (timeRect, property.FindPropertyRelative ("time"), new GUIContent("time"));
		#endregion

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}
}
