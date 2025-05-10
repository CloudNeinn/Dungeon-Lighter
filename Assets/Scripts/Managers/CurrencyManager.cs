using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    [SerializeField] private int totalCurrency;
    [SerializeField] private TextMeshProUGUI currencyDisplayText;


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
        DontDestroyOnLoad(this.gameObject);
        
        totalCurrency = PlayerPrefs.GetInt("coins", 11111111);
    }

    void Start()
    {
        findCurrencyUI();
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
        //if(!currencyDisplayText) findCurrencyUI();
        if(currencyDisplayText) currencyDisplayText.text = totalCurrency.ToString();
        saveCurrency();
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

    private void saveCurrency()
    {
        PlayerPrefs.SetInt("coins", totalCurrency);
        PlayerPrefs.Save();
    }

    private void findCurrencyUI()
    {
        GameObject currencyUI = GameObject.Find("CurrencyUI");
        if(currencyUI) currencyDisplayText = currencyUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
}
