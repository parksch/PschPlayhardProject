using System.Collections.Generic;
using UnityEngine;

namespace JsonClass
{
    public partial class BubbleDataScriptable : ScriptableObject
    {
        public List<BubbleData> bubbleData;
    }

    [System.Serializable]
    public partial class BubbleData
    {
        public int index;
        public List<int> contain;
        public int hp;
        public string textKey;
        public string sprite;
        public string prefab;
    }

}