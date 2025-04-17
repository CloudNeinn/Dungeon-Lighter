using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoading : MonoBehaviour
{
    public static SceneLoading Instance;
    public GameObject loadingScreen;
    public Image loadingBar;
    [field: SerializeField] public List<GameObject> Grids;
    [SerializeField] private LayerMask _detectColliderMask;
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    
    void Start()
    {
        Grids = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "Grid").ToList();
    }

    public void StartGame()
    {
        LoadScene(1);
    }

    public void LoadScene(int sceneID, bool isAdditive = false)
    {
        StartCoroutine(LoadSceneAsync(sceneID, isAdditive));
    }

    public void ReloadScene()
    {
        LoadScene(GetCurrentSceneID());
    }

    IEnumerator LoadSceneAsync(int sceneID, bool isAdditive)
    {
        AsyncOperation operation;
        if(isAdditive) operation = SceneManager.LoadSceneAsync(sceneID, LoadSceneMode.Additive);
        else operation = SceneManager.LoadSceneAsync(sceneID);
        loadingScreen.SetActive(true);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingBar.fillAmount = progress;
            yield return null;
        }

        if(operation.isDone)
        {
            loadingScreen.SetActive(false);
            loadingBar.fillAmount = 0;
        }
    }

    public int GetCurrentSceneID()
    {
        Collider2D collider = Physics2D.OverlapCircle(PlayerControl.Instance.transform.position, 10f, _detectColliderMask);
        return collider.gameObject.scene.buildIndex;
    }

    public string GetCurrentSceneName()
    {
        Collider2D collider = Physics2D.OverlapCircle(PlayerControl.Instance.transform.position, 10f, _detectColliderMask);
        return collider.gameObject.scene.name;
    }
}
