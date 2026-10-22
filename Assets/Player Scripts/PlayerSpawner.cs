using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    LobbyPlayer player;
    void Start()
    {
        player = FindAnyObjectByType<LobbyPlayer>();
        player.TellServerToSpawnPlayersRpc();
    }
}
