using UnityEngine;

public class GameManager : MonoBehaviour, IDataPersistance
{
    public static GameManager Instance;
    [SerializeField] private int _consecutiveShotsDrinken;
    [SerializeField] private int _needToDrinkConsecutiveShots;
    [field: SerializeField] public GameData gameData {get; private set;}

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckIfToleranceIncreases()
    {
        if (gameData.toleranceThresholds[gameData.toleranceLevel + 1] <= gameData.shotsConsumed) gameData.toleranceLevel++;
    }

    public void SetLevelComplete(string levelName)
    {
        if (gameData.listOfLevels.ContainsKey(levelName))
        {
            gameData.listOfLevels[levelName] = true;
        }
        else Debug.LogWarning($"[GameManager] Level '{levelName}' not found in level list dictionary.");
    }

    public void LoadData(GameData data)
    {
        gameData.toleranceLevel = data.toleranceLevel;
        gameData.listOfLevels = data.listOfLevels;
        gameData.listOfNotes = data.listOfNotes;
        gameData.shotsConsumed = data.shotsConsumed;
        gameData.toleranceThresholds = data.toleranceThresholds;
    }

    public void SaveData(ref GameData data)
    {
        data.toleranceLevel = gameData.toleranceLevel;
        data.listOfLevels = gameData.listOfLevels;
        data.listOfNotes = gameData.listOfNotes;
        data.shotsConsumed = gameData.shotsConsumed;
        data.toleranceThresholds = gameData.toleranceThresholds;
    }
}
