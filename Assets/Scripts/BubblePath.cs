using JsonClass;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BubblePath : BubbleObject
{
    [SerializeField] bool isLeft;
    [SerializeField] int direction;
    [SerializeField] List<BubbleObject> bubbleObjects;

    int size = 0;
    List<JsonClass.BubbleData> bubbleDatas;

    public override void Set(BubbleData bubbleData, Vector2Int target, bool isShoot = false)
    {
        bubbleDatas = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.FindAll(x => (ClientEnum.Bubble)x.type == ClientEnum.Bubble.Normal);

        if (isLeft) 
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }

        base.Set(bubbleData, target, isShoot);
    }

    public void CheckBubble(BubbleObject bubbleObject)
    {
        if (bubbleObjects.Contains(bubbleObject))
        {
            bubbleObjects.Remove(bubbleObject);
        }
    }
    public override bool isRoot => true; 

    Vector2 center;

    int GetY()
    {
        int y = 0;

        foreach (var bubble in bubbleObjects)
        {
            if (bubble.Grid.y > y)
            {
                y = bubble.Grid.y;
            }
        }

        return y;
    }

    public override void OnCreate()
    {

        while (true)
        {
            int rand = Random.Range(0, 10);
            Vector2Int grid = Vector2Int.zero;

            if (bubbleObjects.Count > 0)
            {
                grid = bubbleObjects[bubbleObjects.Count - 1].Grid;
            }
            else
            {
                grid = Grid;
            }

            if (isLeft)
            {
                if (grid.y % 2 == 0)
                {
                    grid.y += 1;

                    if (direction > 0)
                    {
                        grid.x -= 1;
                    }

                    direction *= -1;
                }
                else if (direction < 0 && (grid.x + direction < 1))
                {
                    grid.y += 1;
                }
                else if (direction > 0 && (grid.x + direction > GameManager.Instance.TargetX / 2))
                {
                    grid.y += 1;
                    grid.x += 1;
                }
                else
                {
                    grid.x += direction;
                }
            }
            else
            {
                if (grid.y % 2 == 0)
                {
                    grid.y += 1;

                    if (direction > 0)
                    {
                        grid.x -= 1;
                    }

                    direction *= -1;
                }
                else if (direction > 0 && (grid.x + direction > GameManager.Instance.TargetX))
                {
                    grid.y += 1;
                    grid.x += 1;
                }
                else if (direction < 0 && (grid.x + direction <= (GameManager.Instance.TargetX / 2) + 1))
                {
                    grid.y += 1;
                }
                else
                {
                    grid.x += direction;
                }
            }

            if (grid.y > GameManager.Instance.TargetY)
            {
                break;
            }

            BubbleObject bubbleObject = GameManager.Instance.GetBubbleObject(bubbleDatas[Random.Range(0, bubbleDatas.Count)].index, grid.x, grid.y);
           
            if (rand > 4)
            {
                bubbleObject.Properties(ClientEnum.BubbleProperty.HitBoss);
            }
            
            bubbleObjects.Add(bubbleObject);
            GameManager.Instance.Bubbles[bubbleObject.Grid] = bubbleObject;
            size = bubbleObjects.Count;
        }
    }

    public override void OnEnd()
    {
        while (size > bubbleObjects.Count)
        {
            int rand = Random.Range(0, 10);
            BubbleObject bubbleObject = GameManager.Instance.GetBubbleObject(bubbleDatas[Random.Range(0, bubbleDatas.Count)].index, Grid.x, Grid.y);

            if (rand > 4)
            {
                bubbleObject.Properties(ClientEnum.BubbleProperty.HitBoss);
            }
            bubbleObjects.Insert(0,bubbleObject);

            for (int i = 0; i < bubbleObjects.Count; i++)
            {
                Vector2Int grid = bubbleObjects[i].Grid;

                if (isLeft)
                {
                    if (grid.y % 4 == 1)
                    {
                        if (grid.x - 1 < 1)
                        {
                            grid.y += 1;
                        }
                        else
                        {
                            grid.x -= 1;
                        }
                    }
                    else if (grid.y % 2 == 0)
                    {
                        if (grid.x == 1)
                        {
                            grid.y += 1;
                        }
                        else if (grid.x > GameManager.Instance.TargetX / 2)
                        {
                            grid.y += 1;
                            grid.x -= 1;
                        }
                    }
                    else if (grid.y % 2 == 1)
                    {
                        if (grid.x + 1 <= GameManager.Instance.TargetX / 2)
                        {
                            grid.x += 1;
                        }
                        else
                        {
                            grid.y += 1;
                            grid.x += 1;
                        }
                    }
                }
                else
                {
                    if (grid.y % 4 == 1)
                    {
                        if (grid.x + 1 > GameManager.Instance.TargetX)
                        {
                            grid.x += 1;
                            grid.y += 1;
                        }
                        else
                        {
                            grid.x += 1;
                        }
                    }
                    else if (grid.y % 2 == 0)
                    {
                        if (grid.x > GameManager.Instance.TargetX)
                        {
                            grid.y += 1;
                            grid.x -= 1;
                        }
                        else
                        {
                            grid.y += 1;
                        }
                    }
                    else if (grid.y % 2 == 1)
                    {
                        if (grid.x - 1 == (GameManager.Instance.TargetX/2 + 1))
                        {
                            grid.y += 1;
                        }
                        else
                        {
                            grid.x -= 1;
                        }
                    }
                }

                bubbleObjects[i].Grid = grid;
                bubbleObjects[i].transform.localPosition = GameManager.Instance.BubblePos(grid.x, grid.y);

                GameManager.Instance.Bubbles[bubbleObjects[i].Grid] = bubbleObjects[i];
            }

        }
    }
}
