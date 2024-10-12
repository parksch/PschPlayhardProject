using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    [SerializeField] int currentStage;

    public int CurrentStage => currentStage;

    private void Start()
    {
        
    }

    public void Init()
    {
        currentStage = 0;
    }
}
