using NUnit.Framework.Constraints;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ZoneScript : MonoBehaviour
{
    NetworkHud NH;
    public GameObject enemyPrefab;
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

    public void SpawnEnemy()
    {
        int ChoseSpawn = Random.Range(0, enemySpawns.Count() - 1);
        Vector3 SpawnLocation = enemySpawns[ChoseSpawn].position;
        GameObject newEnemey = Instantiate(enemyPrefab, SpawnLocation, Quaternion.identity);
        EnemyBehavior EB = newEnemey.gameObject.GetComponent<EnemyBehavior>();
        EB.currentZone = this;
    }

    void Spawner()
    {
        runningSpawnTimer += Time.deltaTime;
        if (runningSpawnTimer >= spawnTimerMax)
        {
            SpawnEnemy();
            runningSpawnTimer = 0;
        }
    }

    public Transform GrabPlayer()
    {
        if (playersInZone.Count > 0)
        {
            GameObject choosePlayer = playersInZone[Random.Range(0, playersInZone.Count - 1)];

            return choosePlayer.transform;
        }

        if(FindAnyObjectByType<PlayerHealth>() != null)
            return NH.GetPlayerObjectById((ulong)Random.Range(0, NH.clientIDs.Value - 1)).transform;

        return null;
    }
}
