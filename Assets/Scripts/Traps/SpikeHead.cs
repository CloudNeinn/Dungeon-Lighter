using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeHead : MonoBehaviour
{
    enum SpikeHeadDirection
    {
        Up,
        Down,
        Left,
        Right

    }
    static readonly Vector3[] vectorDir = new Vector3[] {
        Vector3.up,
        Vector3.down,
        Vector3.left,
        Vector3.right
    };

    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask raycastLayer;
    [SerializeField] private SpikeHeadDirection movementDirection; 
    [SerializeField] private float attackSpeed = 5f;
    [SerializeField] private float returnSpeed = 1f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D coll;
    private bool inInitialPosition = true;
    private bool ready = true;
    private Vector3 initialPosition;
    [SerializeField] private float cooldownTime = 2f;
    private float cooldownTimeCounter = 0f;

    // Start is called before the first frame update
    void Start()
    {  
       initialPosition = transform.position; 
       cooldownTimeCounter = cooldownTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInSight() && ready) Attack();
        if(checkIfCollidingWithWall()) ready = false;

        if(!ready && cooldownTimeCounter > 0f)
        {
            cooldownTimeCounter -= Time.deltaTime;
        }
        else if(!ready && cooldownTimeCounter <= 0f)
        {
            rb.velocity = Vector3.zero;
            if (Vector3.Distance(transform.position, initialPosition) > .1f)
            {
                rb.velocity = -vectorDir[(int)movementDirection] * returnSpeed;
            }
            else
            {
                rb.velocity = Vector3.zero;
                ready = true;
                inInitialPosition = true;
                cooldownTimeCounter = cooldownTime;
            }
        }
    }

    void Attack()
    {
        inInitialPosition = false;
        rb.velocity = vectorDir[(int)movementDirection] * attackSpeed;
    }

    bool playerInSight()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, vectorDir[(int)movementDirection], 20f, raycastLayer);
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            return true;
        }
        else return false;
    }

    bool checkIfCollidingWithWall()
    {
        return Physics2D.OverlapBox(transform.position + vectorDir[(int)movementDirection] * .02f, coll.bounds.size, 0f, raycastLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, vectorDir[(int)movementDirection] * 20f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position + vectorDir[(int)movementDirection] * .02f, coll.bounds.size);
    }
}
