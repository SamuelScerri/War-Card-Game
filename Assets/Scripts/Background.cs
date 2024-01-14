using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private void Awake()
    {
        if (GameManager.Singleton.ActiveBackground == null)
            gameObject.SetActive(false);
    }
}
