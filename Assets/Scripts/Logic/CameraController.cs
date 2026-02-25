using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    public CinemachineFramingTransposer transposer {get; private set;}
    private float _startingScreenX;

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
        transposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if(transposer != null) _startingScreenX = transposer.m_ScreenX;
    }

    public float getCameraScreenX()
    {
        return _startingScreenX;
    }

    public void setCameraScreenX(float screenX)
    {
        transposer.m_ScreenX = screenX;
    }
}
