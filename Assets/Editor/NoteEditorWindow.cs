using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class NoteEditorWindow : EditorWindow
{
    private List<Note> mockNotes;
    private List<Note> notes;
    private NoteData noteData;
    private Note selectedNote;
    private FileDataHandler<NoteData> notesHandler;
    private Vector2 scrollPosition;
    private bool needsSave;
    private string filePath = "Assets/Resources/";
    private string fileName = "NoteData.json";
    private ListView notesListView;
    private TextField noteTitleField;
    private TextField noteGuidField;
    private TextField noteContentField;
    private Toggle isReadToggle;
    private Label notesCountLabel;
    private Button addNoteButton;
    private Button saveAllButton;
    private Button jsonViewButton;
    private Toggle showReadToggle;
    private Toggle showUnreadToggle;


    [MenuItem("Tools/Notes Editor")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<NoteEditorWindow>();
        wnd.titleContent = new GUIContent("Notes Editor");
    }

    void OnEnable()
    {
        notesHandler = new FileDataHandler<NoteData>(filePath, fileName);
        noteData = notesHandler.Load();
        notes = new List<Note>(noteData.notes.Values);
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

        notesListView = root.Q<ListView>("notesListView");
        noteTitleField = root.Q<TextField>("noteTitleField");
        noteGuidField = root.Q<TextField>("noteGuidField");
        noteContentField = root.Q<TextField>("noteContentField");
        isReadToggle = root.Q<Toggle>("isReadToggle");
        notesCountLabel = root.Q<Label>("notesCountLabel");
        addNoteButton = root.Q<Button>("addNoteButton");
        saveAllButton = root.Q<Button>("saveAllButton");
        jsonViewButton = root.Q<Button>("jsonViewButton");
        showReadToggle = root.Q<Toggle>("showReadToggle");
        showUnreadToggle = root.Q<Toggle>("showUnreadToggle");

        if (addNoteButton != null)
            addNoteButton.clicked += createNote;

        if (saveAllButton != null)
            saveAllButton.clicked += saveNotes;

        if (jsonViewButton != null)
            jsonViewButton.clicked += createNote;

        if (notesListView != null)
        {
            notesListView.itemsSource = notes;
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
                if (i >= 0 && i < notes.Count)
                {
                    var label = element as Label;
                    label.text = notes[i].title;
                    label.style.color = notes[i].isRead ? new Color(0.7f, 0.7f, 0.7f) : Color.white;
                }
            };

            notesListView.Rebuild();
        }
        else
        {
            Debug.LogError("Failed to find ListView in UXML");
        }

        if (notesListView != null && noteTitleField != null)
        {
            notesListView.onSelectionChange += (selectedItems) =>
            {
                selectedNote = notesListView.selectedItem as Note;
                if (selectedNote != null)
                {
                    noteTitleField.value = selectedNote.title;
                    noteGuidField.value = selectedNote.guid;
                    noteContentField.value = selectedNote.content;
                    isReadToggle.value = selectedNote.isRead;
                }
            };
        }

        if (notesListView != null && notes.Count > 0)
        {
            notesListView.selectedIndex = 0;
        }

        if (notesCountLabel != null)
        {
            notesCountLabel.text = $"Total notes: {notes.Count}";
        }
    }

    void createNote()
    {
        Debug.Log("asdfasdfas");
        Note newNote = new Note
        {
            guid = Guid.NewGuid().ToString(),
            title = "New Note",
            content = "Enter your note content here...",
            isRead = false
        };

        noteData.notes[newNote.guid] = newNote;
        notes.Add(newNote);
        selectedNote = newNote;
        needsSave = true;

        if (notesListView != null)
        {
            notesListView.itemsSource = null;
            notesListView.itemsSource = notes;

            notesListView.Rebuild();

            notesListView.selectedIndex = notes.Count - 1;
        }

        EditorApplication.delayCall += () => {
            scrollPosition.y = float.MaxValue;
        };

        Repaint();
    }

    void deleteNote()
    {

    }

    void saveNotes()
    {
        notesHandler.Save(noteData);
        needsSave = false;
    }
    
    private void OnDestroy()
    {
        if (needsSave)
        {
            if (EditorUtility.DisplayDialog("Unsaved Changes",
                "You have unsaved changes. Would you like to save them?", "Save", "Don't Save"))
            {
                saveNotes();
            }
        }
    }
}