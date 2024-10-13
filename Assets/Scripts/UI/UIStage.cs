using JsonClass;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class UIStage : MonoBehaviour
{
    [SerializeField] GameObject lockObject;
    [SerializeField] Text text;
    [SerializeField] MapData target;

    public void CheckLock() { lockObject.SetActive(target.stage > DataManager.Instance.CurrentStage); }
    public void SetStage(MapData map)
    {
        target = map;
        text.text = map.stage.ToString();
    }

    public void OnClick()
    {
        GameManager.Instance.SetStage(target);
        UIManager.Instance.ClosePanel();
    }
}
