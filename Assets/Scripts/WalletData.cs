using UnityEngine;

public struct WalletData
{
    private int defaultAmount;

    public int Coins
    {
        get => PlayerPrefs.GetInt("Coins", defaultAmount);
        set => PlayerPrefs.SetInt("Coins", value);
    }

    public WalletData(int amount)
    {
        defaultAmount = amount;
    }
}
