using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    private bool _isPaused = false;
    [SerializeField] private GameObject _pauseMenu;

    void Start()
    {
        if(_pauseMenu) disablePause();
    }

    void Update()
    {
        if (UserInput.Instance && UserInput.Instance.menuToggleInput && _pauseMenu)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        _pauseMenu.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void disablePause()
    {
        _isPaused = false;
        _pauseMenu.SetActive(_isPaused);
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    public void QuitApp()
    {    Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
