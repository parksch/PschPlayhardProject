using ClientEnum;
using JsonClass;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using TMPro.EditorUtilities;
using Unity.Android.Types;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR;


public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameStatus status;
    [SerializeField] GameMode mode;
    [SerializeField] GameObject bubbleGameObject;
    [SerializeField] Transform gameObjectParent;
    [SerializeField] Transform bubbleParent;
    [SerializeField] List<Transform> setBubble;
    [SerializeField] List<BubbleObject> shootBubbles;
    [SerializeField] Dictionary<Vector2Int,BubbleObject> bubbles = new Dictionary<Vector2Int, BubbleObject>();
    [SerializeField] List<BubbleObject> visitBubbles = new List<BubbleObject>();
    [SerializeField] List<BubbleObject> closeBubbles = new List<BubbleObject>();
    [SerializeField] List<int> shootBubbleID = new List<int>();
    [SerializeField] BubbleObject prefab;
    [SerializeField] float bubbleSize = 1;
    [SerializeField] int bubbleCount;
    [SerializeField] int horizontal = 10;
    [SerializeField] int targetX, targetY;
    [SerializeField] int stage;

    public Transform GameObjectParent => gameObjectParent;
    public GameStatus Status
    {
        get { return status; }
        set { status = value; }
    }
    public float CurrentBubbleRadius => shootBubbles[0].Radius;

    protected override void Awake()
    {
            
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        status  = GameStatus.Selection;
        UIManager.Instance.Init();
    }

    public void BubbleShoot(Vector3 normal)
    {
        status = GameStatus.Running;
        bubbleCount--;
        UIManager.Instance.SetBubbleCount(bubbleCount);
        shootBubbles[0].Shoot(normal);
        shootBubbles[0] = null;
    }

    public void SetStage(MapData mapData)
    {
        stage = mapData.stage;
        targetX = mapData.x;
        targetY = mapData.y;
        bubbleCount = mapData.bubbleCount;
        mode = mapData.GameMode();
        bubbles = CreateBubbles(mapData.layouts);
        CheckShootBubble();
        CreateShootBubble();
        bubbleGameObject.SetActive(true);
        UIManager.Instance.SetBubbleCount(bubbleCount);
        status = GameStatus.Play;
    }

    public void CheckShootBubble()
    {
        shootBubbleID.Clear();

        foreach (var item in bubbles)
        {
            if (!shootBubbleID.Contains(item.Value.ID) && ScriptableManager.Instance.bubbleDataScriptable.GetBubbleType(item.Value.ID) == ClientEnum.Bubble.Normal)
            {
                shootBubbleID.Add(item.Value.ID);
            }
        }   
    }

    public void CreateShootBubble()
    {
        for (int i = 0; i < shootBubbles.Count; i++)
        {
            if (i >= bubbleCount)
            {
                break;
            }

            if (shootBubbles[i] == null)
            {
                for(int j = i + 1; j < shootBubbles.Count; j++)
                {
                    if (shootBubbles[j] != null && shootBubbles[i] == null)
                    {
                        shootBubbles[i] = shootBubbles[j];
                        shootBubbles[i].transform.position = setBubble[i].transform.position;

                        if (i == 0)
                        {
                            shootBubbles[i].transform.localScale = Vector3.one * bubbleSize;
                        }
                        else
                        {
                            shootBubbles[i].transform.localScale = (Vector3.one * bubbleSize) * 0.5f;
                        }

                        shootBubbles[j] = null;

                        if (!shootBubbleID.Contains(shootBubbles[i].ID))
                        {
                            int rand = shootBubbleID[Random.Range(0, shootBubbleID.Count)];
                            JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == rand);
                            shootBubbles[i].Set(bubbleData, Vector2Int.zero, true);
                        }
                    }
                }
            }
            else if (!shootBubbleID.Contains(shootBubbles[i].ID))
            {
                int rand = shootBubbleID[Random.Range(0, shootBubbleID.Count)];
                JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == rand);
                shootBubbles[i].Set(bubbleData, Vector2Int.zero, true);
            }
        }

        for (int i = 0; i < shootBubbles.Count; i++)
        {
            if (i < bubbleCount && shootBubbles[i] == null)
            {
                shootBubbles[i] = GetShootBubble();
                shootBubbles[i].transform.position = setBubble[i].transform.position;
                if (i == 0)
                {
                    shootBubbles[i].transform.localScale = Vector3.one * bubbleSize;
                }
                else
                {
                    shootBubbles[i].transform.localScale *= 0.5f;
                }
                shootBubbles[i].transform.parent = GameObjectParent;
                shootBubbles[i].gameObject.SetActive(true);
            }
        }
    }

    BubbleObject GetShootBubble()
    {
        int rand = shootBubbleID[Random.Range(0, shootBubbleID.Count)];
        JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == rand);
        BubbleObject bubble = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();
        bubble.Set(bubbleData, Vector2Int.zero,true);
        bubble.transform.localScale = Vector3.one * bubbleSize;
        return bubble;
    }

    Dictionary<Vector2Int, BubbleObject> CreateBubbles(List<JsonClass.Layouts> _bubbles)
    {
        if (bubbles.Count > 0)
        {
            foreach (var item in bubbles)
            {
                ResourcesManager.Instance.Push(item.Value.gameObject.name, item.Value.gameObject);
            }
            bubbles.Clear();
        }

        Dictionary<Vector2Int, BubbleObject> bubbleObjects = new Dictionary<Vector2Int, BubbleObject>();
        bubbleSize = (float)horizontal / targetX;

        foreach (JsonClass.Layouts layout in _bubbles)
        {
            JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == layout.bubble);
            BubbleObject bubbleObject = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();

            bubbleObject.Set(bubbleData,new Vector2Int(layout.x,layout.y));
            bubbleObject.transform.parent = bubbleParent;
            bubbleObject.transform.localScale = Vector3.one * bubbleSize;
            bubbleObject.transform.position = BubblePos(layout.x,layout.y);
            bubbleObject.gameObject.SetActive(true);

            bubbleObjects[bubbleObject.Grid] = bubbleObject;
        }

        bubbleParent.transform.position += Vector3.up * targetY * bubbleSize;
        return bubbleObjects;
    }

    Vector3 BubblePos(int _x,int _y)
    {
        Vector3 pos = Vector3.zero;
        float offset = 0;

        offset = _y % 2 == 0 ? bubbleSize : bubbleSize / 2;

        pos.x = -offset + (-targetX * bubbleSize / 2f) + (_x * bubbleSize);
        pos.y = -_y * bubbleSize;

        return pos;
    }

    public void CollisionBubble(BubbleObject bubbleObject,Collider2D collision)
    {
        bubbleObject.transform.parent = bubbleParent;
        Vector2Int grid = Vector2Int.zero;
        float dist = float.MaxValue;

        if (collision.gameObject.layer == LayerMask.NameToLayer("TopWall"))
        {
            for (int i = 0; i < targetX; i++)
            {
                float result = 0;

                if (!bubbles.ContainsKey(new Vector2Int(i + 1,1)))
                {
                    Vector3 pos = BubblePos(i + 1, 1);
                    pos.y += bubbleParent.transform.position.y;
                    result = Vector3.Distance(pos,bubbleObject.transform.position);

                    if (result < dist)
                    {
                        grid = new Vector2Int(i + 1, 1);
                        dist = result;
                    }
                }
            }
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Bubble"))
        {
            BubbleObject collisionBubble = collision.gameObject.GetComponent<BubbleObject>();

            for (int y = -1; y < 2; y++)
            {
                for (int x = -1; x < 2; x++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    Vector2Int current = new Vector2Int(collisionBubble.Grid.x + x, collisionBubble.Grid.y + y);

                    if (current.x < 1 || current.x > targetX)
                    {
                        continue;
                    }

                    if (current.y < 1)
                    {
                        continue;
                    }

                    bool isOdd = collisionBubble.Grid.y % 2 == 1;

                    if (y != 0 && ((isOdd && x == -1) || (!isOdd && x == 1)))
                    {
                        continue;
                    }

                    if (!bubbles.ContainsKey(current))
                    {
                        float result = 0;
                        Vector3 pos = BubblePos(current.x, current.y);
                        pos.y += bubbleParent.transform.position.y;

                        result = Vector3.Distance(pos, bubbleObject.transform.position);

                        if (result < dist)
                        {
                            dist = result;
                            grid = current;
                        }
                    }

                }
            }
        }

        bubbleObject.Grid = grid;
        bubbleObject.transform.localPosition = BubblePos(grid.x, grid.y);
        bubbles[grid] = (bubbleObject);

        CheckBubble(bubbleObject);
    }

    public void CheckBubble(BubbleObject bubbleObject)
    {
        visitBubbles.Add(bubbleObject);

        for (int i = 0; i < visitBubbles.Count; i++)
        {
            VisitBubble(visitBubbles[i]);
            visitBubbles.RemoveAt(i);
            i--;
        }

        if (closeBubbles.Count >= 3)
        {
            ExplosionBubbles(closeBubbles);
        }

        CheckBubbleRoot();
    }

    void ExplosionBubbles(List<BubbleObject> expBubbles)
    {
        for (int i = 0; i < expBubbles.Count; i++)
        {
            bubbles.Remove(expBubbles[i].Grid);
            expBubbles[i].OnExplosion();
            ResourcesManager.Instance.Push(expBubbles[i].name,expBubbles[i].gameObject);
        }
    }

    void VisitBubble(BubbleObject bubbleObject)
    {
        closeBubbles.Add(bubbleObject);

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                Vector2Int current = new Vector2Int(bubbleObject.Grid.x + x, bubbleObject.Grid.y + y);

                bool isOdd = bubbleObject.Grid.y % 2 == 1;

                if (y != 0 && ((isOdd && x == -1) || (!isOdd && x == 1)))
                {
                    continue;
                }

                if (bubbles.ContainsKey(current))
                {
                    BubbleObject near = bubbles[current];
                    near.OnCollision();

                    if (near.ID == bubbleObject.ID && !closeBubbles.Contains(near) && !visitBubbles.Contains(near))
                    {
                        visitBubbles.Add(near);
                    }
                }
            }
        }
    }

    void CheckBubbleRoot()
    {
        visitBubbles.Clear();

        foreach (var item in bubbles.Values)
        {
            visitBubbles.Add(item);
        }

        closeBubbles.Clear();

        for (int i = 0; i < visitBubbles.Count; i++)
        {
            bool result = VisibleRootBubble(visitBubbles[i]);

            if (!result)
            {
                DropBubbles(closeBubbles);
            }

            closeBubbles.Clear();
            visitBubbles.RemoveAt(i);
            i--;
        }

        CheckStage();
    }

    void DropBubbles(List<BubbleObject> bubbleObjects)
    {
        for (int i = 0; i < bubbleObjects.Count; i++)
        {
            bubbles.Remove(bubbleObjects[i].Grid);
            bubbleObjects[i].OnDrop();
        }
    }

    bool VisibleRootBubble(BubbleObject bubbleObject)
    {
        closeBubbles.Add(bubbleObject);

        if (bubbleObject.isRoot)
        {
            return true;
        }

        bool result = false;
        List<BubbleObject> bubbleObjects = new List<BubbleObject>();

        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (x == 0 && y == 0)
                {
                    continue;
                }

                Vector2Int current = new Vector2Int(bubbleObject.Grid.x + x, bubbleObject.Grid.y + y);

                bool isOdd = bubbleObject.Grid.y % 2 == 1;

                if (y != 0 && ((isOdd && x == -1) || (!isOdd && x == 1)))
                {
                    continue;
                }

                if (bubbles.ContainsKey(current))
                {
                    BubbleObject near = bubbles[current];
                    bubbleObjects.Add(near);
                }
            }
        }

        for (int i = 0; i < bubbleObjects.Count; i++)
        {
            if (!closeBubbles.Contains(bubbleObjects[i]))
            {
                result = VisibleRootBubble(bubbleObjects[i]);

                if (result)
                {
                    break;
                }
            }
        }

        return result;
    }

    public void CheckStage()
    {
        switch (mode)
        {
            case GameMode.Normal:
                bubbleParent.transform.position = Vector3.up * GetY() * bubbleSize;
                break;
            case GameMode.Boss:
                bubbleParent.transform.position = Vector3.up * GetY() * bubbleSize;
                break;
            case GameMode.Rescue:
                break;
            default:
                break;
        }

        EndPhase();
    }

    int GetY()
    {
        int y = 0;
        foreach (var item in bubbles)
        {
            if (item.Value.Grid.y > y)
            {
                y = item.Value.Grid.y;
            }
        }

        return y;
    }

    public void EndPhase()
    {
        if (CheckGameMode()|| bubbles.Count == 0)
        {
            DataManager.Instance.CurrentStage = stage;
            status = GameStatus.Clear;
            UIManager.Instance.OpenPanel(UIManager.Instance.GetPanel<ResultPanel>());
        }
        else if (bubbleCount <= 0)
        {
            status = GameStatus.Fail;
            UIManager.Instance.OpenPanel(UIManager.Instance.GetPanel<ResultPanel>());
        }
        else
        {
            CheckShootBubble();
            CreateShootBubble();
            status = GameStatus.Play;
        }
    }

    bool CheckGameMode()
    {
        bool result = false;

        switch (mode)
        {
            case GameMode.Normal:
                result = false;
                break;
            case GameMode.Boss:
                break;
            case GameMode.Rescue:
                break;
            default:
                break;
        }

        return result;
    }

    public void OnClickChangeBubble()
    {
        if (bubbleCount < 2 || status != GameStatus.Play)
        {
            return;
        }
        else if (bubbleCount == 2)
        {
            BubbleObject bubbleObject = shootBubbles[0];
            shootBubbles[0] = shootBubbles[1];
            shootBubbles[0].transform.position = setBubble[0].transform.position;
            shootBubbles[1] = bubbleObject;
            shootBubbles[1].transform.position = setBubble[1].transform.position;
        }
        else
        {   
            BubbleObject bubbleObject = shootBubbles[2];
            shootBubbles[2] = shootBubbles[0];
            shootBubbles[2].transform.position = setBubble[2].transform.position;

            BubbleObject bubbleObject2 = shootBubbles[1];
            shootBubbles[1] = bubbleObject;
            shootBubbles[1].transform.position = setBubble[1].transform.position;

            shootBubbles[0] = bubbleObject2;
            shootBubbles[0].transform.position = setBubble[0].transform.position;
        }

        for (int i = 0; i < shootBubbles.Count; i++)
        {
            if (shootBubbles[i] == null)
            {
                continue;
            }

            if (i == 0)
            {
                shootBubbles[i].transform.localScale = Vector3.one * bubbleSize;
            }
            else
            {
                shootBubbles[i].transform.localScale = Vector3.one * bubbleSize * 0.5f;
            }
        }

    }
}
