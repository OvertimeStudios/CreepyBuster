using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(RandomBetweenTwoConst))]
public class RandomBetweenTwoConstDrawer : PropertyDrawer 
{
	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
	{
		Rect labelRect = new Rect (position.x, position.y, position.width * 0.35f, position.height);

		EditorGUI.LabelField (labelRect, label);

		SerializedProperty minValue = property.FindPropertyRelative( "min" );
		SerializedProperty maxValue = property.FindPropertyRelative( "max" );

		//TODO: clamp float value
		/*if (minValue.floatValue > maxValue.floatValue)
			maxValue.floatValue = minValue.floatValue;

		if (maxValue.floatValue < minValue.floatValue)
			minValue.floatValue = maxValue.floatValue;*/

		Rect minRect = new Rect (labelRect.x + labelRect.width, position.y, position.width * 0.3f, position.height);
		Rect maxRect = new Rect (minRect.x + minRect.width + 2f, position.y, position.width * 0.3f, position.height);

		EditorGUIUtility.labelWidth = minRect.width * 0.3f;
		EditorGUI.PropertyField (minRect, minValue, new GUIContent ("min"));
		EditorGUIUtility.labelWidth = maxRect.width * 0.3f;
		EditorGUI.PropertyField (maxRect, maxValue, new GUIContent ("max"));
	}
}
