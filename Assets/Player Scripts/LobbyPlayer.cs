using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; //Needed to grab text, as GetComponent<Text> is incorrect
public class LobbyPlayer : NetworkBehaviour
{
    ulong clientID;
    Material myColor; //Color of the player
    GameObject playerPrefab; //The prefab to spawn in the main game
    Transform spawnPoint; //Where the player will spawn in the main game
    string[] spawnPointTag = { "SpawnPoint0", "SpawnPoint1", "SpawnPoint2", "SpawnPoint3" }; //The four spawn points need to have these tags
    public Material[] playerColors; //List of player colors to choose from
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); //Need this to go to the main game scene.
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) //Lobby NEEDS to be build index 0
        {
            if (IsLocalPlayer) //This takes the client ID and gives each client its own color, connection text, and image to color in the lobby.
            {
                clientID = NetworkManager.Singleton.LocalClientId;
                myColor = playerColors[clientID];
                FindAnyObjectByType<NetworkHud>().RequestUpdateText(clientID);
            }
        }
    }


    [Rpc(SendTo.Server)]
    public void TellServerToSpawnPlayersRpc()
    {
        SpawnPlayerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void SpawnPlayerRpc()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1) //Main game NEEDS to be Build Index 1
        {
            spawnPoint = GameObject.FindWithTag(spawnPointTag[clientID]).GetComponent<Transform>();
            transform.position = spawnPoint.position;
            Instantiate(playerPrefab, this.gameObject.transform); //This creates the playerPrefab as a child of this game object.
        }
    }
}
