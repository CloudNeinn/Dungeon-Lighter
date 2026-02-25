using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private bool _litOnStart;
    [SerializeField] private Animator _torchAnimator;
    [SerializeField] private GameObject _lightObject;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _torchLightRadius;
    void Start()
    {
        _lightObject = gameObject.transform.GetChild(0).gameObject;
        if(_litOnStart) LightUpTorch();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange() && PlayerController.Instance.use1Input)
        {
            LightUpTorch();
        }
    }

    void LightUpTorch()
    {
        _lightObject.SetActive(true);
        _torchAnimator.SetTrigger("Lit");       
    }

    Collider2D playerInRange()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position, _torchLightRadius, _playerLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position, _torchLightRadius);
    }
}
