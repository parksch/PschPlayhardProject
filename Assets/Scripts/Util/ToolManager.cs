using JsonClass;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ToolManager : Singleton<ToolManager>
{
    static string jsonDatapath = "Assets/JsonFiles";

    public List<GirdRow> gridRow;
    public List<ToolGrid> visitGrid = new List<ToolGrid>();
    public List<ToolGrid> closeGrid = new List<ToolGrid>();
    public List<ToolGrid> finishedGrid = new List<ToolGrid>();
    public JsonClass.BubbleData currentBubble;
    public int x;
    public int y;

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

    public class VisitGrid
    {
        public ToolGrid current;
        public VisitGrid root;
    }

    [System.Serializable]
    public class GirdRow
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
        for (int i = 0; i < y; i++)
        {
            gridRow.Add(new GirdRow());
            gridParent.transform.position += Vector3.up;
            for (int j = 0; j < x -(i % 2 == 0 ? 0: -1); j++)
            {
                GameObject go = Instantiate(gridPrefab);
                ToolGrid toolGrid = go.GetComponent<ToolGrid>();
                go.transform.position = new Vector3(-((x / 2) + (i % 2 == 0 ? 0:0.5f)) + j, 0, 0);
                go.transform.parent = gridParent.transform;
                toolGrid.x = j + 1;
                toolGrid.y = i + 1;
                gridRow[i].toolGrids.Add(toolGrid);
                if (toolGrid.y == y)
                {
                    toolGrid.SetGreen();
                }
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
        foreach (var row in gridRow)
        {
            foreach (var tool in row.toolGrids)
            {
                if (tool.x == x && tool.y == y)
                    return tool;  
            }
        }
        return null;
    }

    public void VisitBubble(ToolGrid grid)
    {
        closeGrid.Clear();
        visitGrid.Clear();
        finishedGrid.Clear();

        foreach (var item in grid.GetAdjacentGrid())
        {
            visitGrid.Add(item);
        }

        for (int i = 0; i < visitGrid.Count; i++)
        {
            closeGrid.Clear();
            bool result = visitGrid[i].VisitBubble();

            if (!result)
            {
                foreach (var item in closeGrid)
                {
                    item.SetWhite();
                }
            }

            finishedGrid.Add(visitGrid[i]);
            visitGrid.RemoveAt(i);
            i--;
        }

        CheckBubble();
    }

    public void CheckBubble()
    {
        foreach (var row in gridRow)
        {
            foreach (var tool in row.toolGrids)
            {
                if (tool.bubble.index == 0 && tool.y != y)
                {
                    tool.SetWhite();
                }
            }
        }

        foreach (var row in gridRow)
        {
            foreach (var tool in row.toolGrids)
            {
                if (tool.y == y && tool.bubble.index == 0)
                {
                    tool.SetGreen();
                }
                else if(tool.bubble.index != 0)
                {
                    tool.OnClickButton();
                }
            }
        }
    }

    public bool IsValidPosition(int _x, int _y, int i, int j)
    {
        if (!isBubble(_x + j, _y + i))
            return false;

        bool isOddRow = (_y % 2 != 0);

        if (i == 1 || i == -1)
        {
            return (isOddRow && j > -1) || (!isOddRow && j < 1);
        }

        return true;
    }

    void DestroyGrid()
    {
        for (int i = 0; i < gridRow.Count; i++)
        {
            for (int j = 0; j < gridRow[i].toolGrids.Count; j++)
            {
                Destroy(gridRow[i].toolGrids[j].gameObject);
            }
        }

        gridRow.Clear();
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
        DestroyGrid();

        x = mapData.x;
        y = mapData.y;
        currentMap = mapData;
        mapName.text = currentMap.stage.ToString();
        saveMapButton.interactable = true;
        removeMapButton.interactable = true;

        CreateGrid();
        MapDataSetBubble();
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
            }
        }

        CheckBubble();
    }

    void AddMapDataLayout(JsonClass.MapData mapData)
    {
        if (mapData.layouts == null)
        {
           mapData.layouts = new List<Layout>();
        }

        mapData.layouts.Clear();

        for (int i = 0; i < gridRow.Count; i++)
        {
            for (int j = 0; j < gridRow[i].toolGrids.Count; j++)
            {
                if (gridRow[i].toolGrids[j].bubble.index != 0)
                {
                    ToolGrid toolGrid = gridRow[i].toolGrids[j];
                    JsonClass.Layout layout = new Layout();
                    layout.x = toolGrid.x;
                    layout.y = toolGrid.y;
                    layout.bubble = toolGrid.bubble.index;
                    mapData.layouts.Add(layout);
                }
            }
        }
    }

    public void OnClickRemove()
    {
        ScriptableManager.Instance.mapDataScriptable.mapData.Remove(currentMap);
        mapName.text = "";
        saveMapButton.interactable = false;
        removeMapButton.interactable = false;
        currentMap = null;
        x = 11;
        y = 11;
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
        mapData.x = x;
        mapData.y = y;
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
        EditorUtility.DisplayDialog("결과", "Scriptable 에서 Json 변환", "확인");
    }
}
