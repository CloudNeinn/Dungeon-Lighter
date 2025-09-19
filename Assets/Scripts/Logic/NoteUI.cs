using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    [SerializeField] private string _id;

    [ContextMenu("Generate note id")]
    private void GenerateGuid()
    {
        _id = System.Guid.NewGuid().ToString();
    }
    [SerializeField] private TextMeshProUGUI textUI;

    public string GetId()
    {
        return _id;
    }

    public void SetId(string newId)
    {
        _id = newId;

        if (textUI != null)
        {
            if (Application.isPlaying && NoteManager.Instance != null) LoadNoteText();
            else textUI.text = $"(Note content unavailable in editor)";
        }
    }

    public void LoadNoteText()
    {
        if (NoteManager.Instance == null)
        {
            Debug.LogWarning("NoteManager.Instance is null - cannot load note text.");
            return;
        }

        Note note = NoteManager.Instance.getNote(_id);
        if (note == null)
        {
            Debug.LogWarning($"No note found with id {_id}");
            textUI.text = "(Note not found)";
            return;
        }

        textUI.text = note.content;
    }
}
