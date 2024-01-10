using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DLCStore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _coinCounter;

    public int CoinCounter { set => _coinCounter.SetText(value.ToString()); }

    private void Awake()
    {
        CoinCounter = GameManager.Singleton.Wallet.Coins;
    }

    public void BuyItem(DLCItem item)
    {
        if (GameManager.Singleton.Wallet.Coins > item.Cost)
        {
            GameManager.Singleton.Wallet.Coins -= item.Cost;
            CoinCounter = GameManager.Singleton.Wallet.Coins;
        }
    }
}
