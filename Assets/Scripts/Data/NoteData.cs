using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public SerializableDictionary<string, Note> notes;
    public string getNoteGuid(Note note)
    {
        foreach (var kvp in notes)
        {
            if (kvp.Value == note)
            {
                return kvp.Key;
            }
        }
        return null;
    }
}