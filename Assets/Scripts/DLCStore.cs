using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Firebase.Extensions;
using Firebase.Storage;
using TMPro;
using UnityEngine;

public class DLCStore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI coinCounter;
    public int CoinCounter { set => coinCounter.SetText(value.ToString()); }

    private List<Asset> assets;

    private void Awake()
    {
        //CoinCounter = GameManager.Singleton.Wallet.Coins;

        assets = new List<Asset>();

        FirebaseStorage storage = FirebaseStorage.DefaultInstance;
        DownloadManifest(storage.GetReference("Manifest.xml"), node => {
            print("Creating Node");

            Asset loadedAsset = new Asset(0, "Test", 300);
            //print(node.SelectSingleNode("backgroundImageURL").InnerText);

            DownloadTexture(storage.GetReference("Manifest.xml"));

            assets.Add(loadedAsset);
        });
    }

    private void DownloadTexture(StorageReference reference)
    {
        print("Preparing To Download");
        string localUrl = Application.persistentDataPath + "/test.xml";

        Task task = reference.GetFileAsync(localUrl, 
            new StorageProgress<DownloadState>(state => {
                Debug.Log("Downloading Texture: " + state.BytesTransferred);
            }), CancellationToken.None);

        task.ContinueWithOnMainThread(resultTask => {
            if (!resultTask.IsFaulted && !resultTask.IsCanceled) {
                Debug.Log("Complete");
                //Debug.LogException(resultTask.Exception);
                //ImageConversion.LoadImage(texture, File.ReadAllBytes(Application.persistentDataPath + reference.Path));
            }
        });
    }

    private void DownloadManifest(StorageReference reference, Action<XmlNode> onComplete)
    {
        XmlDocument manifest = new XmlDocument();

        reference.GetStreamAsync(stream => {
            manifest.Load(stream);
            print("Loaded Manifest");

            if (stream.CanRead)
                onComplete.Invoke(manifest.SelectNodes("/store/item")[0]);

            //foreach(XmlNode itemNode in manifest.SelectNodes("/store/item"))
                //onComplete.Invoke(itemNode);
        }, null, CancellationToken.None);
    }
}
