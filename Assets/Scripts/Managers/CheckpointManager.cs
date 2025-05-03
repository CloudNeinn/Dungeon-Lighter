using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance; 
    public Vector3 currentCheckpointPosition;
    public checkpoint currentCheckpoint;
    public checkpoint[] checkIfSaved;
    public bool checkpMenuOpen;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        checkIfSaved = FindObjectsOfType<checkpoint>();
        if(currentCheckpoint != null) currentCheckpoint.isActivated = true;
    }

    void Update()
    {
        
    }

    public void updateCheckpoint()
    {
        foreach(checkpoint obj in checkIfSaved)
        {
            if(obj.isActivated)
            {
                checkpMenuOpen = obj.menuOpen;
                obj.checkpointIsSaved = false;
                obj.isActivated = false;
                Debug.Log("Found an object with true myBoolValue: " + obj.name);
            }
        }        
    }
}
