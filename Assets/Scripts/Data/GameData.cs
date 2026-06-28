using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class GameData
{
    public StringBoolDictionary listOfLevels;
    public StringBoolDictionary listOfNotes;
    public int toleranceLevel;
    public int shotsConsumed;
    public int[] toleranceThresholds;

    public StringBoolDictionary GetListOfLevel()
    {
        return listOfLevels;
    }
}
