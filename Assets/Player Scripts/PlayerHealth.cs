using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [Header("Health")]
    [SerializeField] HealthStat healthStat;
    public int health;

    [Header("Invincibility")]
    public float invincibilityTime;
    private float invincibilityTimer;

    void Start()
    {
        health = healthStat.health;
    }

    private void Update()
    {
        if (invincibilityTimer > 0) invincibilityTimer -= Time.deltaTime;
    }

    [Rpc(SendTo.Server)]
    public void TakeDamageServerRpc(int damage, ulong clientID)
    {
        if (invincibilityTimer > 0) return;
        TakeDamageClientRpc(damage, clientID);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void TakeDamageClientRpc(int damage, ulong clientID)
    {
        health -= damage;
        invincibilityTimer = invincibilityTime;
        Debug.Log("Ow");
        if (health <= 0)
        {
            DieServerRpc(clientID);
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
