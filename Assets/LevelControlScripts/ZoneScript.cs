using NUnit.Framework.Constraints;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.Netcode;

public class ZoneScript : NetworkBehaviour
{
    NetworkHud NH;
    [SerializeField] NetworkObject enemyPrefab;
    public Transform[] enemySpawns;
    public float spawnTimerMax;
    float runningSpawnTimer;
    public float playerCountSpawnModifier = 2;
    public List<GameObject> playersInZone = new List<GameObject>();

    private void Awake()
    {
        NH = FindAnyObjectByType<NetworkHud>();
    }

    private void Update()
    {
        Spawner();
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !playersInZone.Contains(other.gameObject))
        {
            playersInZone.Add(other.gameObject);
            
            spawnTimerMax *= playerCountSpawnModifier;
        }

        EnemyBehavior EB = other.gameObject.GetComponent<EnemyBehavior>();

        if (other.tag == "Enemey" && EB != null && EB.currentZone != gameObject.GetComponent<ZoneScript>())
        {

            EB.currentZone = gameObject.GetComponent<ZoneScript>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playersInZone.Contains(other.gameObject))
        {
            playersInZone.Remove(other.gameObject);
            Debug.Log("Exitzone");
            spawnTimerMax /= playerCountSpawnModifier;
        }
    }

    [Rpc(SendTo.Server)]
    public void SpawnEnemyRpc()
    {
        int ChoseSpawn = Random.Range(0, enemySpawns.Count() - 1);
        Vector3 SpawnLocation = enemySpawns[ChoseSpawn].position;
        NetworkObject newEnemey = Instantiate(enemyPrefab, SpawnLocation, Quaternion.identity);
        newEnemey.Spawn();
        EnemyBehavior EB = newEnemey.gameObject.GetComponent<EnemyBehavior>();
        EB.currentZone = this;
        EB.AquireTarget();
    }

    void Spawner()
    {
        runningSpawnTimer += Time.deltaTime;
        if (runningSpawnTimer >= spawnTimerMax)
        {
            SpawnEnemyRpc();
            runningSpawnTimer = 0;
        }
    }

    public GameObject GrabPlayer()
    {
        GameObject choosePlayer;
        if (playersInZone.Count > 0)
        {
            choosePlayer = playersInZone[Random.Range(0, playersInZone.Count - 1)];

            return choosePlayer;
        }

        if (GameObject.FindWithTag("Player") != null) {
            GameObject[] possibleTargets = GameObject.FindGameObjectsWithTag("Player");
            return choosePlayer = possibleTargets[Random.Range(0, possibleTargets.Count() - 1)];
        }

        return this.gameObject;
    }
}
