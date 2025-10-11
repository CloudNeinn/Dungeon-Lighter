using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public static Bar Instance { get; private set; }
    [SerializeField] private GameObject[] _barPages;
    private int _currentPage = 0;
    [SerializeField] private GameObject[] shotButtons;

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
    }

    void Start()
    {
        _barPages[_currentPage].SetActive(true);
        setButtonActions();
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

    void setButtonActions()
    {
        foreach(var button in shotButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(() => ShotManager.Instance.setActiveShots(button.name));
        }
    }
}
