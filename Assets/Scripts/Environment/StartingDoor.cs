using UnityEngine;

public class StartingDoor : MonoBehaviour
{
    public static StartingDoor Instance;
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
        PlayerController.Instance.gameObject.transform.position = this.transform.position;
    }
}
