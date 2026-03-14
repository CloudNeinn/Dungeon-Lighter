using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Diagnostics;
using System.Drawing;

public class DialogueBubble : MonoBehaviour
{
    [SerializeField] private float _typeSpeed = 0.05f;
    [SerializeField] private GameObject _indicator;
    [SerializeField] private GameObject _dialogueDoneIndicator;
    [SerializeField] private GameObject _bubble;
    private TextMeshProUGUI _textComponent;
    private string _fullText;
    [SerializeField] private string _jokeText;
    [SerializeField] private float _interacivityRadius;
    [SerializeField] private LayerMask _playerLayer;
    private GameObject _canvas;
    [field: SerializeField] public RectTransform[] dots {get; private set;} // Assign dot RectTransforms in inspector
    [SerializeField] private float _jumpPower = 10f; // Height of the jump
    [SerializeField] private float _duration = 0.5f; // Duration of one jump
    [SerializeField] private float _delay = 0.1f; // Delay between dots
    private Tween[] _dotTweens;
    private bool _indicatorActive = false;
    private GameObject _parent;
    private RectTransform _textTransform;
    private RectTransform _bubbleTransform;
    private bool facingRight;
    private bool _talking;
    [SerializeField] private DialogueScriptableObject _currentDialogue;
    private int _dialogueLength;
    private int _currentDialoguePart;
    private Coroutine _typingCoroutine;
    private float[] _dotOriginalY;
    [SerializeField] private NPCName _npcName;


    void Awake()
    {
        _dotOriginalY = new float[dots.Length];
        for (int i = 0; i < dots.Length; i++)
            _dotOriginalY[i] = dots[i].anchoredPosition.y;
    }
    
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
        //dots = _indicator.GetComponentsInChildren<RectTransform>();
        //_canvas.SetActive(false);
        //InteractionIndicator(true);
        facingRight = _parent != null && _parent.transform.localScale.x < 0;
        GetDialogue();
        if(_currentDialogue == null)
        {
            _indicator.SetActive(false);
            _bubble.SetActive(false);
        }
        else _dialogueLength = _currentDialogue.dialogue.Length;
        _currentDialoguePart = 0;
    }
    void Update()
    {
        if(_currentDialogue != null)
        {
            if (inInteractivityRadius() && isLookingForward())
            {
                if (PlayerController.Instance.use2Input && !_talking)
                {
                    InteractionIndicator(false);
                    if(_currentDialoguePart >= _dialogueLength) 
                    {
                        _currentDialoguePart = 0;
                        //_indicator.GetComponent<Image>().color = UnityEngine.Color.gray;
                        _dialogueDoneIndicator.SetActive(true);
                        InteractionIndicator(true);
                    }
                    else _typingCoroutine = StartCoroutine(TypeText(_currentDialogue.dialogue[_currentDialoguePart++].dialogueText));
                }
                else if(PlayerController.Instance.use2Input)
                {
                    if (_typingCoroutine != null)
                    {
                        StopCoroutine(_typingCoroutine);
                        _typingCoroutine = null;
                        _talking = false;
                        _textComponent.text = _currentDialogue.dialogue[_currentDialoguePart-1].dialogueText;  // Or handle UI as needed
                    }
                }
            }
            else InteractionIndicator(true);
        }

        FlipDialogueBubble();
    }

    bool inInteractivityRadius()
    {
        return Physics2D.OverlapCircle(transform.position, _interacivityRadius, _playerLayer); 
    }

    bool isLookingForward()
    {
        if (_parent == null && transform.position.x * transform.localScale.x < PlayerController.Instance.transform.position.x * transform.localScale.x) return true;
        else if (_parent != null && _parent.transform.position.x * _parent.transform.localScale.x < PlayerController.Instance.transform.position.x * _parent.transform.localScale.x) return true;
        else return false;
    }

    void FlipDialogueBubble()
    {
        if (_parent.transform.localScale.x == -1 && facingRight)
        {
            facingRight = false;
            _textTransform.pivot = new Vector2(0.5f, 0.5f);
            _textTransform.localScale = new Vector3(-1, _textTransform.localScale.y, _textTransform.localScale.z);
            _textTransform.pivot = new Vector2(1, 0);
            _bubbleTransform.localScale = new Vector3(-1, _bubbleTransform.localScale.y, _bubbleTransform.localScale.z);
            _dialogueDoneIndicator.transform.localScale = new Vector3(-1, _dialogueDoneIndicator.transform.localScale.y, _dialogueDoneIndicator.transform.localScale.z);
        }
        else if (_parent.transform.localScale.x == 1 && !facingRight)
        {
            facingRight = true;
            _textTransform.pivot = new Vector2(0.5f, 0.5f);
            _textTransform.localScale = new Vector3(1, _textTransform.localScale.y, _textTransform.localScale.z);
            _textTransform.pivot = new Vector2(0, 0);
            _bubbleTransform.localScale = new Vector3(1, _bubbleTransform.localScale.y, _bubbleTransform.localScale.z);
            _dialogueDoneIndicator.transform.localScale = new Vector3(1, _dialogueDoneIndicator.transform.localScale.y, _dialogueDoneIndicator.transform.localScale.z);
        }
    }

    void InteractionIndicator(bool turnOn)
    {
        if (turnOn && !_indicatorActive) // Only if turning on and not already on
        {
            _indicatorActive = true;
            _bubble.SetActive(false);
            _indicator.SetActive(true);
            
            _dotTweens = new Tween[dots.Length];
            for (int i = 0; i < dots.Length; i++)
            {
                int idx = i;

                Vector2 pos = dots[idx].anchoredPosition;
                pos.y = _dotOriginalY[idx];
                dots[idx].anchoredPosition = pos;

                _dotTweens[idx] = dots[idx].DOAnchorPosY(_jumpPower, _duration)
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
        Gizmos.color = UnityEngine.Color.green;
        if (_parent == null) Gizmos.DrawWireSphere(transform.position, _interacivityRadius);
        else Gizmos.DrawWireSphere(_parent.transform.position, _interacivityRadius);
    }
    
    IEnumerator TypeText(string text)
    {
        _talking = true;
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
        _talking = false;
    }

    void GetDialogue()
    {
        DialogueScriptableObject[] availableDialogues = DialogueManager.Instance.GetDialogues(_npcName);
        
        if (availableDialogues == null || availableDialogues.Length == 0)
        {
            UnityEngine.Debug.LogWarning("No dialogues found in DialogueManager for NPC: " + _npcName);
            return;
        }

        StringBoolDictionary levelList = GameManager.Instance.gameState.GetListOfLevel();
        
        foreach (DialogueScriptableObject dialogueOption in availableDialogues)
        {
            if (ConditionsMet(levelList, dialogueOption.requiredLevels))
            {
                _fullText = "";
                foreach (Dialogue dialogueLine in dialogueOption.dialogue)
                {
                    _fullText += dialogueLine.dialogueText + " ";
                }
                
                _fullText = _fullText.Trim();
                _currentDialogue = dialogueOption;
                return;
            }
        }
    }

    bool ConditionsMet(StringBoolDictionary levelList, LevelSO[] requiredLevels)
    {
        if (requiredLevels == null || requiredLevels.Length == 0)
            return false;

        foreach (LevelSO requiredLevel in requiredLevels)
        {
            if (!levelList.ContainsKey(requiredLevel.levelKey) || !levelList[requiredLevel.levelKey])
            {
                return false;
            }
        }

        return true;
    }
}