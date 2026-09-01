using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;

    public float triggerRadius = 10f;
    public int maxAliveEnemies = 3;
    public float spawnInterval = 3f;
    public int maxTotalSpawned = 10;
    public int totalSpawned = 0;
    private float spawnTimer;

    public Transform playerTarget;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) {playerTarget = playerObj.transform;}
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTarget == null) return;
        if(IsEmpty()) return;

        RemoveDead();

        bool playerInRange = Vector3.Distance(transform.position, playerTarget.position) <= triggerRadius;
        if (!playerInRange) return;

        if (spawnedEnemies.Count >= maxAliveEnemies) return;

        spawnTimer += Time.deltaTime;

        if(spawnTimer >= spawnInterval)
        {
            spawnTimer = 0;
            SpawnEnemy();
        } 
    }

    private void SpawnEnemy()
    {
        if(enemyPrefabs.Length == 0 || spawnPoints.Length == 0) return;

        GameObject spawnedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        Transform spawnPoiont = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject newEnemy = Instantiate(spawnedPrefab, spawnPoiont.position, spawnPoiont.rotation);

        spawnedEnemies.Add(newEnemy);
        totalSpawned++;
    }

    void RemoveDead()
    {
        for (int i = spawnedEnemies.Count - 1; i >= 0; --i)
        {
            if (spawnedEnemies[i] == null)
            {
                spawnedEnemies.RemoveAt(i); 
            }
        }
    }

    public bool IsEmpty()
    {
        return maxTotalSpawned > 0 && totalSpawned >= maxTotalSpawned;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, triggerRadius);
    }
}
