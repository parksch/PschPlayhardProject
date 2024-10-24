using JsonClass;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using ClientEnum;
#if UNITY_EDITOR

public class ToolManager : Singleton<ToolManager>
{
    static string jsonDatapath = "Assets/JsonFiles";

    public Dictionary<Vector2Int, ToolGrid> gridDict = new Dictionary<Vector2Int, ToolGrid>();
    public List<ToolGrid> visitGrid = new List<ToolGrid>();
    public List<ToolGrid> closeGrid = new List<ToolGrid>();
    public List<ToolGrid> finishedGrid = new List<ToolGrid>();
    public JsonClass.BubbleData currentBubble;
    public int maxX;
    public int maxY;

    [SerializeField] Toggle pathToggle;
    [SerializeField] List<ToolBubbleButton> bubbleButtons = new List<ToolBubbleButton>();
    [SerializeField] ToolBubbleButton buttonPrefab;
    [SerializeField] List<ToolSave> stageButtons = new List<ToolSave>();
    [SerializeField] ToolSave savePrefab;
    [SerializeField] RectTransform bubbleContent;
    [SerializeField] RectTransform saveContent;
    [SerializeField] Text mapName;
    [SerializeField] GameObject gridParent;
    [SerializeField] GameObject gridPrefab;
    [SerializeField] Button saveMapButton;
    [SerializeField] Button removeMapButton;
    [SerializeField] Image bubble;
    [SerializeField] float cameraSizeMin;
    [SerializeField] float cameraSizeMax;
    [SerializeField] int bubbleCount;
    [SerializeField] int gameMode;

    JsonClass.MapData currentMap;

    [System.Serializable]
    public class VisitGrid
    {
        public ToolGrid current;
        public VisitGrid root;
    }

    [System.Serializable]
    public class GridRow
    {
        public List<ToolGrid> toolGrids = new List<ToolGrid>();
    }

    private void Start()
    {
        ResourcesManager.Instance.Init();
        CreateButton();
        CreateSave();
        CreateGrid();
    }

    void CreateButton()
    {
        List<JsonClass.BubbleData> bubbleDatas = ScriptableManager.Instance.bubbleDataScriptable.bubbleData;

        for (int i = 0; i < bubbleDatas.Count; i++)
        {
            if (bubbleDatas[i].Type() == ClientEnum.Bubble.NoSet)
            {
                continue;
            }

            if (i == 0)
            {
                buttonPrefab.SetTarget(bubbleDatas[i]);
            }
            else
            {
                ToolBubbleButton go = Instantiate(buttonPrefab.gameObject, bubbleContent).GetComponent<ToolBubbleButton>();
                go.SetTarget(bubbleDatas[i]);
                bubbleButtons.Add(go);
            }
        }
    }

    void CreateSave()
    {
        List<JsonClass.MapData> mapDatas = ScriptableManager.Instance.mapDataScriptable.mapData;

        for (int i = 0; i < mapDatas.Count; i++)
        {
            if (i == 0)
            {
                savePrefab.Set(mapDatas[i]);
                savePrefab.gameObject.SetActive(true);
            }
            else
            {
                ToolSave go = Instantiate(savePrefab.gameObject, saveContent).GetComponent<ToolSave>();
                go.Set(mapDatas[i]);
                stageButtons.Add(go);
            }
        }
        
    }

    void CreateGrid()
    {
        for (int i = 0; i < maxY; i++)
        {
            gridParent.transform.position += Vector3.up;
            for (int j = 0; j < maxX -(i % 2 == 0 ? 0: -1); j++)
            {
                GameObject go = Instantiate(gridPrefab);
                ToolGrid toolGrid = go.GetComponent<ToolGrid>();
                go.transform.position = new Vector3(-((maxX / 2) + (i % 2 == 0 ? 0:0.5f)) + j, 0, 0);
                go.transform.parent = gridParent.transform;
                toolGrid.gridX = j + 1;
                toolGrid.gridY = i + 1;
                gridDict[new Vector2Int(toolGrid.gridX, toolGrid.gridY)] = toolGrid;
                if (toolGrid.gridY == maxY)
                {
                    toolGrid.SetGreen();
                }
            }
        }
    }
    public void CreatePath()
    {
        if (maxX < 9)
        {
            maxX = 9;
        }

        if (maxX % 2 == 0)
        {
            maxX -= 1;
        }

        ToolGrid toolGrid = null;

        for (int y = 0; y < maxY; y++)
        {
            gridParent.transform.position += Vector3.up;
            for (int x = 0; x < maxX - (y % 2 == 0 ? 0 : -1); x++)
            {
                GameObject go = Instantiate(gridPrefab);
                toolGrid = go.GetComponent<ToolGrid>();
                go.transform.position = new Vector3(-((maxX / 2) + (y % 2 == 0 ? 0 : 0.5f)) + x, 0, 0);
                go.transform.parent = gridParent.transform;
                toolGrid.gridX = x + 1;
                toolGrid.gridY = y + 1;

                gridDict[new Vector2Int(toolGrid.gridX, toolGrid.gridY)] = toolGrid;
            }
        }

        toolGrid = FindToolGridAt((maxX / 2) + 1, 1);
        toolGrid.CreateBoss();
    }
    public void CheckBubble()
    {
        foreach (var item in gridDict.Values)
        {
            if (item.bubble.index == 0 && item.gridY != maxY)
            {
                item.SetWhite();
            }
        }

        foreach (var tool in gridDict.Values)
        {
            if (tool.gridY == maxY && tool.bubble.index == 0)
            {
                tool.SetGreen();
            }
            else if (tool.bubble.index != 0)
            {
                tool.OnClickButton();
            }
        }

    }

    private void Update()
    {
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (scrollValue > 0)
            {
                Camera.main.orthographicSize -= 0.1f;
                if (Camera.main.orthographicSize < cameraSizeMin)
                {
                    Camera.main.orthographicSize = cameraSizeMin;
                }
            }
            else if (scrollValue < 0)
            {
                Camera.main.orthographicSize += 0.1f;
                if (Camera.main.orthographicSize > cameraSizeMax)
                {
                    Camera.main.orthographicSize = cameraSizeMax;
                }
            }
        }
        else
        {
            if (scrollValue > 0)
            {
                Camera.main.transform.position -= (Vector3.up * 0.4f);
            }
            else if (scrollValue < 0)
            {
                Camera.main.transform.position += (Vector3.up * 0.4f);
            }
        }
    }

    public void SetCurrentBubble(JsonClass.BubbleData current)
    {
        currentBubble = current;
        bubble.sprite = ResourcesManager.Instance.GetSprite(currentBubble.atlas,currentBubble.sprite);
    }

    public bool isBubble(int x,int y)
    {
        var toolGrid = FindToolGridAt(x, y);
        if (toolGrid == null) return false; 

     
        if (closeGrid.Contains(toolGrid))
            return false;

        return toolGrid.isBubble();
    }

    public ToolGrid FindToolGridAt(int x, int y)
    {
        if (!gridDict.ContainsKey(new Vector2Int(x,y)))
        {
            return null;
        }

        return gridDict[new Vector2Int(x, y)];
    }

    public void VisitBubble()
    {
        closeGrid.Clear();
        visitGrid.Clear();
        finishedGrid.Clear();

        foreach (var item in gridDict.Values)
        {
            if (item.isBubble())
            {
                visitGrid.Add(item);
            }
        }

        for (int i = 0; i < visitGrid.Count; i++)
        {
            closeGrid.Clear();
            bool result = visitGrid[i].VisitBubble();

            if (visitGrid.Count > 0)
            {
                visitGrid.RemoveAt(i);
                i--;
            }

            if (!result)
            {
                foreach (var item in closeGrid)
                {
                    if (visitGrid.Contains(item))
                    {
                        visitGrid.Remove(item);
                    }
                    item.SetWhite();
                }
            }
        }

        CheckBubble();
    }


    public bool IsValidPosition(int _x, int _y, int xOffset, int yOffset)
    {
        if (!isBubble(_x + xOffset, _y + yOffset))
            return false;

        bool isOddRow = (_y % 2 != 0);

        if (yOffset == 1 || yOffset == -1)
        {
            return (isOddRow && xOffset > -1) || (!isOddRow && xOffset < 1);
        }

        return true;
    }

    void DestroyGrid()
    {
        foreach (var item in gridDict.Values)
        {
            Destroy(item.gameObject);
        }

        gridDict.Clear();
    }

    void RemoveSaveButton()
    {
        for(int i = 0;i < stageButtons.Count;i++)
        {
            if (i == 0)
            {
                stageButtons[i].gameObject.SetActive(false);
            }
            else
            {
                Destroy(stageButtons[i].gameObject);
            }
        }

        stageButtons.Clear();
        stageButtons.Add(savePrefab);
    }

    public void LoadMap(MapData mapData)
    {
        if (mapData.gameMode == 2)
        {
            pathToggle.isOn = true;
        }
        else
        {
            pathToggle.isOn = false;
        }

        DestroyGrid();

        gameMode = mapData.gameMode;
        maxX = mapData.x;
        maxY = mapData.y;
        currentMap = mapData;
        mapName.text = currentMap.stage.ToString();
        saveMapButton.interactable = true;
        removeMapButton.interactable = true;

        if (gameMode == 2)
        {
            CreatePath();
        }
        else
        {
            CreateGrid();
            MapDataSetBubble();
        }
    }

    void MapDataSetBubble()
    {
        for (int i = 0; i < currentMap.layouts.Count; i++)
        {
            ToolGrid toolGrid = FindToolGridAt(currentMap.layouts[i].x, currentMap.layouts[i].y);

            if (toolGrid == null)
            {
                continue;
            }

            if (currentMap.layouts[i].bubble == 0)
            {
                toolGrid.SetWhite();
            }
            else
            {
                toolGrid.SetBubble(ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == currentMap.layouts[i].bubble));
                toolGrid.OnClickButton();
            }
        }

        CheckBubble();
    }

    void AddMapDataLayout(JsonClass.MapData mapData)
    {
        if (mapData.layouts == null)
        {
           mapData.layouts = new List<Layouts>();
        }

        mapData.layouts.Clear();

        foreach (var item in gridDict.Values)
        {
            ToolGrid toolGrid = item;
            JsonClass.Layouts layout = new Layouts();
            layout.x = toolGrid.gridX;
            layout.y = toolGrid.gridY;
            layout.bubble = toolGrid.bubble.index;
            mapData.layouts.Add(layout);
        }
    }

    public void OnClickRemove()
    {
        ScriptableManager.Instance.mapDataScriptable.mapData.Remove(currentMap);
        mapName.text = "";
        saveMapButton.interactable = false;
        removeMapButton.interactable = false;
        currentMap = null;
        maxX = 11;
        maxY = 11;
        DestroyGrid();
        RemoveSaveButton();
        CreateGrid();
        CreateSave();
    }

    public void OnClickSave()
    {
        AddMapDataLayout(currentMap);
        SaveJson();
    }

    public void OnClickNewSave()
    {
        JsonClass.MapData mapData = new JsonClass.MapData();
        mapData.x = maxX;
        mapData.y = maxY;
        mapData.stage = ScriptableManager.Instance.mapDataScriptable.mapData.Count + 1;
        mapData.name = "stagename";
        mapData.bubbleCount = bubbleCount;
        mapData.gameMode = gameMode;
        currentMap = mapData;
        mapName.text = currentMap.stage.ToString();
        AddMapDataLayout(mapData);
        ScriptableManager.Instance.mapDataScriptable.mapData.Add(mapData);
        RemoveSaveButton();
        CreateSave();
        SaveJson();
    }

    void SaveJson()
    {
        List<JsonClass.MapData> mapDatas = ScriptableManager.Instance.mapDataScriptable.mapData;
        string json = JsonConvert.SerializeObject(mapDatas);
        string path = string.Format("{0}/{1}", jsonDatapath, "MapData.json");
        File.WriteAllText(path, json.ToString(), Encoding.UTF8);

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    public void OnClickToggle()
    {
        DestroyGrid();

        if (pathToggle.isOn)
        {
            gameMode = 2;
            CreatePath();
        }
        else
        {
            gameMode = 1;
            CreateGrid();
        }
    }

}

#endif 