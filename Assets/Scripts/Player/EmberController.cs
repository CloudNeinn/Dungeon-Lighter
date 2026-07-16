using UnityEngine;

public class EmberController : MonoBehaviour
{
    public static EmberController Instance;
    [SerializeField] public Ember[] embers;
    [SerializeField] private bool _allowEmberReset;
    [SerializeField] private int _availableEmberCount;
    [field: SerializeField] public int activeEmberCount;
    [SerializeField] private float _emberResetTime;
    private float _emberResetTimeCooldown;
    private float _resetSpeedMultiplier;

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
    
    void Start()
    {
        for (int i = 0; i < _availableEmberCount; i++)
        {
            if (embers[i] != null)
            {
                embers[i].gameObject.SetActive(true);
            }
        }
    }

    void Update()
    {
        if(_allowEmberReset && _availableEmberCount > activeEmberCount && _emberResetTimeCooldown <= 0) 
        {
            ResetEmber();
            _emberResetTimeCooldown = _emberResetTime;
        }
        else if(_emberResetTimeCooldown > 0) 
        {
            _emberResetTimeCooldown -= Time.deltaTime * _resetSpeedMultiplier;
        }
    }
    
    void ResetEmber()
    {
        embers[activeEmberCount].ActivateEmber();
    }
    
    public void ConsumeEmber()
    {
        embers[activeEmberCount - 1].DeactivateEmber();
    }

    public bool HasEmber()
    {
        return activeEmberCount > 0;
    }

    public void StartEmberReset(float multiplier = 1)
    {
        _resetSpeedMultiplier = multiplier;
        _allowEmberReset = true;
    }

    public void StopEmberReset()
    {
        _allowEmberReset = false;
    }
}