using UnityEngine;

[CreateAssetMenu(fileName = "DialogueScriptableObject", menuName = "Scriptable Objects/Dialogue")]
public class DialogueScriptableObject : ScriptableObject
{
    public NPCName npcName;
    public string dialogueID;
    public bool wasPlayed;
    public bool playOnce;
    public Dialogue[] dialogue;
    public LevelSO[] requiredLevels;
}