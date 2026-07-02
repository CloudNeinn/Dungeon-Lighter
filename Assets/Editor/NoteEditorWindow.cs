using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;
using System.Linq;
public class NoteEditorWindow : EditorWindow
{
    private List<Note> notes;
    private List<Note> filteredNotes;
    private Dictionary<string, Note> originalNoteVersions;
    private NoteData noteData;
    private HashSet<string> editedNoteGuids = new HashSet<string>();
    private Note selectedNote;
    private string selectedNoteGuid;
    private FileDataHandler<NoteData> notesHandler;
    private Vector2 scrollPosition;
    private bool needsSave;
    private string filePath = "Assets/Resources/";
    private string fileName = "NoteData.json";
    private ListView notesListView;
    private TextField noteTitleField;
    private TextField noteGuidField;
    private TextField noteContentField;
    private ToolbarSearchField searchField;
    private Toggle isReadToggle;
    private Toggle isReturnedToggle;
    private EnumField shotDropdown;
    private Label notesCountLabel;
    private Label notesFilteredCountLabel;
    private Button addNoteButton;
    private Button saveAllButton;
    private Button saveSelectedButton;
    private Button revertNoteButton;
    private Toggle jsonViewToggle;
    private Toggle showReadToggle;
    private Toggle showUnreadToggle;
    public enum NoteListSelectionMode
    {
        RestorePrevious,
        SelectLast,
        SelectFirst,
        None
    }


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
        filteredNotes = new List<Note>(notes);
        originalNoteVersions = new Dictionary<string, Note>();
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
        searchField = root.Q<ToolbarSearchField>("searchField");
        isReadToggle = root.Q<Toggle>("isReadToggle");
        isReturnedToggle = root.Q<Toggle>("isReturnedToggle");
        shotDropdown = root.Q<EnumField>("noteShotDropdown");
        notesCountLabel = root.Q<Label>("notesCountLabel");
        notesFilteredCountLabel = root.Q<Label>("notesFilteredCountLabel");
        addNoteButton = root.Q<Button>("addNoteButton");
        saveAllButton = root.Q<Button>("saveAllButton");
        saveSelectedButton = root.Q<Button>("saveSelectedButton");
        jsonViewToggle = root.Q<Toggle>("jsonViewToggle");
        showReadToggle = root.Q<Toggle>("showReadToggle");
        showUnreadToggle = root.Q<Toggle>("showUnreadToggle");
        revertNoteButton = root.Q<Button>("revertNoteButton");

        if (revertNoteButton != null)
            revertNoteButton.clicked += revertSelectedNote;

        if (addNoteButton != null)
            addNoteButton.clicked += createNote;

        if (saveAllButton != null)
            saveAllButton.clicked += saveNotes;

        if (saveSelectedButton != null)
            saveSelectedButton.clicked += saveSelectedNote;

        if (shotDropdown != null)
        {
            shotDropdown.Init(Shots.None);
        }

        if (notesListView != null)
        {
            notesListView.itemsSource = filteredNotes;
            notesListView.fixedItemHeight = 24;
            notesListView.selectionType = SelectionType.Single;
            notesListView.showBorder = true;
            notesListView.showAlternatingRowBackgrounds = AlternatingRowBackground.All;

            notesListView.style.flexGrow = 1.0f;

            notesListView.makeItem = () =>
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.alignItems = Align.Center;

                var label = new Label();
                label.style.flexGrow = 1;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.paddingLeft = 5;
                label.style.unityFontStyleAndWeight = FontStyle.Normal;
                label.style.whiteSpace = WhiteSpace.Normal;

                var deleteButton = new Button();
                deleteButton.text = "Delete";
                deleteButton.style.marginLeft = 8;
                deleteButton.style.backgroundColor = new Color(0.7f, 0.2f, 0.2f);
                deleteButton.style.color = Color.white;

                row.Add(label);
                row.Add(deleteButton);

                row.userData = new System.Tuple<Label, Button>(label, deleteButton);

                return row;
            };

            notesListView.bindItem = (element, i) =>
            {
                if (i >= 0 && i < filteredNotes.Count)
                {
                    var row = element as VisualElement;
                    var refs = row.userData as System.Tuple<Label, Button>;
                    var label = refs.Item1;
                    var deleteButton = refs.Item2;

                    var note = filteredNotes[i];
                    var noteGuid = noteData.getNoteGuid(note);

                    label.text = note.title;
                    label.style.color = note.isRead ? new Color(0.7f, 0.7f, 0.7f) : Color.white;
                    row.RemoveFromClassList("note-item-edited");
                    if (noteGuid != null && editedNoteGuids.Contains(noteGuid))
                    {
                        row.AddToClassList("edited-note");
                    }

                    deleteButton.clicked -= deleteButton.userData as System.Action;

                    System.Action onClick = () =>
                    {
                        bool confirm = EditorUtility.DisplayDialog(
                            "Delete Note",
                            $"Are you sure you want to delete '{note.title}'?",
                            "Delete", "Cancel"
                        );
                        if (confirm)
                        {
                            noteData.notes.Remove(noteGuid);
                            notes.RemoveAt(i);
                            filteredNotes.RemoveAt(i);

                            notesListView.selectedIndex = Mathf.Clamp(notesListView.selectedIndex, 0, notes.Count - 1);

                            notesListView.RefreshItems();
                            notesCountLabel.text = $"Total notes: {notes.Count}";
                            notesFilteredCountLabel.text = $"Filtered notes: {filteredNotes.Count}";
                        }
                    };

                    deleteButton.userData = onClick;
                    deleteButton.clicked += onClick;
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
            notesListView.selectionChanged += (selectedItems) =>
            {
                selectedNote = notesListView.selectedItem as Note;
                    if (selectedNote != null)
                    {
                        foreach (var kvp in noteData.notes)
                        {
                            if (kvp.Value == selectedNote)
                            {
                                selectedNoteGuid = kvp.Key;
                                break;
                            }
                        }
                        noteGuidField.value = selectedNoteGuid;
                        noteTitleField.value = selectedNote.title;
                        noteContentField.value = selectedNote.content;
                        isReadToggle.value = selectedNote.isRead;
                        isReturnedToggle.value = selectedNote.isReturned;
                        shotDropdown.SetValueWithoutNotify(selectedNote.shot);

                        // Store the original version only if not already stored
                        if (!originalNoteVersions.ContainsKey(selectedNoteGuid))
                        {
                            originalNoteVersions[selectedNoteGuid] = new Note
                            {
                                title = selectedNote.title,
                                content = selectedNote.content,
                                isRead = selectedNote.isRead,
                                shot = selectedNote.shot
                            };
                        }
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
            notesFilteredCountLabel.text = $"Filtered notes: {filteredNotes.Count}";
        }

        editNotes();
        filterNotes();
    }

    void createNote()
    {
        Note newNote = new Note
        {
            title = "New Note " + (notes.Count + 1),
            content = "Enter your note content here...",
            isRead = false,
            isReturned = false,
            shot = Shots.None
        };

        string newNoteGuid = Guid.NewGuid().ToString();
        noteData.notes[newNoteGuid] = newNote;
        notes.Add(newNote);
        filteredNotes.Add(newNote);
        selectedNote = newNote;
        selectedNoteGuid = newNoteGuid;
        needsSave = true;
        editedNoteGuids.Add(newNoteGuid);

        if (notesCountLabel != null)
        {
            notesCountLabel.text = $"Total notes: {notes.Count}";
            notesFilteredCountLabel.text = $"Filtered notes: {filteredNotes.Count}";
        }

        EditorApplication.delayCall += () => {
            scrollPosition.y = float.MaxValue;
        };

        rebuildNoteList();
        Repaint();
    }

    void rebuildNoteList(NoteListSelectionMode selectionMode = NoteListSelectionMode.SelectLast)
    {
        int selectedIndex = notesListView.selectedIndex;

        if (notesListView != null)
        {
            notesListView.itemsSource = null;
            notesListView.itemsSource = filteredNotes;
            notesListView.Rebuild();

            switch (selectionMode)
            {
                case NoteListSelectionMode.RestorePrevious:
                    notesListView.selectedIndex = selectedIndex;
                    break;
                case NoteListSelectionMode.SelectLast:
                    notesListView.selectedIndex = notes.Count - 1;
                    break;
                case NoteListSelectionMode.SelectFirst:
                    notesListView.selectedIndex = 0;
                    break;
                case NoteListSelectionMode.None:
                    notesListView.selectedIndex = -1;
                    break;
            }
        }

        Repaint();
    }


    void saveNotes()
    {
        notesHandler.Save(noteData);
        editedNoteGuids.Clear();
        originalNoteVersions.Clear();
        needsSave = false;
    }

    void saveSelectedNote()
    {
        if (selectedNote != null && selectedNoteGuid != null)
        {
            noteData.notes[selectedNoteGuid] = selectedNote;
            notesHandler.Save(noteData);
            editedNoteGuids.Remove(selectedNoteGuid);
            if(editedNoteGuids.Count <= 0) needsSave = false;
            rebuildNoteList(NoteListSelectionMode.RestorePrevious);
        }
    }

    void filterNotes()
    {
        searchField.RegisterValueChangedCallback(evt =>
        {
            string search = evt.newValue;

            filteredNotes.Clear();

            if (string.IsNullOrEmpty(search))
            {
                filteredNotes.AddRange(notes);
            }
            else
            {
                foreach (var note in notes)
                {
                    if ((note.title != null && note.title.Contains(search, System.StringComparison.OrdinalIgnoreCase)) ||
                        (note.content != null && note.content.Contains(search, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        filteredNotes.Add(note);
                    }
                }
            }

            notesListView.itemsSource = filteredNotes;
            rebuildNoteList();
            Repaint();
        });

        showReadToggle.RegisterValueChangedCallback(evt => applyReadUnreadFilter());
        showUnreadToggle.RegisterValueChangedCallback(evt => applyReadUnreadFilter());
    }

    void revertSelectedNote()
    {
        if (selectedNote != null && selectedNoteGuid != null && originalNoteVersions.ContainsKey(selectedNoteGuid))
        {
            var original = originalNoteVersions[selectedNoteGuid];

            selectedNote.title = original.title;
            selectedNote.content = original.content;
            selectedNote.isRead = original.isRead;
            selectedNote.isReturned = original.isReturned;
            selectedNote.shot = original.shot;

            noteTitleField.value = original.title;
            noteContentField.value = original.content;
            isReadToggle.value = original.isRead;
            isReturnedToggle.value = original.isReturned;

            editedNoteGuids.Remove(selectedNoteGuid);
            originalNoteVersions.Remove(selectedNoteGuid);

            needsSave = editedNoteGuids.Count > 0;

            var selectedIndex = notesListView.selectedIndex;
            notesListView.Rebuild();
            notesListView.selectedIndex = selectedIndex;
            Repaint();
        }
    }

    void applyReadUnreadFilter()
    {
        bool showRead = showReadToggle.value;
        bool showUnread = showUnreadToggle.value;

        filteredNotes.Clear();

        if (!showRead && !showUnread)
        {
            filteredNotes.AddRange(notes);
        }
        else if (showRead && !showUnread)
        {
            filteredNotes.AddRange(notes.Where(x => x.isRead));
        }
        else if (!showRead && showUnread)
        {
            filteredNotes.AddRange(notes.Where(x => !x.isRead));
        }

        notesListView.itemsSource = filteredNotes;
        rebuildNoteList();
        Repaint();
    }

    void editNotes()
    {
        noteTitleField.RegisterValueChangedCallback(evt =>
        {
            if (selectedNote != null && selectedNote.title != evt.newValue)
            {
                selectedNote.title = evt.newValue;
                onNoteEdit();
            }
        });

        noteGuidField.RegisterValueChangedCallback(evt =>
        {
            if (selectedNote != null && selectedNoteGuid != evt.newValue)
            {
                noteData.notes.Remove(selectedNoteGuid);
                selectedNoteGuid = evt.newValue;
                noteData.notes[selectedNoteGuid] = selectedNote;
                onNoteEdit();
            }
        });

        noteContentField.RegisterValueChangedCallback(evt =>
        {
            if (selectedNote != null && selectedNote.content != evt.newValue)
            {
                selectedNote.content = evt.newValue;
                onNoteEdit();
            }
        });

        isReadToggle.RegisterValueChangedCallback(evt =>
        {
            if (selectedNote != null && selectedNote.isRead != evt.newValue)
            {
                selectedNote.isRead = evt.newValue;
                onNoteEdit();
            }
        });

        isReturnedToggle.RegisterValueChangedCallback(evt =>
        {
            if (selectedNote != null && selectedNote.isRead != evt.newValue)
            {
                selectedNote.isReturned = evt.newValue;
                onNoteEdit();
            }
        });

        shotDropdown.RegisterValueChangedCallback(evt =>
        {
            var newShot = (Shots)evt.newValue;
            if (selectedNote != null && selectedNote.shot != newShot)
            {
                selectedNote.shot = newShot;
                onNoteEdit();
            }
        });

    }

    void onNoteEdit()
    {
        if (selectedNoteGuid != null)
            editedNoteGuids.Add(selectedNoteGuid);
        needsSave = true;
        var selectedIndex = notesListView.selectedIndex;
        notesListView.Rebuild();
        notesListView.selectedIndex = selectedIndex;
    }


    void toggleJsonView()
    {
        
    }
    
    private void OnDestroy()
    {
        if (needsSave)
        {
            if (EditorUtility.DisplayDialog("Unsaved Changes", 
                "You have unsaved changes. Would you like to save them? THERE IS NO CANCEL BUTTON!", "Save", "Don't Save"))
            {
                saveNotes();
            }
        }
    }
}