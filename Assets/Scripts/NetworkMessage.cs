using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkMessage : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SpawnCardServerRpc(int id)
    {
        print("Spawning Cards Server RPC");

        //Clear And Spawn Player ID's Card
        if (id < GameManager.Singleton.PlayerList.Count)
            SpawnCardClientRpc(id, UnityEngine.Random.Range(0, GameManager.Singleton.PlayingCards.Count));
    
        //If All The Player Cards Are Initialized, Then We Can Start Giving Control To The Users
        else GiveControlClientRpc();
    }

    [ClientRpc]
    public void SpawnCardClientRpc(int playerId, int cardId)
    {
        print("Destroying Player ID: " + playerId + "'s Card");

        if (GameManager.Singleton.PlayerList[playerId].Card.CardGameObject != null)
        {
            Destroy(GameManager.Singleton.PlayerList[playerId].Card.CardGameObject);
            GameManager.Singleton.PlayerList[playerId].Card = new Card();
        }

        print("Creating Player ID: " + playerId + "'s Card");

        Card chosenCard = new Card(GameManager.Singleton.PlayingCards.ElementAt(cardId).Key, GameManager.Singleton.PlayingCards.ElementAt(cardId).Value);
        GameObject card = Instantiate(chosenCard.CardGameObject, GameManager.Singleton.PlayerList[playerId].PlayerGameObject.position,Quaternion.Euler(-90, 0, 180), GameManager.Singleton.PlayerList[playerId].PlayerGameObject);
        card.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        GameManager.Singleton.PlayerList[playerId].Card = new Card(chosenCard.CardName, card);
        int.TryParse(chosenCard.CardName.Substring(chosenCard.CardName.Length - 2), out GameManager.Singleton.PlayerList[playerId].Card.CardValue);
    
        //If The Final Card Is Spawned, Then We Inform The Server To Spawn The Next Player's Card
        if (!GameManager.Singleton.NetworkManager.IsHost)
            SpawnCardServerRpc(playerId + 1);
    }

    [ClientRpc]
    public void GiveControlClientRpc()
    {
        //We Hide The Other Player's Button & Enable The Correct Button
        if (GameManager.Singleton.NetworkManager.IsHost)
        {
            GameManager.Singleton.PlayButton2.gameObject.SetActive(false);
            GameManager.Singleton.PlayButton1.onClick.AddListener(() =>
            {
                GameManager.Singleton.PlayButton1.enabled = false;
                RotateCardServerRpc(0);
            });
        }

        //Same Thing But For The Non-Host
        else
        {
            GameManager.Singleton.PlayButton1.gameObject.SetActive(false);
            GameManager.Singleton.PlayButton2.onClick.AddListener(() =>
            {
                GameManager.Singleton.PlayButton2.enabled = false;
                RotateCardServerRpc(1);
            });
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RotateCardServerRpc(int id)
    {
        RotateCardClientRpc(id);
    }

    [ClientRpc]
    public void RotateCardClientRpc(int id)
    {
        //We Simply Call The Coroutine To Rotate The Card
        GameManager.Singleton.StartCoroutine(
            GameManager.Singleton.RotateCard(GameManager.Singleton.PlayerList[id]));
    }

    public void Start()
    {
        GameManager.Singleton.NetworkMessage = this;

        if (!GameManager.Singleton.NetworkManager.IsHost)
            SpawnCardServerRpc(0);
    }
}