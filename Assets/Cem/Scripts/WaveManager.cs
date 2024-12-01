using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    [System.Serializable]
    public class Wave
    {
        public int totalEnemyCount;
        public List<GameObject> enemyPrefabs;


        public GameObject GetRandomPrefab()
        {
            return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        }
    }
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance; // Singleton instance


            [Header("Wave Settings")]
        public List<Wave> waves;
        private int currentWaveIndex = -1; // The current wave number
        //public int totalWaves = 5; // Total waves to progress through

        [Header("Spawn Settings")]
        public Transform[] spawnPoints; // Empty GameObjects placed outside the camera's view
        public CharController Character; // Empty GameObjects placed outside the camera's view

        private Wave currentWave;
        private int enemiesAlive = 0; // Tracks active enemies

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
       
            StartNextWave();  // Begin the first wave
        }


        public void StartNextWave()
        {
            if (currentWaveIndex >= waves.Count - 1)
            {

                //todo Game End
                Debug.Log("All waves completed!");
                return;
            }

            currentWaveIndex++;
            Debug.Log("Next wave!");
            currentWave = waves[currentWaveIndex];
            StartCoroutine(SpawnEnemies());
        }

        private IEnumerator SpawnEnemies()
        {
            for (int i = 0; i < currentWave.totalEnemyCount; i++)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                GameObject enemy = ObjectPool.Instance.GetFromPool(currentWave.GetRandomPrefab());
                enemy.transform.position = spawnPoint.position;
                enemy.transform.rotation = Quaternion.identity;
                enemiesAlive++;

                yield return new WaitForSeconds(0.5f); // Delay between spawns
            }
        }

        public void EnemyDefeated(GameObject enemy)
        {
            enemiesAlive--;

            ObjectPool.Instance.ReturnToPool(enemy); // Deactivate and return to pool

            if (enemiesAlive <= 0)
            {
                Debug.Log("Wave completed!");
                StartNextWave();
            }
        }

    
    }

   

}
