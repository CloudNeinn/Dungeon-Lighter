#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System.Collections.Generic;

[CustomEditor(typeof(NoteUI))]
public class NoteUIEditor : Editor
{
    NoteData editorNoteData;

    void LoadNoteData()
    {
        if (editorNoteData != null)
            return;

        TextAsset jsonAsset = Resources.Load<TextAsset>("NoteData");
        if (jsonAsset != null)
        {
            var rootNode = JSON.Parse(jsonAsset.text);
            var notesNode = rootNode["notes"];
            if (notesNode != null)
            {
                editorNoteData = new NoteData();
                editorNoteData.notes = new SerializableDictionary<string, Note>();

                foreach (var kvp in notesNode.AsObject)
                {
                    Note note = new Note();
                    note.title = kvp.Value["title"];
                    note.content = kvp.Value["content"];
                    note.isRead = kvp.Value["isRead"].AsBool;

                    editorNoteData.notes.Add(kvp.Key, note);
                }
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NoteUI noteUI = (NoteUI)target;

        LoadNoteData();

        if (editorNoteData != null && editorNoteData.notes != null && editorNoteData.notes.Count > 0)
        {
            // Build arrays for titles and keys
            string[] titles = new string[editorNoteData.notes.Count];
            string[] keys = new string[editorNoteData.notes.Count];
            int selectedIndex = -1;

            int i = 0;
            foreach (var kvp in editorNoteData.notes)
            {
                keys[i] = kvp.Key;
                titles[i] = kvp.Value.title;

                if (kvp.Key == noteUI.GetId())
                {
                    selectedIndex = i;
                }
                i++;
            }

            int newIndex = EditorGUILayout.Popup("Select Note by Title", selectedIndex, titles);

            if (newIndex != selectedIndex && newIndex >= 0)
            {
                Undo.RecordObject(noteUI, "Change Note Selection");
                noteUI.SetId(keys[newIndex]);
                EditorUtility.SetDirty(noteUI);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Could not load notes data from Resources/NoteData.json", MessageType.Warning);
        }
    }
}
#endif
