using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private int _direction;
    [SerializeField] private float _moveForce;

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController.Instance.PlayerRigidbody.AddForce(_moveForce * _direction * Vector2.right, ForceMode2D.Force);
        }
    }
}
