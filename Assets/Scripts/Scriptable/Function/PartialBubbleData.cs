using System.Collections.Generic;
using UnityEngine;

namespace JsonClass
{
    public partial class BubbleDataScriptable
    {
        public ClientEnum.Bubble GetBubbleType(int index) { return bubbleData.Find(x => x.index == index).Type(); }

        public JsonClass.BubbleData GetRandomBubbleOnType(ClientEnum.Bubble bubble)
        {
            List<JsonClass.BubbleData> bubbleDatas =  bubbleData.FindAll(x => (ClientEnum.Bubble)x.type == bubble);
            return bubbleDatas[Random.Range(0, bubbleDatas.Count)];
        }
    }

    public partial class BubbleData
    {
        public Sprite Sprite() => ResourcesManager.Instance.GetSprite(atlas, sprite);
        public ClientEnum.Bubble Type() => (ClientEnum.Bubble)type;
    }

}
