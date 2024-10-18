using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] int currentStage;

    public int CurrentStage
    {
        get
        {
            return currentStage;
        }
        set
        {
            JsonClass.MapData map = ScriptableManager.Instance.mapDataScriptable.mapData.Find(x => x.stage == value);

            if (map != null && currentStage == map.stage)
            {
                currentStage = map.next;
            }
        }
    
    }

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        
    }

    public void Init()
    {
        currentStage = 1;
    }
}
