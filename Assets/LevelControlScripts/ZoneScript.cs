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
            Debug.Log("added player" + playersInZone[0].name);
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

        if (FindAnyObjectByType<PlayerHealth>() != null) {
            choosePlayer = NH.GetPlayerObjectById((ulong)Random.Range(1, NH.clientIDs.Value) - 1);
            return choosePlayer.transform.GetChild(0).gameObject;
        }

        return null;
    }
}
