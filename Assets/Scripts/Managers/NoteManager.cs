using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;
    public SerializableDictionary<string, Note> notes;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public Note getNote(string uuid)
    {
        if (string.IsNullOrEmpty(uuid))
        {
            Debug.LogWarning("Attempted to get a note with null or empty UUID");
            return null;
        }

        if (notes != null && notes.TryGetValue(uuid, out Note note))
        {
            if (note != null)
            {
                note.isRead = true;
                return note;
            }
        }

        Debug.LogWarning($"Note with UUID '{uuid}' not found in the notes dictionary");
        return null;
    }
}
