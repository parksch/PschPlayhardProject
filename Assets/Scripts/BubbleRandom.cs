using JsonClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleRandom : BubbleObject
{

    public override void OnCreate()
    {
        Set(ScriptableManager.Instance.bubbleDataScriptable.GetRandomBubbleOnType(ClientEnum.Bubble.Normal) ,Grid);
    }
}
