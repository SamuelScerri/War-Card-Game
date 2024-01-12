using System;
using UnityEngine;

public class Asset
{
    public int ID { get; }
    public string Description { get; }
    public int Price { get; }
    public Texture2D Image;
    public bool Owned;

    public Action OnBuy;

    public Asset(int id, string description, int price)
    {
        ID = id;
        Description = description;
        Price = price;
        Image = new Texture2D(0, 0);
        Owned = PlayerPrefs.HasKey("DLC" + id);
    }

    public bool BuyOrEquip()
    {
        if (Owned)
            return true;

        else if (GameManager.Singleton.Wallet.Coins > Price)
        {
            GameManager.Singleton.Wallet.Coins -= Price;
            PlayerPrefs.SetString("DLC" + ID, Description);
            
            OnBuy.Invoke();

            return true;
        }

        else return false;
    }
}