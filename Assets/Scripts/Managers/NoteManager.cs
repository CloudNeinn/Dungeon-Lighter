using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NoteManager : MonoBehaviour
{
    public static NoteManager Instance;
    [SerializeField] public SerializableDictionary<string, Note> notes;

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

    public Note getNote(string guid)
    {
        if (string.IsNullOrEmpty(guid))
        {
            Debug.LogWarning("Attempted to get a note with null or empty guid");
            return null;
        }

        if (notes != null && notes.TryGetValue(guid, out Note note))
        {
            if (note != null)
            {
                note.isRead = true;
                return note;
            }
        }

        Debug.LogWarning($"Note with guid '{guid}' not found in the notes dictionary");
        return null;
    }
}
