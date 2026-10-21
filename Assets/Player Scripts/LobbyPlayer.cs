using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LobbyPlayer : NetworkBehaviour
{
    ulong clientID;
    Material myColor; //Color of the player
    Image myImage; //Image in the lobby screen
    GameObject playerPrefab; //The prefab to spawn in the main game
    Transform spawnPoint; //Where the player will spawn in the main game
    string[] spawnPointTag = { "SpawnPoint0", "SpawnPoint1", "SpawnPoint2", "SpawnPoint3" }; //The four spawn points need to have these tags
    [SerializeField] Material[] playerColors; //List of player colors to choose from
    [SerializeField] GameObject[] playerTexts; //Text and Images for each player in the lobby screen

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) //Lobby NEEDS to be build index 0
        {
            if (IsOwner) //This takes the client ID and gives each client its own color, connection text, and image to color in the lobby.
            {
                clientID = NetworkManager.Singleton.LocalClientId;
                myColor = playerColors[clientID];
                playerTexts[clientID].SetActive(true);
                myImage = playerTexts[clientID].GetComponentInChildren<Image>();
                myImage.material = myColor;
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
