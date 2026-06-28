using UnityEngine;
using System.Collections.Generic;
using Eflatun.SceneReference;

[CreateAssetMenu(fileName = "LevelNameDB", menuName = "Scriptable Objects/LevelNameDB")]
public class LevelNameDB : ScriptableObject
{
    public List<SceneReference> scenes;
    public List<string> sceneDisplayNames;
}
