using UnityEngine;

public class ComfyChair : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private LayerMask _playerLayer;
    
    void Start()
    {
        
    }

    void Update()
    {
        if(UserInput.Instance.use1Pressed && InRange()) Sit();
    }

    void Sit()
    {
        DataPersistanceManager.Instance.SaveGame();
        Debug.Log("hello");
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
