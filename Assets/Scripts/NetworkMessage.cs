using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [ServerRpc]
    public void ChangeScoreServerRpc(int id)
    {
        print("Changing Score Server RPC");
        ChangeScoreClientRpc(id);
    }

    [ServerRpc(RequireOwnership = false)]
    public void GameDoneServerRpc()
    {
        SceneManager.LoadSceneAsync("ScoreScene");

        GetComponent<NetworkObject>().Despawn();
        GameManager.Singleton.NetworkManager.Shutdown();
    }

    [ClientRpc]
    public void ChangeScoreClientRpc(int id)
    {
        GameManager.Singleton.StopAllCoroutines();
        GameManager.Singleton.PlayerList[id].PlayerScoreUI.text = $"P{GameManager.Singleton.PlayerList[id].PlayerId}: {GameManager.Singleton.IncrementScore(id)}";

        //We Call The Transition Coroutine By The Client, To Ensure Everything Is Synchronized Correctly
        if (!GameManager.Singleton.NetworkManager.IsHost)
        {
            if (GameManager.Singleton.PlayerList[0].Score == 5 || GameManager.Singleton.PlayerList[1].Score == 5)
            {
                SceneManager.LoadSceneAsync("ScoreScene");

                GameDoneServerRpc();
                GameManager.Singleton.NetworkManager.Shutdown();
            }

            else GameManager.Singleton.StartCoroutine(
                GameManager.Singleton.TransitionNextRound(2));
        }
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
            GameManager.Singleton.PlayButton1.gameObject.SetActive(true);
            GameManager.Singleton.PlayButton2.gameObject.SetActive(false);
            GameManager.Singleton.PlayButton1.onClick.AddListener(() =>
            {
                GameManager.Singleton.PlayButton1.enabled = false;
                GameManager.Singleton.PlayButton1.gameObject.SetActive(false);
                RotateCardServerRpc(0);

                GameManager.Singleton.PlayButton1.onClick.RemoveAllListeners();
            });

            GameManager.Singleton.PlayButton1.enabled = true;
        }

        //Same Thing But For The Non-Host
        else
        {
            GameManager.Singleton.PlayButton1.gameObject.SetActive(false);
            GameManager.Singleton.PlayButton2.gameObject.SetActive(true);
            GameManager.Singleton.PlayButton2.onClick.AddListener(() =>
            {
                GameManager.Singleton.PlayButton2.enabled = false;
                GameManager.Singleton.PlayButton2.gameObject.SetActive(false);
                RotateCardServerRpc(1);

                GameManager.Singleton.PlayButton2.onClick.RemoveAllListeners();
            });

            GameManager.Singleton.PlayButton2.enabled = true;
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
        GameManager.Singleton.InitializeElements();
        GameManager.Singleton.NetworkMessage = this;

        if (!GameManager.Singleton.NetworkManager.IsHost)
            SpawnCardServerRpc(0);
    }
}