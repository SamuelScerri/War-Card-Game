using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Asset
{
    public int ID { get; }
    public string Description { get; }
    public int Price { get; }
    public Texture2D Image;
    public bool Owned;

    public Asset(int id, string description, int price)
    {
        ID = id;
        Description = description;
        Price = price;
        Image = new Texture2D(0, 0);
        Owned = PlayerPrefs.HasKey("DLC" + id);
    }

    public void Buy()
    {
        if (GameManager.Singleton.Wallet.Coins > Price)
        {
            GameManager.Singleton.Wallet.Coins -= Price;
            PlayerPrefs.SetString("DLC" + ID, Description);
        }
    }
}