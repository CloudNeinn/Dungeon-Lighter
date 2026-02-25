using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [SerializeField] private int _totalCurrency;
    [SerializeField] private TextMeshProUGUI _currencyDisplayText;


    public void OnSceneLoaded()
    {
        findCurrencyUI();
        SetCurrencyUI();
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        _totalCurrency = PlayerPrefs.GetInt("coins", 0);
    }

    void Start()
    {
        findCurrencyUI();
    }

    public void addCurrency(int amount)
    {
        _totalCurrency += amount;
        SetCurrencyUI();
    }

    public void removeCurrency(int amount)
    {
        _totalCurrency -= amount;
        SetCurrencyUI();
    }

    public void SetCurrencyUI()
    {
        //if(!_currencyDisplayText) findCurrencyUI();
        if(_currencyDisplayText) _currencyDisplayText.text = _totalCurrency.ToString();
        saveCurrency();
    }

    public void SetTotalCurrency(int amount)
    {
        _totalCurrency = amount;
        SetCurrencyUI();
    }

    public int getTotalCurrency()
    {
        return _totalCurrency;
    }

    private void saveCurrency()
    {
        PlayerPrefs.SetInt("coins", _totalCurrency);
        PlayerPrefs.Save();
    }

    private void findCurrencyUI()
    {
        GameObject currencyUI = GameObject.Find("CurrencyUI");
        if(currencyUI) _currencyDisplayText = currencyUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
}
