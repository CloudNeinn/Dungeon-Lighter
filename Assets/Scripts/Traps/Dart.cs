using UnityEngine;

public class Dart : MonoBehaviour
{
    [SerializeField] private LayerMask _collisionMask;
    public float damage;
    public float speed;
    public Vector2 movementVector;
    public Rigidbody2D rigid;
    public bool canBeBlocked;
    public bool canBeRedirected;
    public Vector2 projectileVector;
    private Vector3 playerPosition;
    private Vector3 enemyPosition;
    private bool _justSpawned;

    void OnEnable()
    {
        _justSpawned = true;
    }

    void FixedUpdate()
    {
        _justSpawned = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //kill player ig
        if(_justSpawned) return;
        if (((1 << collision.gameObject.layer) & _collisionMask) == 0) return;
        ObjectPoolManager.ReturnObjectToPool(gameObject);
    }

    public Vector2 getVector()
    {
        Vector3 playerPosition = PlayerController.Instance.transform.position;
        Vector3 enemyPosition = transform.position;
        return new Vector2(playerPosition.x - enemyPosition.x, playerPosition.y - enemyPosition.y).normalized;
    }
}
