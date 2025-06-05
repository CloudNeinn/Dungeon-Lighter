using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    private InteractiveUIController[] _popUps;
    [SerializeField] private bool _popUpActive;

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

    public void getNotesUI()
    {
        _popUps = Object.FindObjectsOfType<InteractiveUIController>();
    }

    public void changeState()
    {
        _popUpActive = _popUps.Any(popup => popup.isActive());;
        if(!_popUpActive) CameraController.Instance.setCameraScreenX(CameraController.Instance.getCameraScreenX());
    }
}
