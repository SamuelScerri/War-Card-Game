using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Firebase.Extensions;
using Firebase.Storage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DLCStore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinCounter;

    [SerializeField]
    private Transform grid;

    [SerializeField]
    private GameObject DLCItemPrefab;

    public int CoinCounter { set => coinCounter.SetText(value.ToString()); }

    [SerializeField] private List<Asset> assets;
    public List<Asset> Assets { get => assets; }

    private DLCItem CreateDLCItem(Asset asset)
    {
        DLCItem newItem = Instantiate(DLCItemPrefab, grid).GetComponent<DLCItem>();
        newItem.AssetData = asset;

        return newItem;
    }

    private void Awake()
    {
        CoinCounter = GameManager.Singleton.Wallet.Coins;
        assets = new List<Asset>();

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        DownloadFile(storage.GetReference("Manifest.xml"), true, null, data => {
            XmlDocument manifest = new XmlDocument();
            manifest.Load(data);

            foreach(XmlNode itemNode in manifest.SelectNodes("/store/item")) {
                Asset newAsset = new Asset(
                    Int32.Parse(itemNode.SelectSingleNode("id").InnerText),
                    itemNode.SelectSingleNode("description").InnerText,
                    Int32.Parse(itemNode.SelectSingleNode("price").InnerText));

                DLCItem newItem = CreateDLCItem(newAsset);
                
                assets.Add(newAsset);

                if (!newAsset.Owned)
                    DownloadTexture(storage.GetReference(itemNode.SelectSingleNode("previewImageURL").InnerText), newAsset.Image, newItem);
                else DownloadTexture(storage.GetReference(itemNode.SelectSingleNode("backgroundImageURL").InnerText), newAsset.Image, newItem);

                newAsset.OnBuy = delegate() {
                    CoinCounter = GameManager.Singleton.Wallet.Coins;
                    DownloadTexture(storage.GetReference(itemNode.SelectSingleNode("backgroundImageURL").InnerText), newAsset.Image, newItem);
                };
            }
        });
    }

    //Downloads a File, if it already exists there is no need to re-download it again
    private void DownloadFile(StorageReference reference, bool forceDownload, Action<DownloadState> onDownload, Action<string> onComplete)
    {
        string localUrl = Application.persistentDataPath + reference.Path;

        if (!Directory.Exists(Path.GetDirectoryName(localUrl)))
            Directory.CreateDirectory(Path.GetDirectoryName(localUrl));
        
        if (forceDownload || !File.Exists(localUrl))
        {
            Task task = reference.GetFileAsync(localUrl, 
                new StorageProgress<DownloadState>(state => {
                    onDownload.Invoke(state);
                }), CancellationToken.None);

            task.ContinueWithOnMainThread(resultTask => {
                if (!resultTask.IsFaulted && !resultTask.IsCanceled)
                    onComplete.Invoke(localUrl);
            });
        }

        else
            onComplete.Invoke(localUrl);
    }

    private void DownloadTexture(StorageReference reference, Texture2D texture, DLCItem item) {
        DownloadFile(reference, false, state => {
            if (state.BytesTransferred != 0)
                item.Progress = (int)(state.TotalByteCount / state.BytesTransferred);
        }, data => {
            item.Progress = 1;
            texture.LoadImage(File.ReadAllBytes(data));
        });
    }
}
