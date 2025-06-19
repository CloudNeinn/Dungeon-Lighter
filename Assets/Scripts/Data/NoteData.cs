using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public string title;
    public string content;
}

[System.Serializable]
public class NoteDictionary : SerializableDictionary<string, NoteData> { }