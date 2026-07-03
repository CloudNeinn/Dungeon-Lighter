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
        PlaceInPosition();
    }

    void PlaceInPosition()
    {
        if(SceneLoading.Instance.departureSceneType == SceneLoading.SceneType.Hub) Place(_pointOfArrivalFromHub.transform.position);
        else if(SceneLoading.Instance.departureSceneType == SceneLoading.SceneType.Level) Place(_pointOfArrivalFromLevel.transform.position);
        else if(SceneLoading.Instance.departureSceneType == SceneLoading.SceneType.AbyssLevel) Place(_pointOfArrivalFromAbyssLevel.transform.position);
    }

    void Place(Vector2 position)
    {
        Vector3 oldPlayerPos = PlayerController.Instance.transform.position;
        Vector3 delta = (Vector3)position - oldPlayerPos;

        PlayerController.Instance.gameObject.transform.position = position;
        CameraController.Instance.WarpCamera(position, delta);
    }
}
