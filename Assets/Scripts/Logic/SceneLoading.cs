using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using Cinemachine;
using System.Diagnostics;

public class SceneLoading : MonoBehaviour
{
    public enum SceneType 
    { 
        Hub,
        Level 
    }

    public static SceneLoading Instance;
    [field: SerializeField] public SceneType currentSceneType {get; private set;}
    public GameObject loadingScreen  {get; private set;}
    public Image loadingBar  {get; private set;}
    [field: SerializeField] public List<GameObject> grids {get; private set;}
    [SerializeField] private LayerMask _detectColliderMask;
    [SerializeField] private Vector3 _returnDoorPosition;
    [SerializeField] private Animator _loadingScreenAnimator;
    [SerializeField] private GameObject _loadingScreen;
    private bool _closeAnimationFinished;
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
        SceneManager.sceneLoaded += onSceneLoad;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= onSceneLoad;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.Debug.Log("destroyed");
    }

    void Start()
    {
        grids = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Grid").ToList();
    }

    //May be useful in the  future
    // private void OnEnable()
    // {
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }

    // private void OnDisable()
    // {
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerController.Instance.Respawn();
        CameraController.Instance.SetConfiner();
    }

    public void StartGame()
    {
        LoadScene(1);
        LoadScene(2, true);
    }

    public void LoadScene(int sceneID, bool isAdditive = false)
    {
        StartCoroutine(LoadSceneAsync(sceneID, isAdditive));
    }

    public void ReloadScene()
    {
        LoadScene(GetCurrentSceneID());
    }
    
    public void SetCloseAnimationFinished()
    {
        _closeAnimationFinished = true;
    }

    IEnumerator LoadSceneAsync(int sceneID, bool isAdditive)
    {
        _loadingScreen.SetActive(true);
        _closeAnimationFinished = false;
        _loadingScreenAnimator.SetTrigger("Close");
    
        yield return new WaitUntil(() => _closeAnimationFinished);

        AsyncOperation operation;
        if (isAdditive) operation = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Additive);
        else operation = SceneManager.LoadSceneAsync(sceneID);
        
        while (!operation.isDone)
        {
            yield return null;
        }
        
        _loadingScreenAnimator.SetTrigger("Open");
    }

    public int GetCurrentSceneID()
    {
        Collider2D collider = Physics2D.OverlapCircle(PlayerController.Instance.transform.position, 10f, _detectColliderMask);
        return collider.gameObject.scene.buildIndex;
    }

    public string GetCurrentSceneName()
    {
        Collider2D collider = Physics2D.OverlapCircle(PlayerController.Instance.transform.position, 10f, _detectColliderMask);
        return collider.gameObject.scene.name;
    }

    void onSceneLoad(Scene scene, LoadSceneMode mode)
    {
        setCurrentSceneType(scene);

        if (ShotManager.Instance != null)
        {
            ShotManager.Instance.applyShotEffects();
        }

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnSceneLoaded();
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.getNotesUI();
        }

        if (currentSceneType == SceneType.Hub)
        {
            if(PlayerController.Instance != null) PlayerController.Instance.transform.position = _returnDoorPosition;
        }

        StartCoroutine(ResetCameraConfiner());
    }

    IEnumerator ResetCameraConfiner()
    {
        yield return null;

        var confiner2D = FindFirstObjectByType<CinemachineConfiner2D>();
        if (confiner2D != null)
        {
            confiner2D.InvalidateCache();
        }

    }

    void setCurrentSceneType(Scene scene)
    {
        if (scene.name == "Hub" || scene.name == "PersistentObjects")
        {
            currentSceneType = SceneType.Hub;
        }
        else
        {
            currentSceneType = SceneType.Level;
        }
    }

    public void SetReturnDoor(Vector3 door)
    {
        _returnDoorPosition = door;
    }

    IEnumerator SetPlayerPositionWhenReady()
    {
        while (PlayerController.Instance == null)
        {
            yield return null;
        }
        PlayerController.Instance.transform.position = _returnDoorPosition;
    }
}
