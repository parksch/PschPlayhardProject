using JsonClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
public class ToolBubbleButton : MonoBehaviour
{
    [SerializeField] Image sprite;
    [SerializeField] Text text;
    [SerializeField] BubbleData target;

    public void SetTarget(BubbleData data)
    {
        target = data;
        sprite.sprite = ResourcesManager.Instance.GetSprite(target.atlas,target.sprite);
        text.text = target.index.ToString();
    }

    public void OnClick()
    {
        ToolManager.Instance.SetCurrentBubble(target);
    }
}
# endif