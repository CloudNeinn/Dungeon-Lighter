using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "Scriptable Objects/Level")]
public class LevelSO : ScriptableObject
{
    public string levelKey;
    public string levelName;
    public bool completed;
}