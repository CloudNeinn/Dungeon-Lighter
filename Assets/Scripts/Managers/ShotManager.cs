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

    [SerializeField] public Shots activeShot;

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
        getOriginalValues();
    }

    public void setActiveShot(Shots shot)
    {
        activeShot = shot;
    }

    public void setActiveShot(string shot)
    {
        activeShot = (Shots)Enum.Parse(typeof(Shots), shot);
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
            switch (activeShot)
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
                    PlayerControl.Instance.GravityScale *= 0.5f;
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
