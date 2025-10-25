using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(StringBoolDictionary))]
public class StringBoolDictionaryDrawer : PropertyDrawer
{
    private static float RowHeight = EditorGUIUtility.singleLineHeight + 2f;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty keys = property.FindPropertyRelative("keys");
        SerializedProperty values = property.FindPropertyRelative("values");

        EditorGUI.BeginProperty(position, label, property);

        // Header
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(position, label);
        position.y += RowHeight;

        // Draw each key-value pair
        for (int i = 0; i < keys.arraySize; i++)
        {
            Rect keyRect = new Rect(position.x, position.y, position.width * 0.45f, EditorGUIUtility.singleLineHeight);
            Rect valueRect = new Rect(position.x + position.width * 0.5f, position.y, position.width * 0.35f, EditorGUIUtility.singleLineHeight);
            Rect removeRect = new Rect(position.x + position.width * 0.9f, position.y, position.width * 0.1f, EditorGUIUtility.singleLineHeight);

            // Draw key
            EditorGUI.PropertyField(keyRect, keys.GetArrayElementAtIndex(i), GUIContent.none);
            // Draw value
            EditorGUI.PropertyField(valueRect, values.GetArrayElementAtIndex(i), GUIContent.none);
            // Draw remove button
            if (GUI.Button(removeRect, "-"))
            {
                keys.DeleteArrayElementAtIndex(i);
                values.DeleteArrayElementAtIndex(i);
                break; // Stop drawing so indices don't get messed up
            }
            position.y += RowHeight;
        }

        // Draw Add button
        Rect addRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(addRect, "Add"))
        {
            keys.InsertArrayElementAtIndex(keys.arraySize);
            values.InsertArrayElementAtIndex(values.arraySize);
            // Optionally set default values here, e.g.:
            keys.GetArrayElementAtIndex(keys.arraySize - 1).stringValue = "";
            values.GetArrayElementAtIndex(values.arraySize - 1).boolValue = false;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty keys = property.FindPropertyRelative("keys");
        return (keys.arraySize + 2) * RowHeight; // +2 for header + add button
    }
}
