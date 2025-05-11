using System.Collections;
using UnityEngine;

public class DivineBeing : MonoBehaviour
{
    [SerializeField] private float triggerRadius = 5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Animator animator;
    [SerializeField] private string animationStateName = "Attack"; // exact name of the state, not the trigger
    [SerializeField] private string animationTriggerName = "Attack"; // trigger to transition to attack
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip attackSound;

    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered) return;

        Collider2D player = Physics2D.OverlapCircle(transform.position, triggerRadius, playerLayer);
        if (player != null)
        {
            hasTriggered = true;
            animator.SetTrigger(animationTriggerName);
            StartCoroutine(WaitForAttackAnimationThenQuit());
        }
    }

    private IEnumerator WaitForAttackAnimationThenQuit()
    {
        // Wait until the animator transitions to the attack state
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName))
        {
            yield return null;
        }

        // Then wait until the animation finishes
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animLength);

        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void PlayAttackSound()
    {
        if (audioSource && attackSound)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }
    
    // Visualize the detection radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
