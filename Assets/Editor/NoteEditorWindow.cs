using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements; // Needed for Toolbar and ToolbarButton

public class NoteEditorWindow : EditorWindow
{
    private string searchQuery = "";
    private bool showRead = true;
    private bool showUnread = true;
    private string dataPath = "Assets/Resources/NoteData.json";
    private bool needsSave = false;
    private NoteData noteData;
    private Note selectedNote;

    [MenuItem("Tools/Note Editor")]
    public static void OpenWindow()
    {
        NoteEditorWindow wnd = GetWindow<NoteEditorWindow>();
        wnd.titleContent = new GUIContent("Note Editor");
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;
        var toolbar = new Toolbar();
        var splitView = new TwoPaneSplitView(0, 150, TwoPaneSplitViewOrientation.Horizontal);
        var rightPane = new VisualElement();
        var leftPane = new ListView();

        var addNoteButton = new ToolbarButton(AddNote)
        {
            text = "Add Note"
        };

        var toggleReadButton = new ToolbarToggle
        {
            text = "Read",
            value = showRead
        };
        toggleReadButton.RegisterValueChangedCallback(evt =>
        {
            showRead = evt.newValue;
        });

        var toggleUnreadButton = new ToolbarToggle
        {
            text = "Unread",
            value = showUnread
        };
        toggleUnreadButton.RegisterValueChangedCallback(evt =>
        {
            showUnread = evt.newValue;
        });

        toolbar.Add(addNoteButton);
        toolbar.Add(toggleReadButton);
        toolbar.Add(toggleUnreadButton);

        splitView.Add(leftPane);
        splitView.Add(rightPane);

        root.Add(toolbar);
        root.Add(splitView);
    }

    private void AddNote()
    {
        Debug.Log("Note added");
        Debug.Log(showRead);
        Debug.Log(showUnread);
    }

    private void ToggleReadButton()
    {
        showRead = !showRead;
    }

    private void ToggleUnreadButton()
    {
        showUnread = !showUnread;
    }
    
    //private void OnDestroy()
    //{
    //    if (needsSave)
    //    {
    //        if (EditorUtility.DisplayDialog("Unsaved Changes", 
    //            "You have unsaved changes. Would you like to save them?", "Save", "Don't Save"))
    //        {
    //            SaveNotes();
    //        }
    //    }
    //}
}
