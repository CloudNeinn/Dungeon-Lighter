using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    public static FPSManager Instance;
    public enum FrameRate
    {
        FPS_15 = 15,
        FPS_30 = 30,
        FPS_60 = 60,
        FPS_120 = 120,
        FPS_144 = 144,
        FPS_240 = 240,
        UNLIMITED = 0
    }

    [field: SerializeField] public FrameRate frameRate {get; private set; }
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
        
        DontDestroyOnLoad(this.gameObject);
    }
    void Update()
    {
        Application.targetFrameRate = (int)frameRate;
    }
}
