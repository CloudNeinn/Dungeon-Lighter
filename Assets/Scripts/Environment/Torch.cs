using System.Diagnostics;
using System.Collections;
using UnityEngine;

public class Torch : MonoBehaviour
{
    [SerializeField] private bool _litOnStart;
    [SerializeField] private Animator _torchAnimator;
    [SerializeField] private GameObject _lightObject;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private float _torchLightRadius;
    [SerializeField] private bool _isLit;
    [SerializeField] private bool _isFlare;
    [SerializeField] private float _timeToFlare;
    [SerializeField] private float _timeToFlareCounter;
    [SerializeField] private float _flareFlightRadius;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private GameObject _flare;
    [SerializeField] private float _flareOutForce;
    [SerializeField] private float _flareCooldownTime;
    [SerializeField] private float _flareDrag;
    [SerializeField] private float _flareDragDuration;


    void Start()
    {
        _isLit = _litOnStart;
        _lightObject = gameObject.transform.GetChild(0).gameObject;
        if(_litOnStart) LightUpTorch();
        _flare.SetActive(false);
    }

    void Update()
    {
        if(playerInRange() && UserInput.Instance.use1Input)
        {
            LightUpTorch();
        }
        if(playerInRange() && UserInput.Instance.use1Pressed && !_isFlare)
        {
            _timeToFlareCounter -= Time.deltaTime;
        }
        else ResetTime();

        if(_timeToFlareCounter <= 0)
        {
            _isFlare = true;
            ResetTime();
        }
        
        if(_isFlare && (UserInput.Instance.use1Input || UserInput.Instance.jumpInput)) 
        {
            FlareOut();
            _isFlare = false;
        }

        if(_isFlare) FlareIn();

    }

    void ResetTime()
    {
        _timeToFlareCounter = _timeToFlare;
    }

    void FlareIn()
    {
        PlayerController.Instance.gameObject.SetActive(false);
        _flare.SetActive(true);
        _flare.transform.RotateAround(transform.position, Vector3.forward, _rotationSpeed * Time.deltaTime);
    }

    void FlareOut()
    {
        PlayerController.Instance.transform.position = _flare.transform.position;
        _flare.SetActive(false);
        PlayerController.Instance.gameObject.SetActive(true);
        PlayerController.Instance.PlayerRigidbody.AddForce(GetFlareDirection() * _flareOutForce, ForceMode2D.Impulse);
        PlayerController.Instance.SetMoveCooldown(_flareCooldownTime);
        StartCoroutine(FlareDrag());
    }

    Vector2 GetFlareDirection()
    {
        return new Vector2(_flare.transform.position.x - transform.position.x, _flare.transform.position.y - transform.position.y).normalized;
    }

    void LightUpTorch()
    {
        _lightObject.SetActive(true);
        _torchAnimator.SetTrigger("Lit");
        _isLit = true; 
    }

    Collider2D playerInRange()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position, _torchLightRadius, _playerLayer);
    }

    IEnumerator FlareDrag()
    {
        float originalDrag = PlayerController.Instance.PlayerRigidbody.linearDamping;
        PlayerController.Instance.PlayerRigidbody.linearDamping = _flareDrag; // e.g. 5f
        yield return new WaitForSeconds(_flareDragDuration); // e.g. 0.2f
        PlayerController.Instance.PlayerRigidbody.linearDamping = originalDrag;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position, _torchLightRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere((Vector2)transform.position, _flareFlightRadius);
    }
}
