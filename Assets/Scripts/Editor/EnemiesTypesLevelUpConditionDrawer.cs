using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnemiesTypesLevelUpCondition))]
public class EnemiesTypesLevelUpConditionDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		label.text = label.text.Replace ("Element", "Level");
		int element = int.Parse(label.text.Substring (label.text.Length - 1));
		label.text = label.text.Substring (0, label.text.Length - 1) + (element + 1);

		EditorGUIUtility.labelWidth = 70f;
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty (position, label, property);

		// Draw label
		Rect contentPosition = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID (FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		// Calculate rects
		if (position.height > 16f) 
		{
			position.height = 16f;
			contentPosition.height = 16f;
		}
		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		#region points
		Rect pointsRect = new Rect (contentPosition.x, contentPosition.y, contentPosition.width * 0.25f, contentPosition.height);
		EditorGUIUtility.labelWidth = pointsRect.width * 0.6f;
		EditorGUI.PropertyField (pointsRect, property.FindPropertyRelative ("points"), new GUIContent("points"));
		#endregion

		#region streak
		Rect streakRect = new Rect (pointsRect.x + pointsRect.width, contentPosition.y, contentPosition.width * 0.25f, contentPosition.height);
		EditorGUIUtility.labelWidth = streakRect.width * 0.6f;
		EditorGUI.PropertyField (streakRect, property.FindPropertyRelative ("killStreak"), new GUIContent("/streak"));
		#endregion

		#region monsters
		Rect typesRect = new Rect (streakRect.x + streakRect.width * 1.4f, contentPosition.y, contentPosition.width * 0.5f, contentPosition.height);
		EditorGUIUtility.labelWidth = typesRect.width * 0.6f;
		SerializedProperty p = property.FindPropertyRelative ("enemies");

		typesRect.height = 16f;
		ShowFoldout(typesRect, p, new GUIContent("Monsters"));
		typesRect.y += 16f;

		if(p.isExpanded)
		{
			typesRect.x -= 10f;
			typesRect.width -= 10f;
			//EditorGUI.indentLevel++;
			EditorGUI.PropertyField(typesRect, p.FindPropertyRelative("Array.size"));
			typesRect.y += 16f;
			ShowElements(typesRect, p);
			//EditorGUI.indentLevel--;
		}
		#endregion;
		// Set indent back to what it was
		EditorGUI.indentLevel = indent;
		
		EditorGUI.EndProperty ();
	}

	private void ShowFoldout (Rect position, SerializedProperty property, GUIContent label) 
	{
		position.x -= 14f;
		position.width += 14f;
		label = EditorGUI.BeginProperty(position, label, property);
		property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label, true);
		EditorGUI.EndProperty();
	}

	private void ShowElements (Rect position, SerializedProperty property) 
	{
		for (int i = 0; i < property.arraySize; i++) 
		{
			SerializedProperty element = property.GetArrayElementAtIndex(i);
			position.height = EditorGUI.GetPropertyHeight(element);
			EditorGUI.PropertyField(position, element, GUIContent.none, true);
			position.y += position.height;
		}
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) 
	{
		property = property.FindPropertyRelative ("enemies");

		if (!property.isExpanded) return 16f;

		float height = 32f;
		for (int i = 0; i < property.arraySize; i++) 
			height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));

		return height;
	}
}
