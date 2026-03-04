using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameState", menuName = "Scriptable Objects/Game State")]
public class GameState : ScriptableObject
{
    [SerializeField] private int _numberOfLevelsCompleted;
    [SerializeField] private StringBoolDictionary _listOfLevels;
    [SerializeField] private StringBoolDictionary _listOfNotes;
    [SerializeField] private bool _enoughShotsDrinken;
    
    public StringBoolDictionary GetListOfLevel()
    {
        return _listOfLevels;
    }
}
