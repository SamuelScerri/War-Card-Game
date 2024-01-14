using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWait : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.Singleton.NetworkManager.IsHost)
            StartCoroutine(WaitForSecondPlayer());
        else gameObject.SetActive(false);
    }

    private IEnumerator WaitForSecondPlayer()
    {
        while (true)
        {
            if (GameManager.Singleton.NetworkManager.ConnectedClientsList.Count == 2)
            {
                gameObject.SetActive(false);
                break;
            }
            yield return new WaitForEndOfFrame();
        }

    }
}
