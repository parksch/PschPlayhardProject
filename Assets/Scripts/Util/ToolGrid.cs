using JsonClass;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.EditorTools;
using UnityEngine;

public class ToolGrid : MonoBehaviour
{
    public int x;
    public int y;
    public BubbleData bubble;
    public SpriteRenderer circleRenderer;
    public SpriteRenderer front;

    [SerializeField] bool isEnter = false;
    [SerializeField] Color green;
    [SerializeField] Color white;
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
        if (bubble.index != 0 && y == ToolManager.Instance.y)
        {
            return true;
        }

        ToolManager.Instance.closeGrid.Add(this);

        bool result = false;
        List<ToolGrid> toolGrids = GetAdjacentGrid();

        for (int i = 0; i < toolGrids.Count; i++)
        {
            if (!ToolManager.Instance.closeGrid.Contains(toolGrids[i]))
            {
                result = toolGrids[i].VisitBubble();
                if (result)
                {
                    foreach (var item in toolGrids)
                    {
                        if (!ToolManager.Instance.finishedGrid.Contains(item) && ToolManager.Instance.closeGrid.Contains(item))
                        {
                            ToolManager.Instance.visitGrid.Add(item);
                        }
                    }
                    break;
                }
            }
        }

        return result;
    }

    public List<ToolGrid> GetAdjacentGrid()
    {
        List<ToolGrid> toolGrids = new List<ToolGrid>();

        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                if (ToolManager.Instance.IsValidPosition(x, y, i, j))
                {
                    toolGrids.Add(ToolManager.Instance.FindToolGridAt((x + j), (y + i)));
                }
            }
        }

        return toolGrids;
    }

    public void OnClickButton()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                bool isOddRow = (y % 2 != 0);

                if ((i != 0 && ((isOddRow && j > -1) || (!isOddRow && j < 1))) || i == 0)
                {
                    ToolGrid tool = ToolManager.Instance.FindToolGridAt(x + j, y + i);
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
        OnClickButton();
    }

    private void Update()
    {
        if (isEnter && isOn && ToolManager.Instance.currentBubble.index != 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetBubble(ToolManager.Instance.currentBubble);
            }
            else if (Input.GetMouseButtonDown(1) && bubble.index != 0)
            {
                bubble.index = 0;
                front.gameObject.SetActive(false);
                ToolManager.Instance.VisitBubble(this);
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
}
