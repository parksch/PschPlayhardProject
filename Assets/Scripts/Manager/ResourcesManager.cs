using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{
    [SerializeField] int defaultCreateNum = 30;

    [System.Serializable]
    public class  Resources
    {
        public GameObject prefab;
        public Queue<GameObject> objects = new Queue<GameObject>();       
    }

    public void Get(string name)
    {

    }

    void CreateResource()
    {

    }
}
