using UnityEngine;
using Unity.Netcode;
public class PlayerBullet : NetworkBehaviour
{
    ulong clientID;
    public int damage = 1;
    public float killTimer = 2f;

    void Start()
    {
        clientID = NetworkManager.Singleton.LocalClientId;
        DestroyRpc(killTimer);
    }

    [Rpc(SendTo.Server)]
    private void DestroyRpc(float killTimer) //Server has to destroy it
    {
        Destroy(gameObject, killTimer);
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHealth>() && other.gameObject.GetComponentInParent<LobbyPlayer>().clientID != clientID) //If a player isn't getting hit by it's own bullet
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamageServerRpc(damage);
            DestroyRpc(0f);
        }
    }*/

    private void OnCollisionEnter(Collision other)
    {                                                               //This checks the client ID of the other game object
        if (other.gameObject.GetComponent<PlayerHealth>() && other.gameObject.GetComponentInParent<NetworkObject>().OwnerClientId != clientID) //If a player isn't getting hit by it's own bullet
        {
            Debug.Log("Collision");
            other.gameObject.GetComponent<PlayerHealth>().TakeDamageServerRpc(damage, other.gameObject.GetComponentInParent<NetworkObject>().OwnerClientId); //Damage is only dealt to the client that took damage
            DestroyRpc(0f);
        }
    }
}
