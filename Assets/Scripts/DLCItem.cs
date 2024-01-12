using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DLCItem : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI amountText, descriptionText;
	
	[SerializeField]
	private RawImage itemImage;

	private Asset assetData;
	public Action OnBuy;

	public Asset AssetData
	{
		get => assetData;
		set
		{
			assetData = value;
			amountText.SetText(assetData.Description);

			if (!assetData.Owned)
				descriptionText.SetText(assetData.Price.ToString());
			itemImage.texture = assetData.Image;
		}
	}

	public void Interact()
	{
		if (assetData.BuyOrEquip())
			descriptionText.SetText("EQUIP");
	}
}
