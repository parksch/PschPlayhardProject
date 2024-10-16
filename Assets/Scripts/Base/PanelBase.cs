using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase : MonoBehaviour 
{
    [SerializeField] bool isTranslucent;
    
    public bool IsTranslucent => isTranslucent;
    public abstract void FirstLoad();
    public abstract void OnUpdate();
    public abstract void Open();
    public abstract void Close();
}
