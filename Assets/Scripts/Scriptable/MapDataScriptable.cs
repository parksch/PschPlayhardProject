using System.Collections.Generic;
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
        public int next;
        public string name;
        public int gameMode;
        public int bubbleCount;
        public List<Layouts> layouts;
    }

    [System.Serializable]
    public partial class Layouts
    {
        public int y;
        public int x;
        public int bubble;
    }

}
