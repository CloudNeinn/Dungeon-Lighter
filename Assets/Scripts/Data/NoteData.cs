using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoteData
{
    public SerializableDictionary<string, Note> notes;

    public NoteData()
    {
        notes = new SerializableDictionary<string, Note>
        {
            {
                "note_1",
                new Note
                {
                    uuid = "note_1",
                    title = "Welcome to the Game",
                    content = "This is your first note. Explore the world to find more!",
                    isRead = false
                }
            },
            {
                "note_2",
                new Note
                {
                    uuid = "note_2",
                    title = "Controls",
                    content = "Use WASD to move. Press Space to jump. Press E to interact.",
                    isRead = false
                }
            }
        };
    }
}