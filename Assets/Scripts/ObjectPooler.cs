using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public List<GameObject> Prefabs; // Change to list to hold multiple prefabs
        public int Size;
    }

    public List<Pool> Pools;
    private Dictionary<string, Queue<GameObject>> PoolDictionary;

    void Awake()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in Pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; i++)
            {
                try
                {
                    GameObject prefabToInstantiate = pool.Prefabs[UnityEngine.Random.Range(0, pool.Prefabs.Count)];
                    GameObject obj = Instantiate(prefabToInstantiate, this.gameObject.transform.parent);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error during object instantiation: {ex.Message}");
                }
            }

            PoolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public GameObject GetPooledObject(string tag)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }
        if (PoolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"Pool with tag {tag} is empty.");
            return null;
        }

       try
       { GameObject objectToSpawn = PoolDictionary[tag].Dequeue();
        objectToSpawn.transform.SetParent(null);
        objectToSpawn.SetActive(true);
        return objectToSpawn;
       }catch(Exception e){
        Debug.LogError($"{tag}Queue is empty!:: {e}");
        return null;
       }
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {   
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return;
        }
        if(objectToReturn != null)
        {
        objectToReturn.transform.SetParent(this.gameObject.transform.parent); 
        objectToReturn.SetActive(false);        
        PoolDictionary[tag].Enqueue(objectToReturn);}
        else{
            print($"Object to Return is null! {objectToReturn}");
        }
    }
}