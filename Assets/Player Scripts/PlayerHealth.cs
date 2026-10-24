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
    public void TakeDamageServerRpc(int damage)
    {
        TakeDamageClientRpc(damage);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TakeDamageClientRpc(int damage)
    {
        health -= damage;
        Debug.Log("Ow");
    }
}
