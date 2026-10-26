using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] HealthStat healthStat;
    int health;

    void Start()
    {
        health = healthStat.health;
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(int damage, ulong clientID)
    {
        TakeDamageClientRpc(damage, clientID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TakeDamageClientRpc(int damage, ulong clientID)
    {
        if (IsLocalPlayer)
        {
            if (GetComponentInParent<LobbyPlayer>().clientID == clientID) //Damage is only dealt to the client that took damage.
            {
                health -= damage;
                Debug.Log("Ow");
                if (health <= 0)
                {
                    DieServerRpc(clientID);
                }
            }
        }
        
    }

    [Rpc(SendTo.Server)]
    private void DieServerRpc(ulong clientID)
    {
        DieClientRpc(clientID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DieClientRpc(ulong clientID)
    {
        FindAnyObjectByType<NetworkHud>().GetPlayerObjectById(clientID).SetActive(false);
    }
}
