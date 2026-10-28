using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    public float maxHealth;
    public float health;
    private bool immune;

    void Start()
    {
        health = maxHealth;
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(float damage, ulong clientID)
    {
        if (immune) return;
        TakeDamageClientRpc(damage, clientID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TakeDamageClientRpc(float damage, ulong clientID)
    {
        if (immune) return;

        if (IsLocalPlayer)
        {
            if (GetComponentInParent<LobbyPlayer>().clientID == clientID) //Damage is only dealt to the client that took damage.
            {
                health -= damage;
                StartCoroutine(MakeImmune());
                Debug.Log("Ow");
                if (health <= 0)
                {
                    DieServerRpc(clientID);
                }
            }
        }
    }

    IEnumerator MakeImmune()
    {
        immune = true;
        yield return new WaitForEndOfFrame();
        immune = false;
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
