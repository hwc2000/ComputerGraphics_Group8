using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class objectPooler : MonoBehaviour
{

    public static objectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        GameObject tem;
            
        for(int i = 0; i < amountToPool; i++)
        {
            tem = Instantiate(objectToPool);
            tem.SetActive(false);
            pooledObjects.Add(tem);
        }

    }

        
    public GameObject GetpooledObject()
    {
        for(int i = 0; i < amountToPool; i++)
        {
            if (pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }

        return null;
    }

}
