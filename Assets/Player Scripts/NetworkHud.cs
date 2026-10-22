using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkHud : NetworkBehaviour
{

    [SerializeField] GameObject[] playerTexts; //Text and Images for each player in the lobby screen
    public Dictionary<ulong, GameObject> playerDictionary = new Dictionary<ulong, GameObject>(); //This ties the clientID ulongs to the respective gameobject.
    string[] spawnPointTag = { "SpawnPoint0", "SpawnPoint1", "SpawnPoint2", "SpawnPoint3" }; //The four spawn points need to have these tags

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void StartGame()
    {
        // Crucial: Only the server/host can command clients to change scenes
        if (!IsServer) return;

        // Use LoadSceneMode.Single to close the old scene and load the new one
        SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene("MainGame", LoadSceneMode.Single);

        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load scene with status: {status}");
        }
        
    }

    public void RequestUpdateText(ulong clientID)
    {
        if (IsClient)
        {
            UpdateTextServerRpc(clientID);
        }
    }

    [Rpc(SendTo.Server)]
    private void UpdateTextServerRpc(ulong clientID)
    {
        UpdateTextClientRpc(clientID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateTextClientRpc(ulong clientID)
    {
        for (int x = (int)clientID; x > -1; x--) //ulongs can't be negative //This updates ALL the text that came before the current player joining.
        {
            Material color = FindAnyObjectByType<LobbyPlayer>().playerColors[x];
            GameObject playerText = playerTexts[x];
            playerText.GetComponent<TextMeshProUGUI>().text = "Player " + (x + 1) + " has connected"; //GetComponent<Text> is incorrect
            Image imageInPlayerText = playerText.transform.GetChild(0).GetComponent<Image>(); //Image in the lobby screen
            imageInPlayerText.gameObject.SetActive(true);
            imageInPlayerText.color = color.color; //Need to change the color of the image, not the material (the material is also used for the player color)
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
            foreach (ulong id in playerDictionary.Keys) //Host is running this twice for some reason
            {
                Debug.Log(id + ", " + NetworkManager.Singleton.LocalClientId);
                Transform spawnPoint = GameObject.FindWithTag(spawnPointTag[id]).GetComponent<Transform>();
                GameObject body = playerDictionary[id].transform.GetChild(0).gameObject;
                playerDictionary[id].transform.position = spawnPoint.position;
                body.SetActive(true);
                body.GetComponent<MeshRenderer>().material = playerDictionary[id].GetComponent<LobbyPlayer>().playerColors[id];
            }
        }
    }
}
