using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<int> shootTrapEvent; 
    public static event Action<string> openDoorEvent; 
    public static event Action<string> closeDoorEvent; 
    public static event Action swithcEnemyPlaneEvent; 

    void Update()
    {
        
    }

    public static void ShootTrap(int trapID)
    {
        shootTrapEvent?.Invoke(trapID);
    }

    public static void OpenDoor(string doorID)
    {
        openDoorEvent?.Invoke(doorID);
    }

    public static void CloseDoor(string doorID)
    {
        closeDoorEvent?.Invoke(doorID);
    }

    public static void SwitchPlane()
    {
        swithcEnemyPlaneEvent?.Invoke();
    }
}
