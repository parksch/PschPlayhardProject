using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanel : PanelBase
{
    [SerializeField] RectTransform content;
    [SerializeField] UIStage prefab;
    [SerializeField] List<UIStage> stages = new List<UIStage>();

    public override void FirstLoad()
    {
        List<JsonClass.MapData> mapDatas = ScriptableManager.Instance.mapDataScriptable.mapData;

        for (int i = 0; i < mapDatas.Count; i++)
        {
            if (i == 0 )
            {
                prefab.SetStage(mapDatas[i]);
            }
            else
            {
                UIStage uIStage = Instantiate(prefab, content);
                uIStage.transform.SetAsFirstSibling();
                uIStage.SetStage(mapDatas[i]);
                stages.Add(uIStage);
            }
        }

    }

    public override void Close()
    {
    }

    public override void OnUpdate()
    {
    }

    public override void Open()
    {
        for (int i = 0; i < stages.Count; i++)
        {
            stages[i].CheckLock();
        }
    }
}
