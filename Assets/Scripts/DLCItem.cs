using System;
using System.Collections;
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

	//Special Effect
	public IEnumerator FlashAnimation()
	{
		float time = 0;

		while (time < 1)
		{
			time += Time.deltaTime;
			itemImage.color = Color.Lerp(Color.red, Color.white, time);
			itemImage.rectTransform.localScale = Vector3.one + Vector3.one * .25f * Mathf.Sin(Mathf.Lerp(0, Mathf.Deg2Rad*180, time));

			yield return new WaitForEndOfFrame();
		}
	}

	public void Interact()
	{
		if (assetData.BuyOrEquip()) {
			GameManager.Singleton.ActiveBackground = assetData.Image;
			descriptionText.SetText("EQUIP");

			//StartCoroutine(FlashAnimation());
		}
	}
}
