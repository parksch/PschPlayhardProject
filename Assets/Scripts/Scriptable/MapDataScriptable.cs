using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

namespace JsonClass
{
    public partial class MapDataScriptable : ScriptableObject
    {
        public List<MapData> mapData;
    }

    [System.Serializable]
    public partial class MapData
    {
        public int x;
        public int y;
        public int stage;
        public string name;
        public int gameMode;
        public int bubbleCount;
        public List<Layout> layouts;
    }

    [System.Serializable]
    public partial class Layout
    {
        public int y;
        public int x;
        public int bubble;
    }
}
