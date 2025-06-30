using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public enum PoolableItems
{
    LadderGFX,
    Bomb,
}

[Serializable]
public class ObjectPoolItem
{
    // see https://github.com/Rfrixy/Generic-Unity-Object-Pooler

    public PoolableItems name;
    public GameObject objectToPool;
    public int amountToPool;
    public bool shouldExpand = true;

    public ObjectPoolItem(GameObject obj, int amt, bool exp = true)
    {
        objectToPool = obj;
        amountToPool = Mathf.Max(amt, 2);
        shouldExpand = exp;
    }
}

public class ObjectPooler : Singleton<ObjectPooler>
{
    public List<ObjectPoolItem> itemsToPool;

    public List<List<GameObject>> pooledObjectsList;
    private List<GameObject> pooledObjects;
    private int pooledObjectParentIndex;

    public BombSkins bombSkins;

    public IntVariable bombSkinSkinActiveIndex;

    protected override void Awake()
    {
        base.Awake();
        pooledObjectsList = new List<List<GameObject>>();
        pooledObjects = new List<GameObject>();
    }

    public void Event_MakePool()
    {
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            ObjectPoolItemToPooledObject(i);
        }
    }

    public GameObject GetPooledObject(PoolableItems pooledItem)
    {
        bool foundThePool = false;
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (itemsToPool[i].name == pooledItem)
            {
                pooledObjectParentIndex = i;
                foundThePool = true;
                break;
            }
        }

        if (!foundThePool)
        {
            Debug.LogError("You tried to access a non existing pool");
            return null;
        }

        int sizeOfPool = pooledObjectsList[pooledObjectParentIndex].Count;

        for (int i = 0; i < sizeOfPool; i++)
        {
            if (!pooledObjectsList[pooledObjectParentIndex][i % sizeOfPool].activeInHierarchy)
            {
                //positions[index] = i % curSize;
                return pooledObjectsList[pooledObjectParentIndex][i % sizeOfPool];
            }
        }

        if (itemsToPool[pooledObjectParentIndex].shouldExpand)
        {
            GameObject obj = (GameObject)Instantiate(itemsToPool[pooledObjectParentIndex].objectToPool);
            obj.SetActive(false);
            obj.transform.parent = this.transform.GetChild(pooledObjectParentIndex);
            pooledObjectsList[pooledObjectParentIndex].Add(obj);
            return obj;
        }
        return null;
    }

    public List<GameObject> GetAllPooledObjects(int index)
    {
        return pooledObjectsList[index];
    }

    public void repoolObjects(PoolableItems repool)
    {
        bool foundThePool = false;
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (itemsToPool[i].name == repool)
            {
                pooledObjectParentIndex = i;
                foundThePool = true;
                break;
            }
        }

        if (!foundThePool)
        {
            Debug.LogError("You tried to access a non existing pool");
            return;
        }

        int sizeOfPool = pooledObjectsList[pooledObjectParentIndex].Count;

        for (int i = 0; i < sizeOfPool; i++)
        {
            pooledObjectsList[pooledObjectParentIndex][i % sizeOfPool].SetActive(false);

        }
    }


    public int AddObject(GameObject GO, int amt = 3, bool exp = true)
    {
        ObjectPoolItem item = new ObjectPoolItem(GO, amt, exp);
        int currLen = itemsToPool.Count;
        itemsToPool.Add(item);
        ObjectPoolItemToPooledObject(currLen);
        return currLen;
    }


    void ObjectPoolItemToPooledObject(int index)
    {
        ObjectPoolItem item = itemsToPool[index];

        if (item.name == PoolableItems.Bomb)
        {
            GameObject bombCustomized = Instantiate(item.objectToPool) as GameObject;
            Instantiate(bombSkins.availableSkins[bombSkinSkinActiveIndex.Value].skinPrefab, bombCustomized.FindComponentInChildWithTag<Transform>("gfx"));
            item.objectToPool = bombCustomized;

            bombCustomized.FindChildWithTagBreadthFirst<Transform>("BombPulse").localScale = bombSkins.availableSkins[bombSkinSkinActiveIndex.Value].pulseLocalScale;

            bombCustomized.transform.position = Vector3.one * -5000;
            //Destroy(bombCustomized);
        }

        Transform parentTransform = new GameObject().transform;
        parentTransform.name = itemsToPool[index].name.ToString();
        parentTransform.parent = this.transform;

        pooledObjects = new List<GameObject>();
        for (int i = 0; i < item.amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(item.objectToPool);

            if (obj.TryGetComponent<IMustInitialize>(out IMustInitialize t))
            {
                t.Initialize();
            }

            obj.SetActive(false);
            obj.transform.parent = parentTransform;

            pooledObjects.Add(obj);
        }
        pooledObjectsList.Add(pooledObjects);

        //positions.Add();

    }
}
