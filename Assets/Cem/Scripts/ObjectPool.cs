using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class ObjectPool : MonoBehaviour
    {
        public List<GameObject> prefabList;
        public static ObjectPool Instance;
        private readonly Dictionary<GameObject, Queue<GameObject>> pool = new Dictionary<GameObject, Queue<GameObject>>();
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
            foreach (var obj in prefabList)
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
            
            obj.gameObject.SetActive(true);
            if (obj.TryGetComponent<IPoolObject>(out IPoolObject poolObject))
            {
                poolObject.Initialize();
            }
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
            if(pool.TryGetValue(prefab,out Queue<GameObject> que))
            {
                que.Enqueue(newObj);

            }
            else
            {
                pool.Add(prefab, new Queue<GameObject>());
            }
        }
    }



}
