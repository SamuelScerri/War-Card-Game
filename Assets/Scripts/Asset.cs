using System;
using UnityEngine;
using Firebase;
using Firebase.Database;

//Class Used To Store To Realtime Database
public struct AssetData {
    public int playerId;
    public int itemPurchased;
    public string date;
    public string time;

    public AssetData(int playerId, int itemPurchased, string date, string time) {
        this.playerId = playerId;
        this.itemPurchased = itemPurchased;
        this.date = date;
        this.time = time;
    }
}

//Class Used For The In-Game Store
public class Asset
{
    public int ID { get; }
    public string Description { get; }
    public int Price { get; }
    public Texture2D Image;
    public bool Owned;

    public Action OnBuy;

    public void WriteNewAsset()
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        AssetData assetData = new AssetData(GameManager.Singleton.Wallet.ID, ID, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        string json = JsonUtility.ToJson(assetData);

        reference.Child("Purchase History").Push().SetRawJsonValueAsync(json);
    }

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

            WriteNewAsset();
            
            OnBuy.Invoke();

            return true;
        }

        else return false;
    }
}