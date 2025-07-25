using System.Collections;
using UnityEngine;
using TMPro;

public class SkeletonNPC : MonoBehaviour
{
    [SerializeField] private float _typeSpeed = 0.05f;
    private TextMeshProUGUI _textComponent;
    private string _fullText;
    [SerializeField] private string _jokeText;
    [SerializeField] private float _interacivityRadius;
    [SerializeField] private LayerMask _playerLayer;
    private GameObject _canvas;
    [SerializeField] private float _jokeTime;
    [SerializeField] private float _jokeTimeCooldown;
    private bool jokeCalled;

    void Start()
    {
        _textComponent = GetComponentInChildren<TextMeshProUGUI>();
        _fullText = _textComponent.text;
        _canvas = transform.GetChild(0).gameObject;
        _canvas.SetActive(false);
        resetText();
    }

    void Update()
    {
        if (inInteractivityRadius() && transform.position.x * transform.localScale.x < PlayerControl.Instance.transform.position.x * transform.localScale.x)
        {
            _canvas.SetActive(true);
            if (PlayerControl.Instance._use1Input)
            {
                StartCoroutine(TypeText(_fullText));
            }

            if (_jokeTimeCooldown < 0 && !jokeCalled)
            {
                jokeCalled = true;
                StartCoroutine(TypeText(_jokeText));
            }
            else _jokeTimeCooldown -= Time.deltaTime;
        }
        else
        {
            resetText();
            _canvas.SetActive(false);
        }
    }

    void resetText()
    {
        jokeCalled = false;
        _textComponent.text = ". . .";
        _jokeTimeCooldown = _jokeTime;
    }

    bool inInteractivityRadius()
    {
        return Physics2D.OverlapCircle(transform.position, _interacivityRadius, _playerLayer); 
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _interacivityRadius);
    }
    
    IEnumerator TypeText(string text)
    {
        _textComponent.text = "";
        
        for (int i = 0; i <= text.Length; i++)
        {
            _textComponent.text = text.Substring(0, i);
            yield return new WaitForSeconds(_typeSpeed);
        }
    }
}