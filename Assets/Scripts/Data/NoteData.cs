using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    [SerializeField] private List<Note> _notesList = new List<Note>();
    private SerializableDictionary<string, Note> _notes;

    public SerializableDictionary<string, Note> notes
    {
        get
        {
            if (_notes == null)
            {
                _notes = new SerializableDictionary<string, Note>();
                foreach (var note in _notesList)
                {
                    if (!string.IsNullOrEmpty(note.guid))
                    {
                        _notes[note.guid] = note;
                    }
                }
            }
            return _notes;
        }
    }

    public NoteData()
    {
        // Only initialize if notes is empty
        if (_notesList.Count == 0)
        {
            _notesList.Add(new Note
            {
                guid = "0b8bdd4f-b2f1-427b-a20e-cb0a8aa31d59",
                title = "Welcome to the Game",
                content = "This is your first note. Explore the world to find more!",
                isRead = false
            });

            _notesList.Add(new Note
            {
                guid = "0b8bdd4f-b2f1-427b-a20e-cb0a8aa31d50",
                title = "Controls",
                content = "Use WASD to move. Press Space to jump. Press E to interact.",
                isRead = false
            });
        }
    }

    public void SyncDictionaryToList()
    {
        if (_notes == null) return;
        
        // Update the list to match the dictionary
        _notesList.Clear();
        foreach (var kvp in _notes)
        {
            _notesList.Add(kvp.Value);
        }
    }
}