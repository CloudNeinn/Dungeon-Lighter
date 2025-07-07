using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class NoteEditorWindow : EditorWindow
{
    List<Note> mockNotes;
    List<Note> notes;
    FileDataHandler<NoteData> notesHandler;

    [MenuItem("Tools/Notes Editor")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<NoteEditorWindow>();
        wnd.titleContent = new GUIContent("Notes Editor");
    }

    void OnEnable()
    {
        notesHandler = new FileDataHandler<NoteData>("Assets/Resources/", "NoteData.json");
        notes = new List<Note>(notesHandler.Load().notes.Values);
    }

    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/NoteEditorWindow.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/NoteEditorWindow.uss");

        VisualElement root = rootVisualElement;
        root.style.flexGrow = 1;

        root.Clear();

        VisualElement uxmlRoot = visualTree.Instantiate();

        if (styleSheet != null)
        {
            uxmlRoot.styleSheets.Add(styleSheet);
            Debug.Log("Stylesheet added");
        }

        root.Add(uxmlRoot);
        Debug.Log("UXML added to root");

        var notesListView = root.Q<ListView>("notesListView");
        var notetitleField = root.Q<TextField>("notetitleField");
        var noteguidField = root.Q<TextField>("noteguidField");
        var notecontentField = root.Q<TextField>("notecontentField");
        var isReadToggle = root.Q<Toggle>("isReadToggle");
        var notesCountLabel = root.Q<Label>("notesCountLabel");

        //mockNotes = new List<Note>
        //{
        //    new Note { title = "First Note", guid = "1111-AAAA", content = "This is the first note.", isRead = true },
        //    new Note { title = "Second Note", guid = "2222-BBBB", content = "This is the second note.", isRead = false },
        //    new Note { title = "Third Note", guid = "3333-CCCC", content = "This is the third note.", isRead = true }
        //};
        mockNotes = notes;
        Debug.Log($"Created {mockNotes.Count} mock notes");

        if (notesListView != null)
        {
            notesListView.itemsSource = mockNotes;
            notesListView.fixedItemHeight = 24;
            notesListView.selectionType = SelectionType.Single;
            notesListView.showBorder = true;
            notesListView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;

            notesListView.style.flexGrow = 1.0f;

            notesListView.makeItem = () =>
            {
                var label = new Label();
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.paddingLeft = 5;
                label.style.unityFontStyleAndWeight = FontStyle.Normal;
                label.style.whiteSpace = WhiteSpace.Normal;
                return label;
            };

            notesListView.bindItem = (element, i) =>
            {
                if (i >= 0 && i < mockNotes.Count)
                {
                    var label = element as Label;
                    label.text = mockNotes[i].title;
                    label.style.color = mockNotes[i].isRead ? new Color(0.7f, 0.7f, 0.7f) : Color.white;
                }
            };

            notesListView.Rebuild();
        }
        else
        {
            Debug.LogError("Failed to find ListView in UXML");
        }

        if (notesListView != null && notetitleField != null)
        {
            notesListView.onSelectionChange += (selectedItems) =>
            {
                var selectedNote = (selectedItems as IList<Note>)?[0];
                if (selectedNote != null)
                {
                    notetitleField.value = selectedNote.title;
                    noteguidField.value = selectedNote.guid;
                    notecontentField.value = selectedNote.content;
                    isReadToggle.value = selectedNote.isRead;
                }
            };
        }

        if (notesListView != null && mockNotes.Count > 0)
        {
            notesListView.selectedIndex = 0;
        }

        if (notesCountLabel != null)
        {
            notesCountLabel.text = $"Total notes: {mockNotes.Count}";
        }
    }
}
