using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyManager : MonoBehaviour
{

    public int gold;

    public TextMeshProUGUI _goldText;
    private string goldStringDisplay;

    // This script is a singleton, so the gold amount of the player can be easily accessible from anywhere
    public static CurrencyManager currencyManagerInstance;

    private void Awake()
    {
        currencyManagerInstance = this;
    }

    void Start()
    {
        // Serve as initialisation
        AddGold(0);
    }

   
    // Called when a mob is killed or an upgrade purchased (then the amount should be negative)
    public void AddGold(int amount)
    {
        gold += amount;
        goldStringDisplay = gold.ToString();

        // If the gold amount is too big, display it with K, M ,B symbols (thousand, million, billion)
        if (gold >= 1000)
        {
            goldStringDisplay = ((float)gold / 1000f).ToString() + " K";
        }
        if (gold >= 1000000)
        {
            goldStringDisplay = ((float)gold / 1000000f).ToString() + " M";
        }
        if (gold >= 1000000000)
        {
            goldStringDisplay = ((float)gold / 1000000000f).ToString() + " B";
        }

        _goldText.text = goldStringDisplay;

        // This events informs the other scripts that the gold amount of the player has changed.
        OnGold?.Invoke();
    }

    public delegate void GoldEvent();
    public event GoldEvent OnGold;

}