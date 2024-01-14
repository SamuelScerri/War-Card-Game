using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class NetworkMessage : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void SpawnCardsServerRpc()
    {
        print("Spawning Cards");

        //Clear And Spawn Player 1's Card
        DestroyCardClientRpc(0);
        CreateCardClientRpc(0, UnityEngine.Random.Range(0, GameManager.Singleton.PlayingCards.Count));
    
        //Clear And Spawn Player 2's Card
        DestroyCardClientRpc(1);
        CreateCardClientRpc(1, UnityEngine.Random.Range(0, GameManager.Singleton.PlayingCards.Count));
    }

    [ClientRpc]
    public void DestroyCardClientRpc(int id)
    {
        print("Destroying Player ID: " + id + "'s Card");

        if (GameManager.Singleton.PlayerList[id].Card.CardGameObject != null)
        {
            Destroy(GameManager.Singleton.PlayerList[id].Card.CardGameObject);
            GameManager.Singleton.PlayerList[id].Card = new Card();
        }
    }

    [ClientRpc]
    public void CreateCardClientRpc(int playerId, int cardId)
    {
        print("Creating Player ID: " + playerId + "'s Card");

        Card chosenCard = new Card(GameManager.Singleton.PlayingCards.ElementAt(cardId).Key, GameManager.Singleton.PlayingCards.ElementAt(cardId).Value);
        GameObject card = Instantiate(chosenCard.CardGameObject, GameManager.Singleton.PlayerList[playerId].PlayerGameObject.position,Quaternion.Euler(-90, 0, 180), GameManager.Singleton.PlayerList[playerId].PlayerGameObject);
        card.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        GameManager.Singleton.PlayerList[playerId].Card = new Card(chosenCard.CardName, card);
        int.TryParse(chosenCard.CardName.Substring(chosenCard.CardName.Length - 2), out GameManager.Singleton.PlayerList[playerId].Card.CardValue);
    }

    [ClientRpc]
    public void SpawnCardsClientRpc()
    {

    }

    public void Start()
    {
        GameManager.Singleton.NetworkMessage = this;

        if (!GameManager.Singleton.NetworkManager.IsHost)
            SpawnCardsServerRpc();
    }
}