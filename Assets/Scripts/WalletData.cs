using UnityEngine;

public struct WalletData
{
    public int Coins;

    public WalletData(int defaultAmount) {
        Coins = PlayerPrefs.GetInt("Coins", defaultAmount);
    }
}
