using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PickUpItem
{
    public int index;
    public int amountToPool;
    public bool shouldExpand = true;
    public GameObject objectToPool;
    public Material material;
    public Transform parentTransform;

    //public PickUpItem(GameObject obj, int amt, bool exp = true)
    //{
    //    this.objectToPool = obj;
    //    amountToPool = Mathf.Max(amt, 2);
    //    shouldExpand = exp;
    //}

    //public PickUpItem(GameObject obj, bool exp = true)
    //{
    //    this.objectToPool = obj;
    //    shouldExpand = exp;
    //}

    public PickUpItem(int index, GameObject obj, Transform parentTransform, Material mat)
    {
        this.objectToPool = obj;
        this.index = index;
        this.parentTransform = parentTransform;
        this.material = mat;
    }

    //public PickUpItem(int index)
    //{
    //    this.index = index;
    //}
}

public class PickUpsPooler : Singleton<PickUpsPooler>
{
    private List<PickUpItem> itemsToPool;
    private List<List<GameObject>> pooledObjectsList;
    private List<GameObject> pooledObjects;
    public GameObject pickUpPrefab;

    [HideInInspector] public GameObject pickUpCustomized;

    private int pooledObjectParentIndex;

    public PickUpSkins pickUpSkins;
    public IntVariable pickUpSkinActiveIndex;

    public void makePool(int amountToPool, Humanoid[] humanoids)
    {
        int characterCount = humanoids.Length;
        Material[] charMats = new Material[characterCount];

        for (int i = 0; i < characterCount; i++)
        {
            charMats[i] = humanoids[i].defaultSkinRenderer.material;
        }

        pickUpCustomized = Instantiate(pickUpPrefab) as GameObject;
        Instantiate(pickUpSkins.availableSkins[pickUpSkinActiveIndex.Value].skinPrefab, pickUpCustomized.FindComponentInChildWithTag<Transform>("gfx"));

        itemsToPool = new List<PickUpItem>();

        pickUpCustomized.transform.position = Vector3.one * -5000f;

        pooledObjectsList = new List<List<GameObject>>();
        pooledObjects = new List<GameObject>();

        for (int i = 0; i < characterCount; i++)
        {
            ObjectPoolItemToPooledObject(i, amountToPool, charMats[i]);
        }
    }

    public GameObject GetPooledObject(int index)
    {
        bool foundThePool = false;
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (itemsToPool[i].index == index)
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

            obj.transform.parent = itemsToPool[pooledObjectParentIndex].parentTransform;

            obj.transform.localPosition = Vector3.zero;

            obj.GetComponent<PickUp>().initializePickUpValues(pooledObjectParentIndex, itemsToPool[pooledObjectParentIndex].material);

            obj.SetActive(false);

            pooledObjects.Add(obj);
            return obj;
        }
        return null;
    }

    public void repoolAllObjects(int index)
    {
        bool foundThePool = false;
        for (int i = 0; i < itemsToPool.Count; i++)
        {
            if (itemsToPool[i].index == index)
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

    public void Event_OnHumanoidEliminated_Self(HumanoidElimination humanoidElimination)
    {
        Transform pool = transform.GetChild(humanoidElimination.eliminatedHumanoidKey);
        for (int i = pool.childCount - 1; i >= 0; i--)
        {
            pool.GetChild(i).GetComponent<PickUp>().greyOutLadderPickUp();
        }
    }

    public void Event_OnHumanoidEliminated_ByOther(HumanoidElimination humanoidElimination)
    {
        Transform pool = transform.GetChild(humanoidElimination.eliminatedHumanoidKey);
        for (int i = pool.childCount - 1; i >= 0; i--)
        {
            pool.GetChild(i).GetComponent<PickUp>().greyOutLadderPickUp(humanoidElimination.lastDamagerKey, humanoidElimination.lastDamagerMat);
        }
    }

    void ObjectPoolItemToPooledObject(int index_key, int amountToPool, Material mat)
    {
        Transform parentTransform = new GameObject().transform;
        parentTransform.name = index_key.ToString();
        parentTransform.parent = this.transform;

        pooledObjects = new List<GameObject>();

        for (int j = 0; j < amountToPool; j++)
        {
            GameObject obj = (GameObject)Instantiate(pickUpCustomized);

            obj.transform.parent = parentTransform;

            obj.transform.localPosition = Vector3.zero;

            obj.GetComponent<PickUp>().initializePickUpValues(index_key, mat);

            obj.SetActive(false);

            pooledObjects.Add(obj);
        }

        pooledObjectsList.Add(pooledObjects);

        itemsToPool.Add(new PickUpItem(index_key, pickUpCustomized, parentTransform, mat));
    }
}
