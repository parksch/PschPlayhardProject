using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ResourcesManager : Singleton<ResourcesManager>
{
    [SerializeField] int defaultCreateNum = 30;
    Dictionary<string,SpriteAtlas> atlas = new Dictionary<string,SpriteAtlas>();

    public void Init()
    {
        var spriteAtlas = Resources.LoadAll<SpriteAtlas>("SpriteAtlas");

        foreach (var item in spriteAtlas)
        {
            atlas[item.name] = item;
        }
    }

    public Sprite GetSprite(string atlasName,string id)
    {
        return atlas[atlasName].GetSprite(id);
    }

    [System.Serializable]
    public class  ResourcesObject
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
