using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    public static Bar Instance { get; private set; }
    [SerializeField] private float _interacivityRadius;
    [SerializeField] private Vector3 _interactiovityOffset;
    [SerializeField] private LayerMask _playerLayer;
    [SerializeField] private GameObject _barUI;
    public bool _barUIActive { get; private set;}
    [SerializeField] private GameObject[] _barPages;
    private int _currentPage = 0;
    [SerializeField] private float barScreenShiftX = 0.4f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        _barUIActive = false;
    }

    void Start()
    {
        _barPages[_currentPage].SetActive(true);
    }

    void Update()
    {
        _barUI.SetActive(_barUIActive);
        if(PlayerControl.Instance._use1Input && InRange())
        {
            _barUIActive = !_barUIActive;
            CurrencyManager.Instance.SetCurrencyUI();
        } 
        if(!InRange()) _barUIActive = false;
        if(_barUIActive && CameraController.Instance.transposer.m_ScreenX == CameraController.Instance.getCameraScreenX()) CameraController.Instance.setCameraScreenX(barScreenShiftX);
        else if(!_barUIActive) CameraController.Instance.setCameraScreenX(CameraController.Instance.getCameraScreenX());
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

    public void NextPage()
    {
        _barPages[_currentPage].SetActive(false);
        _currentPage++;
        if(_currentPage >= _barPages.Length) _currentPage = 0;
        _barPages[_currentPage].SetActive(true);
    }

    public void PreviousPage()
    {
        _barPages[_currentPage].SetActive(false);
        _currentPage--;
        if(_currentPage < 0) _currentPage = _barPages.Length - 1;
        _barPages[_currentPage].SetActive(true);

    }
}
