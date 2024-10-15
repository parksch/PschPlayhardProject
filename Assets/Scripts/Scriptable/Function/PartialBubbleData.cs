using System.Collections.Generic;
using UnityEngine;

namespace JsonClass
{
    public partial class BubbleDataScriptable
    {
        public ClientEnum.Bubble GetBubbleType(int index) { return bubbleData.Find(x => x.index == index).Type(); }
    }

    public partial class BubbleData
    {
        public Sprite Sprite() => ResourcesManager.Instance.GetSprite(atlas, sprite);
        public ClientEnum.Bubble Type() => (ClientEnum.Bubble)type;
    }

}
