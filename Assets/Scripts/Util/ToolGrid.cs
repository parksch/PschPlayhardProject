using JsonClass;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
public class ToolGrid : MonoBehaviour
{
    public int gridX;
    public int gridY;
    public BubbleData bubble;
    public SpriteRenderer circleRenderer;
    public SpriteRenderer front;

    [SerializeField] bool isEnter = false;
    [SerializeField] Color green;
    [SerializeField] Color white;
    [SerializeField] Color red;
    bool isOn;

    public void SetGreen()
    {
        circleRenderer.material.color = green;
        isOn = true;
    }

    public void SetWhite()
    {
        circleRenderer.material.color = white;
        bubble.index = 0;
        isOn = false;
        front.gameObject.SetActive(false);
    }

    public void SetRed()
    {
        circleRenderer.material.color = red;
        bubble.index = 0;
        isOn = false;
        front.gameObject.SetActive(false);
    }

    public bool isBubble()
    {
        if (bubble.index == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool VisitBubble()
    {
        ToolManager.Instance.closeGrid.Add(this);

        if (bubble.index != 0 && gridY == ToolManager.Instance.maxY)
        {
            return true;
        }

        bool result = false;
        List<ToolGrid> toolGrids = GetAdjacentGrid();

        for (int i = 0; i < toolGrids.Count; i++)
        {
            if (!ToolManager.Instance.closeGrid.Contains(toolGrids[i]))
            {
                result = toolGrids[i].VisitBubble();

                if (result)
                {
                    break;
                }
            }
        }

        return result;
    }

    public List<ToolGrid> GetAdjacentGrid()
    {
        List<ToolGrid> toolGrids = new List<ToolGrid>();

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (y == 0 && x == 0)
                {
                    continue;
                }

                if (ToolManager.Instance.IsValidPosition(gridX, gridY, x, y))
                {
                    toolGrids.Add(ToolManager.Instance.FindToolGridAt((gridX + x), (gridY + y)));
                }
            }
        }

        return toolGrids;
    }

    public void OnClickButton()
    {
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (y == 0 && x == 0)
                {
                    continue;
                }

                bool isOddRow = (gridY % 2 != 0);

                if ((y != 0 && ((isOddRow && x > -1) || (!isOddRow && x < 1))) || y == 0)
                {
                    ToolGrid tool = ToolManager.Instance.FindToolGridAt(gridX + x, gridY + y);
                    if (tool != null)
                    {
                        tool.SetGreen();
                    }
                }
            }
        }
    }

    public void SetBubble(JsonClass.BubbleData target)
    {
        bubble.index = target.index;
        bubble.contain = target.contain;
        bubble.hp = target.hp;
        bubble.textKey = target.textKey;
        bubble.atlas = target.atlas;
        bubble.sprite = target.sprite;
        bubble.prefab = target.prefab;
        front.sprite = ResourcesManager.Instance.GetSprite(bubble.atlas, bubble.sprite);
        front.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (isEnter && isOn)
        {
            if (Input.GetMouseButtonDown(0) && ToolManager.Instance.currentBubble.index != 0)
            {
                SetBubble(ToolManager.Instance.currentBubble);
                OnClickButton();
            }
            else if (Input.GetMouseButtonDown(1) && bubble.index != 0)
            {
                bubble.index = 0;
                front.gameObject.SetActive(false);
                ToolManager.Instance.VisitBubble();
            }
        }
    }
    private void OnMouseEnter()
    {
        isEnter = true;
    }
    private void OnMouseExit()
    {
        isEnter = false;
    }

    public void CreateBoss()
    {
        SetBubble(ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == 6));
        ToolGrid toolGrid = null;

        toolGrid = ToolManager.Instance.FindToolGridAt(gridX - 2, gridY);
        toolGrid.SetBubble(ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == 7));

        toolGrid = ToolManager.Instance.FindToolGridAt(gridX + 2, gridY);
        toolGrid.SetBubble(ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == 8));

        for (int y = 0; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                bool isOdd = gridY % 2 != 0;

                if (y != 0 && ((isOdd && x == -1) || (!isOdd && x == 1)))
                {
                    continue;
                }

                toolGrid = ToolManager.Instance.FindToolGridAt(gridX + x,gridY + y);

                if (toolGrid != null)
                {
                    toolGrid.SetRed();
                }
            }
        }

    }
}
#endif