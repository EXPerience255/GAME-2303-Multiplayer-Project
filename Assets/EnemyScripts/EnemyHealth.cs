using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    PlayerShooting _playerShootScript;
    [Header("Material Types")]
    [SerializeField] MeshRenderer[] _mRenderer;
    public Material baseMat;
    public Material hitFlashMat;

    [Header("Enemy Health")]
    public int healthMax = 5;
    public int enemyHealth;

    bool takenDamage;

    
    private void Start()
    {
        _playerShootScript = GameObject.FindAnyObjectByType<PlayerShooting>();
        foreach (MeshRenderer m in _mRenderer)
        {
            m.material = baseMat;
        }
        SetHealth(healthMax);
    }

    
    public void SetHealth(int addHealth)
    {
        enemyHealth = addHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerBullet" && IsSpawned)
        {
            
            takeDamageServerRpc();
        }
        
    }

    [Rpc(SendTo.Server)]
    public void takeDamageServerRpc()
    {
        if (takenDamage) return;
        takeDamageClientRpc();
    }


    [Rpc(SendTo.ClientsAndHost)]
    public void takeDamageClientRpc()
    {
        if (takenDamage) return;
        StartCoroutine(DamageStop());
        SetHealth(-1);
        if (enemyHealth <= 0)
        {

            EnemyDieRpc();
        }

    }

    [Rpc(SendTo.Server)]
    public void EnemyDieRpc()
    {
        NetworkObject.Destroy(gameObject);
    }

    IEnumerator DamageStop()
    {
        takenDamage = true;
        yield return new WaitForEndOfFrame();
        takenDamage = false;
    }
}
