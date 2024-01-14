using UnityEngine;

public struct WalletData
{
    private int defaultAmount;

    public int ID
    {
        get => PlayerPrefs.GetInt("Player ID", Random.Range(0, 9999));
        set => PlayerPrefs.SetInt("Player ID", value);
    }

    public int Coins
    {
        get => PlayerPrefs.GetInt("Coins", defaultAmount);
        set => PlayerPrefs.SetInt("Coins", value);
    }

    public WalletData(int amount)
    {
        defaultAmount = amount;
        ID = ID;
    }
}
