using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkHud : NetworkBehaviour
{

    [SerializeField] GameObject[] playerTexts; //Text and Images for each player in the lobby screen

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
        for (int x = (int)clientID; x > -1; x--)
        {
            Material color = FindAnyObjectByType<LobbyPlayer>().playerColors[x];
            GameObject playerText = playerTexts[x];
            playerText.GetComponent<TextMeshProUGUI>().text = "Player " + (x + 1) + " has connected";
            Image imageInPlayerText = playerText.transform.GetChild(0).GetComponent<Image>(); //Image in the lobby screen
            imageInPlayerText.gameObject.SetActive(true);
            imageInPlayerText.color = color.color; //Need to change the color of the image, not the material (the material is also used for the player color)
        }
    }
}
