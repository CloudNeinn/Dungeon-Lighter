using UnityEngine;

public class ComfyChair : MonoBehaviour
{
    public static ComfyChair Instance;
    [SerializeField] private float _radius;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private LayerMask _playerLayer;

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
    }
    
    void Update()
    {
        if(UserInput.Instance.use1Input && InRange()) Sit();
    }

    void Sit()
    {
        DataPersistanceManager.Instance.SaveGame();
    }

    bool InRange()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + _offset, _radius, _playerLayer); 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + _offset, _radius);
    }
}
