using UnityEngine;

public class Ember : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private bool _active;
    private float _emberResetTimeCounter;
    private bool _wasActive;

    void Start()
    {
        _wasActive = _active;
        ResetEmberTimer();
        SetEmbers();
    }

    void Update()
    {
        if (!_active && _emberResetTimeCounter > 0)
        {
            _emberResetTimeCounter -= Time.deltaTime;
        }
        else if (!_active && _emberResetTimeCounter < 0)
        {
            ActivateEmber();
            ResetEmberTimer();
        }
        if (_active == _wasActive)
            return;

        _wasActive = _active;

        SetEmbers();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("EmberBackTrigger"))
        {
            EmberBehind();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("EmberBackTrigger"))
        {
            EmberForward();
        }
    }

    void ResetEmberTimer()
    {
        _emberResetTimeCounter = EmberOvalOrbit.Instance.emberResetTime;
    }

    public void ActivateEmber()
    {
        _active = true;
    }

    public void DeactivateEmber()
    {
        _active = false;
    }

    public void SetEmbers()
    {
        if(_active)
        {
            _spriteRenderer.color = Color.white;
            EmberOvalOrbit.Instance.activeEmberCount += 1;
        }
        else
        {
            _spriteRenderer.color = Color.black;
            EmberOvalOrbit.Instance.activeEmberCount -= 1;
        }
    }

    void EmberBehind()
    {
        _spriteRenderer.sortingOrder = 4;
    }

    void EmberForward()
    {
        _spriteRenderer.sortingOrder = 6;
    }
}
