using UnityEngine;

public class CameraConfiner : MonoBehaviour
{
    public static CameraConfiner Instance;
    [SerializeField] private Collider2D _collider;
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
        _collider = this.GetComponent<Collider2D>();
    }

    public Collider2D ReturnCollider()
    {
        return _collider;
    }
}
