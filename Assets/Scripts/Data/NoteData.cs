using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public string title;
    public string content;
    // Add other fields as needed (e.g., found status, timestamp)
}

[System.Serializable]
public class NoteDictionary : SerializableDictionary<string, NoteData> { }