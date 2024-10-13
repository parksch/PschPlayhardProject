using JsonClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolBubbleButton : MonoBehaviour
{
    [SerializeField] Image sprite;
    [SerializeField] Text text;
    [SerializeField] BubbleData target;

    public void SetTarget(BubbleData data)
    {
        target = data;
    }

    public void OnClick()
    {

    }
}
