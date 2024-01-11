using UnityEngine;

public class Asset
{
    public int ID { get; }
    public string Description { get; }
    public int Price { get; }
    public Texture2D Image { get; set; }

    public Asset(int id, string description, int price)
    {
        ID = id;
        Description = description;
        Price = price;
    }
}