using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private float _typeSpeed = 0.05f;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject _bubble;
    private TextMeshProUGUI _textComponent;
    private string _fullText;
    [SerializeField] private string _jokeText;
    [SerializeField] private float _interacivityRadius;
    [SerializeField] private LayerMask _playerLayer;
    private GameObject _canvas;
    public RectTransform[] _dots; // Assign dot RectTransforms in inspector
    [SerializeField] private float _jumpPower = 10f; // Height of the jump
    [SerializeField] private float _duration = 0.5f; // Duration of one jump
    [SerializeField] private float _delay = 0.1f; // Delay between dots
    private Tween[] _dotTweens;
    private bool _indicatorActive = false;
    private GameObject _parent;
    private RectTransform _textTransform;
    private RectTransform _bubbleTransform;
    private bool facingRight;

    void Start()
    {
        _textComponent = GetComponentInChildren<TextMeshProUGUI>();
        _textTransform = _textComponent.gameObject.GetComponent<RectTransform>();
        _bubbleTransform = _textTransform.GetChild(0).GetComponent<RectTransform>();
        _fullText = _textComponent.text;
        _canvas = transform.GetChild(0).gameObject;
        _parent = transform.parent.gameObject;
        //_bubble = transform.GetChild(0).GetChild(0).gameObject;
        //_indicator = transform.GetChild(0).GetChild(1).gameObject;
        //_dots = _indicator.GetComponentsInChildren<RectTransform>();
        //_canvas.SetActive(false);
        //InteractionIndicator(true);
        facingRight = _parent != null && _parent.transform.localScale.x < 0;
    }
    void Update()
    {
        if (inInteractivityRadius() && isLookingForward())
        {
            if (PlayerControl.Instance._use2Input)
            {
                InteractionIndicator(false);
                StartCoroutine(TypeText(_fullText));
            }
        }
        else InteractionIndicator(true);

        FlipDialogueBubble();
    }

    bool inInteractivityRadius()
    {
        return Physics2D.OverlapCircle(transform.position, _interacivityRadius, _playerLayer); 
    }

    bool isLookingForward()
    {
        if (_parent == null && transform.position.x * transform.localScale.x < PlayerControl.Instance.transform.position.x * transform.localScale.x) return true;
        else if (_parent != null && _parent.transform.position.x * _parent.transform.localScale.x < PlayerControl.Instance.transform.position.x * _parent.transform.localScale.x) return true;
        else return false;
    }

    void FlipDialogueBubble()
    {
        if (_parent.transform.localScale.x == -1 && facingRight)
        {
        Debug.Log("called1");
            facingRight = false;
            _textTransform.pivot = new Vector2(0.5f, 0.5f);
            _textTransform.localScale = new Vector3(-1, _textTransform.localScale.y, _textTransform.localScale.z);
            _textTransform.pivot = new Vector2(1, 0);
            _bubbleTransform.localScale = new Vector3(-1, _bubbleTransform.localScale.y, _bubbleTransform.localScale.z);
        }
        else if (_parent.transform.localScale.x == 1 && !facingRight)
        {
        Debug.Log("called2");
            facingRight = true;
            _textTransform.pivot = new Vector2(0.5f, 0.5f);
            _textTransform.localScale = new Vector3(1, _textTransform.localScale.y, _textTransform.localScale.z);
            _textTransform.pivot = new Vector2(0, 0);
            _bubbleTransform.localScale = new Vector3(1, _bubbleTransform.localScale.y, _bubbleTransform.localScale.z);
        }
    }

    void InteractionIndicator(bool turnOn)
    {
        if (turnOn && !_indicatorActive) // Only if turning on and not already on
        {
            _indicatorActive = true;
            _bubble.SetActive(false);
            _indicator.SetActive(true);
            
            _dotTweens = new Tween[_dots.Length];
            for (int i = 0; i < _dots.Length; i++)
            {
                int idx = i;
                _dotTweens[idx] = _dots[idx].DOAnchorPosY(_jumpPower, _duration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetDelay(idx * _delay);
            }
        }
        else if (!turnOn && _indicatorActive) // Only if turning off and was on
        {
            _indicator.SetActive(false);
            _indicatorActive = false;
            _bubble.SetActive(true);
            if (_dotTweens != null)
            {
                for (int i = 0; i < _dotTweens.Length; i++)
                {
                    if (_dotTweens[i] != null && _dotTweens[i].IsActive())
                        _dotTweens[i].Kill();
                }
                _dotTweens = null;
            }
        }
    }


    void OnDrawGizmos()
    {
        _parent = transform.parent.gameObject;
        Gizmos.color = Color.green;
        if (_parent == null) Gizmos.DrawWireSphere(transform.position, _interacivityRadius);
        else Gizmos.DrawWireSphere(_parent.transform.position, _interacivityRadius);
    }
    
    IEnumerator TypeText(string text)
    {
        InteractionIndicator(false);
        //_textComponent.ForceMeshUpdate();

        _textComponent.text = "";

        for (int i = 0; i <= text.Length; i++)
        {
            _textComponent.text = text.Substring(0, i);
            yield return new WaitForSeconds(_typeSpeed);
            if (!_bubble.activeSelf)
            {
                _textComponent.text = "";
                break;
            }
        }
    }
}