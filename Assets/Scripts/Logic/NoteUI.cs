using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NoteUI : MonoBehaviour
{
    [SerializeField] private string id;
    [ContextMenu("Generate note id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    [SerializeField] private TextMeshProUGUI textUI;

    void Awake()
    {
    }

    void Start()
    {

    }

    public void LoadNoteText()
    {
        textUI.text = NoteManager.Instance.getNote(id).content;
    }
}
