using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ProSceneManager
{
    public class NotesWindow : EditorWindow
    {
        private static string scenePath;
        private static Dictionary<string, string> notesDict;
        private string noteText = "";

        public static void OpenWindow(string sceneName, string path, Dictionary<string, string> notes)
        {
            NotesWindow window = GetWindow<NotesWindow>("Notes: " + sceneName);
            scenePath = path;
            notesDict = notes;
            window.noteText = notes.ContainsKey(path) ? notes[path] : "";
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Notes", EditorStyles.boldLabel);
            noteText = EditorGUILayout.TextArea(noteText, GUILayout.Height(100));

            if (GUILayout.Button("Save"))
            {
                notesDict[scenePath] = noteText;
                Close();
            }
        }
    }
}