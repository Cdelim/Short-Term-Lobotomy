using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {

    public class Wave
    {
        public int EnemyCount;
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
        public int currentWaveIndex = 0; // The current wave number
        //public int totalWaves = 5; // Total waves to progress through
        public int enemiesAlive = 0; // Tracks active enemies

        [Header("Spawn Settings")]
        public Transform[] spawnPoints; // Empty GameObjects placed outside the camera's view

        private Wave currentWave;

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

            currentWave = waves[currentWaveIndex];
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
                enemy.SetActive(true); // Activate the enemy
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




    public class ObjectPool : MonoBehaviour
    {
            public List<GameObject> prefabList;
            public static ObjectPool Instance;
            private readonly Dictionary<GameObject,Queue<GameObject>> pool = new Dictionary<GameObject,Queue<GameObject>>();
            private readonly GameObject prefab;
            private readonly Transform parent;
            private readonly int initialSize = 30;


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
                DontDestroyOnLoad(this);
                foreach(var obj in prefabList)
                {
                    for (int i = 0; i < initialSize; i++)
                    {
                        CreateNewObject(obj);

                    }
                }
            }

       

            // Get an object from the pool
            public GameObject GetFromPool(GameObject prefab)
            {
                if (pool[prefab].Count == 0)
                {
                    // No available objects, create a new one
                    CreateNewObject(prefab);
                }

                GameObject obj = pool[prefab].Dequeue();
                IPoolObject poolObject;
                if (obj.TryGetComponent<IPoolObject>(out poolObject))
                {
                    poolObject.Initialize();
                }
                obj.gameObject.SetActive(true);
                return obj;
            }

            // Return an object to the pool
            public void ReturnToPool(GameObject obj)
            {
                IPoolObject poolObject;
                if (obj.TryGetComponent<IPoolObject>(out poolObject))
                {
                    poolObject.DestroyPoolObj();
                }
                obj.gameObject.SetActive(false);
                pool[prefab].Enqueue(obj);
            }

            // Create a new object and add it to the pool
            private void CreateNewObject(GameObject prefab)
            {
                GameObject newObj = Object.Instantiate(prefab, parent);
                newObj.gameObject.SetActive(false);
                pool[prefab].Enqueue(newObj);
            }
        }



}
