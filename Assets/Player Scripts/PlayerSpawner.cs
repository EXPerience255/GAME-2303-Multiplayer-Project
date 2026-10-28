using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    NetworkHud Network;
    void Start()
    {
        Network = FindAnyObjectByType<NetworkHud>();
        Network.TellServerToSpawnPlayersRpc();
    }
}
