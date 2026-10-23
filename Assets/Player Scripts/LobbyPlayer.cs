using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
public class LobbyPlayer : NetworkBehaviour
{
    ulong clientID;
    Material myColor; //Color of the player
    public Material[] playerColors; //List of player colors to choose from
    GameObject body; //The player

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); //Need this to go to the main game scene.
        body = GetComponentInChildren<PlayerMovement>().gameObject;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        StartCoroutine(DisableBodyNextFrame()); //Body needs to be active on game launch so it's synced with the network.
    }

    private IEnumerator DisableBodyNextFrame()
    {
        yield return new WaitForEndOfFrame();
        body.SetActive(false);
    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0) //Lobby NEEDS to be build index 0
        {
            if (IsLocalPlayer) //This takes the client ID and gives each client its own color, connection text, and image to color in the lobby.
            {
                clientID = NetworkManager.Singleton.LocalClientId;
                myColor = playerColors[clientID];
                NetworkHud Network = FindAnyObjectByType<NetworkHud>();
                Network.RequestUpdateText(clientID);
                //Network.playerDictionary.Add(clientID, gameObject);
                Network.RequestUpdateDictionaryRpc(clientID);
            }
        }
    }

   

}
