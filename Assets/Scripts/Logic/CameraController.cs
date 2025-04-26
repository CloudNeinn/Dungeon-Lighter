using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer transposer;
    private float startingScreenX;
    [SerializeField] private float barScreenX = 0.4f;

    void Start()
    {
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        startingScreenX = transposer.m_ScreenX;
    }

    void Update()
    {
        if(Bar.Instance._barUIActive && transposer.m_ScreenX == startingScreenX) setCameraScreenX(barScreenX);
        else if(!Bar.Instance._barUIActive) setCameraScreenX(startingScreenX);
    }

    void setCameraScreenX(float screenX)
    {
        transposer.m_ScreenX = screenX;
    }
}
