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
                "0b8bdd4f-b2f1-427b-a20e-cb0a8aa31d59",
                new Note
                {
                    guid = "0b8bdd4f-b2f1-427b-a20e-cb0a8aa31d59",
                    title = "Welcome to the Game",
                    content = "This is your first note. Explore the world to find more!",
                    isRead = false
                }
            },
            {
                "0b8bdd4f-b2f1-427b-a20e-cb0a8aa31d50",
                new Note
                {
                    guid = "0b8bdd4f-b2f1-427b-a20e-cb0a8aa31d50",
                    title = "Controls",
                    content = "Use WASD to move. Press Space to jump. Press E to interact.",
                    isRead = false
                }
            }
        };
    }
}