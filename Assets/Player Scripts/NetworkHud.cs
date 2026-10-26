using TMPro;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkHud : NetworkBehaviour
{

    [SerializeField] GameObject[] playerTexts; //Text and Images for each player in the lobby screen
    public NetworkVariable<ulong> clientIDs = new NetworkVariable<ulong>();
    //No longer used //public Dictionary<ulong, GameObject> playerDictionary = new Dictionary<ulong, GameObject>(); //This ties the clientID ulongs to the respective gameobject.
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
        SceneEventProgressStatus status = NetworkManager.Singleton.SceneManager.LoadScene("MainLevel", LoadSceneMode.Single);

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
        if (SceneManager.GetActiveScene().name == "MainLevel") //Main game NEEDS to be Build Index 1
        {
            for (ulong id = clientIDs.Value; id > 0; id--)
            {
                Debug.Log("note");
                Transform spawnPoint = GameObject.FindWithTag(spawnPointTag[id - 1]).GetComponent<Transform>();
                GameObject body = GetPlayerObjectById(id - 1).transform.GetChild(0).gameObject;
                GetPlayerObjectById(id - 1).transform.position = spawnPoint.position;
                body.SetActive(true);
                if (id - 1 == NetworkManager.Singleton.LocalClientId) //This makes each player only worry about their own camera as there's four cameras per client technically.
                {
                    CinemachineCamera playerCam = body.GetComponentInChildren<CinemachineCamera>();
                    playerCam.Priority = 1;
                }
                body.GetComponent<MeshRenderer>().material = GetPlayerObjectById(id - 1).GetComponent<LobbyPlayer>().playerColors[id - 1];
            }
            /*foreach (ulong id in playerDictionary.Keys) //This moves each player to each spawn point, and assigns them their color.
            {
                Debug.Log(id + ", " + NetworkManager.Singleton.LocalClientId);
                Transform spawnPoint = GameObject.FindWithTag(spawnPointTag[id]).GetComponent<Transform>();
                GameObject body = playerDictionary[id].transform.GetChild(0).gameObject;
                playerDictionary[id].transform.position = spawnPoint.position;
                body.SetActive(true);
                if (id == NetworkManager.Singleton.LocalClientId) //This makes each player only worry about their own camera as there's four cameras per client technically.
                {
                    CinemachineCamera playerCam = body.GetComponentInChildren<CinemachineCamera>();
                    playerCam.Priority = 1;
                }
                body.GetComponent<MeshRenderer>().material = GetPlayerObjectById(id).GetComponent<LobbyPlayer>().playerColors[id];
            
            }*/
        }
    }

    [Rpc(SendTo.Server)]
    public void RequestUpdateClientIdRpc(ulong id)
    {
        clientIDs.Value++;
    }

    public GameObject GetPlayerObjectById(ulong clientId) //This returns the GameObject tied to the local client ID, it makes the playerDictionary redundant, but things might break if you remove it. Found using Gemini :/
    {
        // Note: ConnectedClients is only available on the Server/Host by default.
        // On a pure client, use NetworkManager.Singleton.LocalClient instead.
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            return networkClient.PlayerObject.gameObject;
        }
        return null;
    }
}
