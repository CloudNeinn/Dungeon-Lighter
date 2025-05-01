using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [SerializeField] private int totalCurrency;
    private TextMeshProUGUI currencyDisplayText;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currencyDisplayText = GameObject.Find("CurrencyUI").transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void addCurrency(int amount)
    {
        totalCurrency += amount;
        SetCurrencyUI();
    }

    public void removeCurrency(int amount)
    {
        totalCurrency -= amount;
        SetCurrencyUI();
    }

    public void SetCurrencyUI()
    {
        currencyDisplayText.text = totalCurrency.ToString();
    }

    public void SetTotalCurrency(int amount)
    {
        totalCurrency = amount;
        SetCurrencyUI();
    }

    public int getTotalCurrency()
    {
        return totalCurrency;
    }
}
