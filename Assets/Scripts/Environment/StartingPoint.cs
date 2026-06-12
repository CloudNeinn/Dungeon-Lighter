using UnityEngine;

public class StartingPoint : MonoBehaviour
{
    [SerializeField] private GameObject _pointOfArrivalFromHub;
    [SerializeField] private GameObject _pointOfArrivalFromLevel;
    [SerializeField] private GameObject _pointOfArrivalFromAbyssLevel;
    public static StartingPoint Instance;
    private void Awake()
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
        PlacePlayerInPosition();
    }

    void PlacePlayerInPosition()
    {
        if(SceneLoading.Instance.departureSceneType == SceneLoading.SceneType.Hub) PlayerController.Instance.gameObject.transform.position = _pointOfArrivalFromHub.transform.position;
        else if(SceneLoading.Instance.departureSceneType == SceneLoading.SceneType.Level) PlayerController.Instance.gameObject.transform.position = _pointOfArrivalFromLevel.transform.position;
        else if(SceneLoading.Instance.departureSceneType == SceneLoading.SceneType.AbyssLevel) PlayerController.Instance.gameObject.transform.position = _pointOfArrivalFromAbyssLevel.transform.position;
    }
}
