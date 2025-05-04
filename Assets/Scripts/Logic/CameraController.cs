using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public CinemachineFramingTransposer transposer {get; private set;}
    float startingScreenX;

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
        transposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if(transposer != null) startingScreenX = transposer.m_ScreenX;
    }

    public float getCameraScreenX()
    {
        return startingScreenX;
    }

    public void setCameraScreenX(float screenX)
    {
        transposer.m_ScreenX = screenX;
    }
}
