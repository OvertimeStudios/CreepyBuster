using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(ItemLevelUpCondition))]
public class ItemLevelUpConditionDrawer : PropertyDrawer 
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
		Rect pointsRect = new Rect (contentPosition.x, contentPosition.y, contentPosition.width * 0.18f, contentPosition.height);
		EditorGUIUtility.labelWidth = pointsRect.width * 0.35f;
		EditorGUI.PropertyField (pointsRect, property.FindPropertyRelative ("points"), new GUIContent("pts"));
		#endregion
		
		#region streak
		Rect streakRect = new Rect (pointsRect.x + pointsRect.width, contentPosition.y, contentPosition.width * 0.2f, contentPosition.height);
		EditorGUIUtility.labelWidth = streakRect.width * 0.5f;
		EditorGUI.PropertyField (streakRect, property.FindPropertyRelative ("killStreak"), new GUIContent("/strk"));
		#endregion
		
		#region monsters
		Rect percentRect = new Rect (streakRect.x + streakRect.width * 1.1f, contentPosition.y, contentPosition.width * 0.12f, contentPosition.height);
		EditorGUIUtility.labelWidth = percentRect.width * 0.35f;
		EditorGUI.PropertyField (percentRect, property.FindPropertyRelative ("chanceToSpawn"), new GUIContent("%"));

		Rect typesRect = new Rect (percentRect.x + percentRect.width * 1.6f, contentPosition.y, contentPosition.width * 0.4f, contentPosition.height);
		EditorGUIUtility.labelWidth = typesRect.width * 0.6f;
		SerializedProperty p = property.FindPropertyRelative ("itens");
		
		typesRect.height = 16f;
		ShowFoldout(typesRect, p, new GUIContent("Itens"));
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
		property = property.FindPropertyRelative ("itens");
		
		if (!property.isExpanded) return 16f;
		
		float height = 32f;
		for (int i = 0; i < property.arraySize; i++) 
			height += EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(i));
		
		return height;
	}
}
