using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
public class ToolSave : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] JsonClass.MapData target;
    public void Set(JsonClass.MapData mapData)
    {
        target = mapData;
        text.text = mapData.stage.ToString();
    }

    public void OnClick()
    {
        ToolManager.Instance.LoadMap(target);
    }
}
#endif