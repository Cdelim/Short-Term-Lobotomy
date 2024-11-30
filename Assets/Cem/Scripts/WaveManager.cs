using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cem.Scripts {
    
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance; // Singleton instance

    [Header("Wave Settings")]
    public int currentWave = 0; // The current wave number
    public int totalWaves = 5; // Total waves to progress through
    public int enemiesAlive = 0; // Tracks active enemies

    [Header("Object Pool Settings")]
    public GameObject enemyPrefab; // Prefab for the enemies
    public int poolSize = 30; // Number of enemies to preload
    private Queue<GameObject> enemyPool = new();

    [Header("Spawn Settings")]
    public Transform[] spawnPoints; // Empty GameObjects placed outside the camera's view

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializePool(); // Preload enemies
        StartNextWave();  // Begin the first wave
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(enemyPrefab);
            obj.SetActive(false); // Start inactive
            enemyPool.Enqueue(obj); // Add to the pool
        }
    }

    public void StartNextWave()
    {
        if (currentWave >= totalWaves)
        {
            Debug.Log("All waves completed!");
            return;
        }

        currentWave++;
        int enemyCount = currentWave * 5; // Example: 5 enemies per wave
        StartCoroutine(SpawnEnemies(enemyCount));
    }

    private IEnumerator SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemy = GetFromPool();
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true); // Activate the enemy
            enemiesAlive++;

            yield return new WaitForSeconds(0.5f); // Delay between spawns
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        enemiesAlive--;

        ReturnToPool(enemy); // Deactivate and return to pool

        if (enemiesAlive <= 0)
        {
            Debug.Log("Wave completed!");
            StartNextWave();
        }
    }

    private GameObject GetFromPool()
    {
        if (enemyPool.Count > 0)
        {
            return enemyPool.Dequeue();
        }

        GameObject newEnemy = Instantiate(enemyPrefab);
        return newEnemy;
    }

    private void ReturnToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }
}

}