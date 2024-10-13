using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Rendering;

public class ToolManager : Singleton<ToolManager>
{
    [SerializeField] List<ToolBubbleButton> bubbleButtons = new List<ToolBubbleButton>();
    [SerializeField] ToolBubbleButton buttonPrefab;
    [SerializeField] RectTransform content;
    [SerializeField] List<GirdRow> grid;
    [SerializeField] GameObject gridParent;
    [SerializeField] GameObject gridPrefab;
    [SerializeField] float cameraSizeMin;
    [SerializeField] float cameraSizeMax;
    [SerializeField] int x;
    [SerializeField] int y;

    [System.Serializable]
    public class GirdRow
    {
        public List<ToolGrid> toolGrids = new List<ToolGrid>();
    }

    private void Start()
    {
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
            grid.Add(new GirdRow());
            gridParent.transform.position += Vector3.up;
            for (int j = 0; j < x -(i % 2 == 0 ? 0: -1); j++)
            {
                GameObject go = Instantiate(gridPrefab);
                ToolGrid toolGrid = go.GetComponent<ToolGrid>();
                go.transform.position = new Vector3(-((x / 2) + (i % 2 == 0 ? 0:0.5f)) + j, 0, 0);
                go.transform.parent = gridParent.transform;
                toolGrid.x = (x - (i % 2 == 0 ? 0 : -1)) - j;
                toolGrid.y = y - i;
                grid[i].toolGrids.Add(toolGrid);
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
}
