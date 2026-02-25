using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [field: SerializeField] public int toleranceLevel { get; private set; }
    [field: SerializeField] public int completedLevels { get; private set; }
    [SerializeField] private int _toleranceIncreaseThreshold;
    [SerializeField] private int _consecutiveShotsDrinken;
    [SerializeField] private int _needToDrinkConsecutiveShots;
    [SerializeField] private GameState _gameState;

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

    public void checkIfToleranceIncreases()
    {
        if (toleranceLevel * _toleranceIncreaseThreshold <= completedLevels) toleranceLevel++;
    }

    public void increaseCompletedLevels()
    {
        completedLevels++;
        checkIfToleranceIncreases();
    }
}
