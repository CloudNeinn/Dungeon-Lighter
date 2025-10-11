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
    [SerializeField] private float originalSpeed;
    [SerializeField] private int originalDoubleJumpIndex;
    [SerializeField] private float originalGravity;
    [SerializeField] public Queue<Shots> activeShots;

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
        if (PlayerControl.Instance) getOriginalValues();
    }

    public void setActiveShots(Shots shot)
    {
        if (!activeShots.Contains(shot))
        {
            if (activeShots.Count == GameManager.Instance._toleranceLevel) activeShots.Dequeue();
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
        originalSpeed = PlayerControl.Instance.RunSpeed;
        originalDoubleJumpIndex = PlayerControl.Instance.TotalDoubleJumpCount;
        originalGravity = PlayerControl.Instance.GravityScale;
    }

    void revertToOriginal()
    {
        PlayerControl.Instance.RunSpeed = originalSpeed;
        PlayerControl.Instance.TotalDoubleJumpCount = originalDoubleJumpIndex;
        PlayerControl.Instance.GravityScale = originalGravity;
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
                        PlayerControl.Instance.RunSpeed *= 1.5f;
                        break;
                    case Shots.FloatFlicker:
                        PlayerControl.Instance.TotalDoubleJumpCount += 1;
                        break;
                    case Shots.OverBurn:
                        PlayerControl.Instance.GravityScale = 2;
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
