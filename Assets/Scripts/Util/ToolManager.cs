using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToolManager : Singleton<ToolManager>
{
    [SerializeField] List<ToolBubbleButton> bubbleButtons = new List<ToolBubbleButton>();
    [SerializeField] ToolBubbleButton buttonPrefab;
    [SerializeField] RectTransform content;
    [SerializeField] GameObject gridParent;
    [SerializeField] GameObject gridPrefab;
    [SerializeField] Image bubble;
    [SerializeField] float cameraSizeMin;
    [SerializeField] float cameraSizeMax;

    public List<GirdRow> gridRow;
    public List<ToolGrid> visitGrid = new List<ToolGrid>();
    public List<ToolGrid> closeGrid = new List<ToolGrid>();
    public List<ToolGrid> finishedGrid = new List<ToolGrid>();
    public JsonClass.BubbleData currentBubble;
    public int x;
    public int y;

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
                ToolBubbleButton go = Instantiate(buttonPrefab.gameObject, content).GetComponent<ToolBubbleButton>();
                go.SetTarget(bubbleDatas[i]);
                bubbleButtons.Add(go);
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
                toolGrid.x = (x - (i % 2 == 0 ? 0 : -1)) - j;
                toolGrid.y = y - i;
                gridRow[i].toolGrids.Add(toolGrid);
                if (toolGrid.y == 1)
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

    public void OnClickSave()
    {

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
                if (tool.bubble.index == 0 && tool.y != 1)
                {
                    tool.SetWhite();
                }
            }
        }

        foreach (var row in gridRow)
        {
            foreach (var tool in row.toolGrids)
            {
                if (tool.y == 1 && tool.bubble.index == 0)
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

        if (y % 2 == 0)
        {
            isOddRow = !isOddRow;
        }

        if (i == 1 || i == -1)
        {
            return (isOddRow && j > -1) || (!isOddRow && j < 1);
        }

        return true;
    }

}
