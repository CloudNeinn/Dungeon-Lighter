using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveUIController : MonoBehaviour
{
    [SerializeField] private float _interacivityRadius;
    [SerializeField] private Vector3 _interactiovityOffset;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private GameObject _UI;
    [SerializeField] private float _screenShiftX = 0.4f;
    [SerializeField] private bool _UIActive;
    private NoteUI _noteUI;


    void Start()
    {
        _UIActive = false;
        _noteUI = GetComponent<NoteUI>();
    }

    void Update()
    {
        _UI.SetActive(_UIActive);
        if (PlayerController.Instance.use1Input && InRange())
        {
            _UIActive = !_UIActive;
            CurrencyManager.Instance.SetCurrencyUI();
            if (_noteUI != null) _noteUI.LoadNoteText();
        }
        if (_UIActive && (!InRange() || Mathf.Abs(PlayerController.Instance.PlayerRigidbody.linearVelocity.x) > 0)) _UIActive = false;
        if (_UIActive && CameraController.Instance.transposer.m_ScreenX == CameraController.Instance.getCameraScreenX())
        {
            CameraController.Instance.setCameraScreenX(_screenShiftX);
            UIManager.Instance.changeState();
        }
        else if (!_UIActive) UIManager.Instance.changeState();
    }

    bool InRange()
    {
        return Physics2D.OverlapCircle(transform.position + _interactiovityOffset, _interacivityRadius, _playerLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + _interactiovityOffset, _interacivityRadius);
    }

    public bool isActive()
    {
        return _UIActive;
    }
}
