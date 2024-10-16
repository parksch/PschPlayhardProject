using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : PanelBase
{
    [SerializeField] Text title;
    [SerializeField] Text resultText;

    public override void Close()
    {

    }

    public override void FirstLoad()
    {

    }

    public override void OnUpdate()
    {

    }

    public override void Open()
    {
        switch (GameManager.Instance.Status)
        {
            case ClientEnum.GameStatus.Clear:
                title.text = "Clear";
                resultText.text = "Next";
                break;
            case ClientEnum.GameStatus.Fail:
                title.text = "Fail";
                resultText.text = "Retry";
                break;
            default:
                break;
        }

    }

    public void OnClickReturn()
    {
        UIManager.Instance.OpenPanel(UIManager.Instance.GetPanel<SelectPanel>());
    }

    public void OnClickRight()
    {
        GameManager.Instance.SetStage(ScriptableManager.Instance.mapDataScriptable.mapData.Find(x => x.stage == DataManager.Instance.CurrentStage));
    }
}
