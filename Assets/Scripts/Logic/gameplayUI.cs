using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class gameplayUI : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject pauseButton;
    public GameObject deathScreen;
    public GameObject settingsMenu;
    public GameObject checkpointMenu;
    public GameObject CHB;
    public Rigidbody2D charRigid;
    public DataPersistanceManager dataPerMan;
    void Start()
    {
        charRigid = PlayerControl.Instance.gameObject.GetComponent<Rigidbody2D>();
        dataPerMan = GameObject.FindObjectOfType<DataPersistanceManager>();
        pauseMenu = GameObject.Find("Pause Menu");
        settingsMenu =  GameObject.Find("Settings Menu");
        checkpointMenu =  GameObject.Find("CheckpointMenu");
        pauseButton = GameObject.Find("Pause button");
        deathScreen =  GameObject.Find("Death screen");
        CHB =  GameObject.Find("CharachterHealthBar");
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        deathScreen.SetActive(false);
        checkpointMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(pauseMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            turnOffPause();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)
        && !(checkpointMenu.activeSelf))
        {
            Pause();
        }
        if(checkpointMenu.activeSelf && 
        Input.GetKeyDown(KeyCode.Escape)) 
        {
            checkpointMenu.SetActive(false);
        }
        if(settingsMenu.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);   
        }

    }

    public void Pause()
    {
        CHB.SetActive(false);
        pauseMenu.SetActive(true);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
    }

    public void deathScreenMenu()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void turnOffPause()
    {
        CHB.SetActive(true);
        pauseMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
    }

    public void Respawn()
    {
        PlayerControl.Instance.transform.position = CheckpointManager.Instance.currentCheckpoint.transform.position + Vector3.up;
        charRigid.bodyType = RigidbodyType2D.Dynamic;
        deathScreen.SetActive(false);
        turnOffPause();
    }

    public void SettingsInGame()
    {
        if(!settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }
        else if(settingsMenu.activeSelf)
        {
            settingsMenu.SetActive(false);
            pauseMenu.SetActive(true);          
        }
    }
}
