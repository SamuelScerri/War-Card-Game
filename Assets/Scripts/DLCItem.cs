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

	[SerializeField]
	private Slider progressBar;

	private Asset assetData;
	public Action OnBuy;

	public float Progress
	{
		set
		{
			if (value >= 1)
			{
				progressBar.gameObject.SetActive(false);
				itemImage.color = new Color(1, 1, 1, 1);

				progressBar.value = value;
			}
				
			else
			{
				progressBar.gameObject.SetActive(true);
				itemImage.color = new Color(1, 1, 1, 0);

				progressBar.value = value;
			}
				
		}
	}

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
		if (assetData.BuyOrEquip()) {
			GameManager.Singleton.ActiveBackground = assetData.Image;
		}
	}
}
