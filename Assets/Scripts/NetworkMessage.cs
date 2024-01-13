using Unity.Netcode;

public class NetworkMessage : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void HelloServerRpc()
    {
        HelloClientRpc();
    }

    [ClientRpc]
    public void HelloClientRpc()
    {
        print("Hello");
    }

    public void Start()
    {
        GameManager.Singleton.NetworkMessage = this;

        if (!GameManager.Singleton.NetworkManager.IsHost)
            HelloServerRpc();
    }
}