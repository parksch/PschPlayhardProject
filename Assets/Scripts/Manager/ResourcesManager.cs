using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourcesManager : Singleton<ResourcesManager>
{
    [SerializeField] int defaultCreateNum = 30;
    Dictionary<string,SpriteAtlas> atlasDict = new Dictionary<string,SpriteAtlas>();
    Dictionary<string, ResourcesObject> gameObjectDict = new Dictionary<string, ResourcesObject>();

    public void Init()
    {
        var spriteAtlas = Resources.LoadAll<SpriteAtlas>("SpriteAtlas");

        foreach (var item in spriteAtlas)
        {
            atlasDict[item.name] = item;
        }
    }

    public Sprite GetSprite(string atlasName,string id)
    {
        return atlasDict[atlasName].GetSprite(id);
    }

    [System.Serializable]
    public class  ResourcesObject
    {
        public GameObject prefab;
        public Queue<GameObject> objects = new Queue<GameObject>();       
    }

    public GameObject Get(string name)
    {
        if (!gameObjectDict.ContainsKey(name))
        {
            ResourcesObject resourcesObject = new ResourcesObject();
            gameObjectDict[name] = resourcesObject;
            resourcesObject.prefab = Resources.Load<GameObject>("Prefab/" + name);
        }

        if (gameObjectDict[name].objects.Count == 0)
        {
            CreateResource(gameObjectDict[name]);
        }

        GameObject gameObject = gameObjectDict[name].objects.Dequeue();

        return gameObject;
    }

    public void Push(string name,GameObject gameObject)
    {
        gameObject.SetActive(false);
        gameObject.transform.parent = transform;
        gameObject.transform.localPosition = Vector3.zero;

        gameObjectDict[name].objects.Enqueue(gameObject);
    }

    void CreateResource(ResourcesObject resourcesObject)
    {
        for (int i = 0; i < defaultCreateNum; i++)
        {
            GameObject gameObject = Instantiate(resourcesObject.prefab,transform);
            gameObject.name = resourcesObject.prefab.name;
            gameObject.SetActive(false);
            resourcesObject.objects.Enqueue(gameObject);
        }
    }
}
