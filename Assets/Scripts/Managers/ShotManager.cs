using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShotManager : MonoBehaviour
{
    public static ShotManager Instance;
    public enum Shots{
        None = -1,
        QuickWick = 0,
        FloatFlicker = 1,
        OverBurn = 2,
        SniffSpark = 3,
        TripTrap = 4,
        GoldGulp = 5,
        AshGlide = 6,
        HotDamn = 7,
        NoctiDrop = 8
    }
    [SerializeField] private float _originalSpeed;
    [SerializeField] private int _originalDoubleJumpIndex;
    [SerializeField] private float _originalGravity;
    [field: SerializeField] public Queue<Shots> activeShots { get; private set; }

    // DEBUG PURPOSES
    // [SerializeField] private List<Shots> _activeShots;

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
        activeShots = new Queue<Shots>();
        if (PlayerController.Instance) getOriginalValues();
    }

    public void setActiveShots(Shots shot)
    {
        if (!activeShots.Contains(shot))
        {
            if (activeShots.Count == GameManager.Instance.toleranceLevel) activeShots.Dequeue();
            activeShots.Enqueue(shot);
        }
        // DEBUG PURPOSES
        // _activeShots = new List<Shots>(activeShots);
    }

    public void setActiveShots(string shot)
    {
        Shots Shot = (Shots)Enum.Parse(typeof(Shots), shot);
        setActiveShots(Shot);
    }

    void getOriginalValues()
    {
        _originalSpeed = PlayerController.Instance.RunSpeed;
        _originalDoubleJumpIndex = PlayerController.Instance.TotalDoubleJumpCount;
        _originalGravity = PlayerController.Instance.GravityScale;
    }

    void revertToOriginal()
    {
        PlayerController.Instance.RunSpeed = _originalSpeed;
        PlayerController.Instance.TotalDoubleJumpCount = _originalDoubleJumpIndex;
        PlayerController.Instance.GravityScale = _originalGravity;
    }

    public void applyShotEffects()
    {
        if (SceneLoading.Instance.currentSceneType == SceneLoading.SceneType.Level)
        {
            foreach (Shots shot in activeShots)
            {            
                switch (shot)
                {
                    case Shots.None:
                        break;
                    case Shots.QuickWick:
                        PlayerController.Instance.RunSpeed *= 1.5f;
                        break;
                    case Shots.FloatFlicker:
                        PlayerController.Instance.TotalDoubleJumpCount += 1;
                        break;
                    case Shots.OverBurn:
                        PlayerController.Instance.GravityScale = 2;
                        break;
                    case Shots.SniffSpark:
                        // Add your effect
                        break;
                    case Shots.TripTrap:
                        // Add your effect
                        break;
                    case Shots.GoldGulp:
                        // Add your effect
                        break;
                    case Shots.AshGlide:
                        // Add your effect
                        break;
                    case Shots.HotDamn:
                        // Add your effect
                        break;
                    case Shots.NoctiDrop:
                        // Add your effect
                        break;
                    default:
                        Debug.LogWarning("Unknown shot effect.");
                        break;
                }      
            }

        }
    }

    

}
