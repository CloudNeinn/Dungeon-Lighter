using UnityEngine;

public class Flame : MonoBehaviour, ISequenceable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private int _trapID;
    [SerializeField] private bool _vertical;
    [SerializeField] private int _animationFiringNumberTotal;
    [SerializeField] private int _animationFiringNumber;

    public void Activate()
    {
        _animator.SetBool("isFiring", true);
        _animationFiringNumber = _animationFiringNumberTotal;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // damage logic
        }
    }

    public void FiringAnimationEnd()
    {
        _animationFiringNumber -= 1;
        if(_animationFiringNumber == 0) _animator.SetBool("isFiring", false);
    }
}