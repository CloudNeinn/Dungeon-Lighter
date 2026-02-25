using System.Collections;
using UnityEngine;

public class DivineBeing : MonoBehaviour
{
    [SerializeField] private float _triggerRadius = 5f;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _animationStateName = "Attack"; // exact name of the state, not the trigger
    [SerializeField] private string _animationTriggerName = "Attack"; // trigger to transition to attack
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _attackSound;

    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered) return;

        Collider2D player = Physics2D.OverlapCircle(transform.position, _triggerRadius, _playerLayer);
        if (player != null)
        {
            hasTriggered = true;
            _animator.SetTrigger(_animationTriggerName);
            StartCoroutine(WaitForAttackAnimationThenQuit());
        }
    }

    private IEnumerator WaitForAttackAnimationThenQuit()
    {
        // Wait until the _animator transitions to the attack state
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationStateName))
        {
            yield return null;
        }

        // Then wait until the animation finishes
        float animLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void PlayAttackSound()
    {
        if (_audioSource && _attackSound)
        {
            _audioSource.PlayOneShot(_attackSound);
        }
    }
    
    // Visualize the detection radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _triggerRadius);
    }
}
