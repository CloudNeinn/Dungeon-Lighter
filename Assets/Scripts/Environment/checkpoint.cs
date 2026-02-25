using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    [field: SerializeField] public string id { get; private set; }
    
    [ContextMenu("Generate checkpoint id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    public bool checkpointIsSaved;
    public GameObject popUpSave;
    public GameObject popUpMenu;
    public bool isActivated;
    public DataPersistanceManager dataPerMan;
    private gameplayUI gameUI;
    public Vector3 checkPosition;
    public bool isHealed;
    public bool menuOpen;
    public GameObject checkpMenu;

    // Start is called before the first frame update
    void Start()
    {
        popUpSave = transform.GetChild(0).gameObject;
        popUpMenu = transform.GetChild(1).gameObject;
        dataPerMan = GameObject.FindFirstObjectByType<DataPersistanceManager>();
        checkpMenu = GameObject.Find("CheckpointMenu");
        gameUI = GameObject.FindFirstObjectByType<gameplayUI>(); 
        if(PlayerController.Instance.transform.position.x <= transform.position.x + 3 
        && PlayerController.Instance.transform.position.x >= transform.position.x - 3
        && PlayerController.Instance.transform.position.y <= transform.position.y + 3 
        && PlayerController.Instance.transform.position.y >= transform.position.y - 3)
        {
            CheckpointManager.Instance.updateCheckpoint();
            CheckpointManager.Instance.currentCheckpointPosition = transform.position;
            CheckpointManager.Instance.currentCheckpoint = this;
            checkpointIsSaved = true;
            dataPerMan.SaveGame();
        }
        //popUpMenu.SetActive(false);
        //checkpMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        checkPosition = PlayerController.Instance.transform.position;
        if(PlayerController.Instance.transform.position.x <= transform.position.x + 4 
        && PlayerController.Instance.transform.position.x >= transform.position.x - 4
        && PlayerController.Instance.transform.position.y <= transform.position.y + 4 
        && PlayerController.Instance.transform.position.y >= transform.position.y - 4)
        {
            if(PlayerController.Instance.use2Input && !menuOpen 
            /*&& isActivated*/ && Time.timeScale != 0) // Input.GetKeyDown(KeyCode.F)
            {        
                menuOpen = true;
                checkpMenu.SetActive(menuOpen);
            }
            else if(PlayerController.Instance.use2Input && menuOpen) // Input.GetKeyDown(KeyCode.F)
            {
                menuOpen = false;
                checkpMenu.SetActive(menuOpen);
            }
            //show e popup icon
            if(checkpointIsSaved && PlayerController.Instance.use1Input && !isHealed) // Input.GetKeyDown(KeyCode.E)
            {
                gameUI.Respawn();
                isHealed = true;
            }
            else if(PlayerController.Instance.use1Input && !checkpointIsSaved) // Input.GetKeyDown(KeyCode.E)
            {
                //save checkpoint
                CheckpointManager.Instance.updateCheckpoint();
                CheckpointManager.Instance.currentCheckpointPosition = transform.position;
                CheckpointManager.Instance.currentCheckpoint = this;
                checkpointIsSaved = true;
                isActivated = true;
                dataPerMan.SaveGame();
            }
            
        }
        else if(isActivated)
        {
            checkpointIsSaved = false;
            isHealed = false;
            menuOpen = false;
            checkpMenu.SetActive(false);
        }
        
        if(PlayerController.Instance.transform.position.x <= transform.position.x + 8 
        && PlayerController.Instance.transform.position.x >= transform.position.x - 8
        && PlayerController.Instance.transform.position.y <= transform.position.y + 8 
        && PlayerController.Instance.transform.position.y >= transform.position.y - 8)
        {
            popUpSave.SetActive(true);
            /*if(isActivated)*/ popUpMenu.SetActive(true);
        }
        else 
        {
            popUpMenu.SetActive(false);
            popUpSave.SetActive(false);
        }
    }
}
