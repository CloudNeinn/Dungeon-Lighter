using UnityEngine;

public class ChainGrab : MonoBehaviour
{
    [SerializeField] private float _grabRadius;
    [SerializeField] private Vector2 _grabOffset;
    [SerializeField] private LayerMask _chainLayer;
    [SerializeField] private GameObject _grabbedChain;
    [SerializeField] private GameObject _chainParent;
    [SerializeField] private HingeJoint2D _swingJoint;
    [SerializeField] private bool _isGrabbing;
[SerializeField] private float _climbCooldown;
[SerializeField] private float _climbCooldownCounter;

    void Start()
    {
        Physics2D.IgnoreLayerCollision(
            gameObject.layer,
            LayerMask.NameToLayer("Chains"));
    }

void Update()
{
    if(UserInput.Instance.use1Input && ChainInRadius() && !_isGrabbing)
    {
        GrabChain();
    }

    if(UserInput.Instance.jumpInput && _isGrabbing)
    {
        ReleaseChain();
    }

    if(_climbCooldownCounter > 0) _climbCooldownCounter -= Time.deltaTime;

    if(_isGrabbing && _climbCooldownCounter <= 0)
    {
        if(UserInput.Instance.moveInput.y > 0)
        {
            ClimbChain(-1);
            _climbCooldownCounter = _climbCooldown;
        }
        else if(UserInput.Instance.moveInput.y < 0)
        {
            ClimbChain(1);
            _climbCooldownCounter = _climbCooldown;
        }
    }
}

    bool ChainInRadius()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            (Vector2)transform.position + new Vector2(_grabOffset.x * Mathf.Sign(transform.localScale.x), _grabOffset.y), 
            _grabRadius, 
            _chainLayer);

        if (colliders.Length == 0) return false;

        Collider2D closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D col in colliders)
        {
            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = col;
            }
        }

        _grabbedChain = closest.gameObject;
        _chainParent = _grabbedChain.transform.parent.gameObject;
        return true;
    }

    void GrabChain()
    {
        _isGrabbing = true;
        _swingJoint = PlayerController.Instance.gameObject.AddComponent<HingeJoint2D>();
        _swingJoint.connectedBody = _grabbedChain.GetComponent<Rigidbody2D>();
        _swingJoint.autoConfigureConnectedAnchor = false;
        _swingJoint.connectedAnchor = Vector2.zero;
        _swingJoint.anchor = Vector2.zero;
    }

    void ReleaseChain()
    {
        _isGrabbing = false;
        Destroy(_swingJoint);
        _swingJoint = null;
        _grabbedChain = null;
        _chainParent = null;
    }

    void ClimbChain(int direction)
    {
        int currentIndex = _grabbedChain.transform.GetSiblingIndex();
        int targetIndex = currentIndex + direction;

        if(targetIndex < 0 || targetIndex >= _chainParent.transform.childCount) return;

        _grabbedChain = _chainParent.transform.GetChild(targetIndex).gameObject;
        _swingJoint.connectedBody = _grabbedChain.GetComponent<Rigidbody2D>();
        
        // Snap player to new link position
        transform.position = _grabbedChain.transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(_grabOffset.x * Mathf.Sign(transform.localScale.x), _grabOffset.y), _grabRadius);
    }
}