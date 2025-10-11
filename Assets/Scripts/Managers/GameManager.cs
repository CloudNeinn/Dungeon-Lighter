using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [field: SerializeField] public int _toleranceLevel { get; private set; }
    [field: SerializeField] public int _completedLevels { get; private set; }
    [SerializeField] private int _toleranceIncreaseThreshold;

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
        if (_toleranceLevel * _toleranceIncreaseThreshold <= _completedLevels) _toleranceLevel++;
    }

    public void increaseCompletedLevels()
    {
        _completedLevels++;
        checkIfToleranceIncreases();
    }
}
