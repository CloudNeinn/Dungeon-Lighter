#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Custom Inspector for PaletteSwapper.
/// Adds a "Sample Colors from Sprite" button that reads the sprite's pixels
/// and pre-populates the swap list, letting you pick replacement colors visually.
/// </summary>
[CustomEditor(typeof(PaletteSwapper))]
public class PaletteSwapperEditor : Editor
{
    private PaletteSwapper _target;
    private bool _swapsFoldout = true;

    void OnEnable() => _target = (PaletteSwapper)target;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SerializedProperty threshProp = serializedObject.FindProperty("threshold");
        EditorGUILayout.PropertyField(threshProp, new GUIContent(
            "Match Threshold",
            "How close a pixel's color must be to the 'Original' color to be swapped.\n" +
            "0 = exact match. Increase if anti-aliased edges aren't swapping."));

        EditorGUILayout.Space(6);

        if (GUILayout.Button("Sample Colors from Sprite", GUILayout.Height(28)))
            SampleAndPopulate();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.HelpBox(
            "Sampling reads unique colors from the sprite texture.\n" +
            "The texture must have 'Read/Write Enabled' in its import settings.",
            MessageType.Info);

        EditorGUILayout.Space(6);

        _swapsFoldout = EditorGUILayout.Foldout(_swapsFoldout, $"Color Swaps  ({_target.swaps.Count})", true);

        if (_swapsFoldout)
        {
            SerializedProperty swapsProp = serializedObject.FindProperty("swaps");

            for (int i = 0; i < swapsProp.arraySize; i++)
            {
                SerializedProperty swap = swapsProp.GetArrayElementAtIndex(i);
                SerializedProperty origProp = swap.FindPropertyRelative("original");
                SerializedProperty replProp = swap.FindPropertyRelative("replacement");
                SerializedProperty enabProp = swap.FindPropertyRelative("enabled");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        enabProp.boolValue = EditorGUILayout.Toggle(enabProp.boolValue, GUILayout.Width(16));
                        EditorGUILayout.LabelField($"Swap {i + 1}", EditorStyles.boldLabel, GUILayout.Width(52));

                        EditorGUILayout.LabelField("From", GUILayout.Width(30));
                        origProp.colorValue = EditorGUILayout.ColorField(
                            GUIContent.none, origProp.colorValue, false, false, false,
                            GUILayout.Width(48));

                        EditorGUILayout.LabelField("→", GUILayout.Width(14));

                        EditorGUILayout.LabelField("To", GUILayout.Width(18));
                        replProp.colorValue = EditorGUILayout.ColorField(replProp.colorValue);

                        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                        if (GUILayout.Button("✕", GUILayout.Width(22)))
                        {
                            swapsProp.DeleteArrayElementAtIndex(i);
                            break;
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Add Swap"))
                {
                    swapsProp.InsertArrayElementAtIndex(swapsProp.arraySize);
                }

                if (GUILayout.Button("Clear All"))
                {
                    if (EditorUtility.DisplayDialog("Clear Swaps", "Remove all color swaps?", "Yes", "Cancel"))
                        swapsProp.ClearArray();
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(6);

        GUI.backgroundColor = new Color(0.5f, 1f, 0.5f);
        if (GUILayout.Button("▶  Apply to Renderer", GUILayout.Height(26)))
            _target.Apply();
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            _target.Apply();
    }

    private void SampleAndPopulate()
    {
        List<Color> colors = _target.SampleSpriteColors();

        if (colors.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "No Colors Found",
                "Could not read any colors from the sprite.\n\n" +
                "Make sure the texture has 'Read/Write Enabled' checked in its Import Settings.",
                "OK");
            return;
        }

        Undo.RecordObject(_target, "Sample Sprite Colors");

        HashSet<Color32> existing = new HashSet<Color32>();
        foreach (var s in _target.swaps)
            existing.Add(Quantize32(s.original, 8));

        int added = 0;
        foreach (Color c in colors)
        {
            Color32 q = Quantize32(c, 8);
            if (existing.Contains(q)) continue;
            existing.Add(q);

            _target.swaps.Add(new PaletteSwapper.ColorSwap
            {
                original    = c,
                replacement = c,   // start with identity (same color)
                enabled     = true
            });
            added++;
        }

        EditorUtility.SetDirty(_target);
        Debug.Log($"[PaletteSwapper] Added {added} color(s) from sprite. Total: {_target.swaps.Count}");
    }

    private static Color32 Quantize32(Color c, int step)
    {
        byte Snap(float v) => (byte)(Mathf.RoundToInt(v * 255f / step) * step);
        return new Color32(Snap(c.r), Snap(c.g), Snap(c.b), 255);
    }
}
#endif
