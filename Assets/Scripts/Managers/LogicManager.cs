using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField] GameObject pauseMenu;

    void Start()
    {
        if(pauseMenu) disablePause();
    }

    void Update()
    {
        if (UserInput.Instance && UserInput.Instance.MenuToggleInput && pauseMenu)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void disablePause()
    {
        isPaused = false;
        pauseMenu.SetActive(isPaused);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void QuitApp()
    {    Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
