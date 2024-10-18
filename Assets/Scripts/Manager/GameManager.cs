using ClientEnum;
using JsonClass;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Resources;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameStatus status;
    [SerializeField] GameMode mode;
    [SerializeField] GameWall wall;
    [SerializeField] GameObject bubbleGameObject;
    [SerializeField] Transform gameObjectParent;
    [SerializeField] Transform bubbleParent;
    [SerializeField] List<Transform> setBubble;
    [SerializeField] List<BubbleObject> shootBubbles;
    [SerializeField] Dictionary<Vector2Int, BubbleObject> bubbles = new Dictionary<Vector2Int, BubbleObject>();
    [SerializeField] List<BubbleObject> visitBubbles = new List<BubbleObject>();
    [SerializeField] List<BubbleObject> closeBubbles = new List<BubbleObject>();
    [SerializeField] List<BubbleObject> effectBubbles = new List<BubbleObject>();
    [SerializeField] List<int> shootBubbleID = new List<int>();
    [SerializeField] BubbleObject prefab;
    [SerializeField] Image fillImage;
    [SerializeField] float horizontal = 10;
    [SerializeField] float bubbleSize = 1;
    [SerializeField] int bubbleCount;
    [SerializeField] int targetX, targetY;
    [SerializeField] int stage;
    [SerializeField] int skillGauge;
    [SerializeField] int currentSkillGauge;

    public void AddEffectBubbles(BubbleObject bubble) => effectBubbles.Add(bubble);
    public void RemoveEffectBubbles(BubbleObject bubble)
    {
        Debug.Log("hi");
        effectBubbles.Remove(bubble);

        if (effectBubbles.Count == 0)
        {
            SetBubbleParentY();
        }
    }
    public int TargetX => targetX;
    public int TargetY => targetY;
    public float Horizontal
    {
        get
        {
            return horizontal;
        }
        set
        {
            horizontal = value;
        }
    }
    public void RemoveDropBubble(BubbleObject bubble)
    { 
        effectBubbles.Remove(bubble);
        if (effectBubbles.Count == 0)
        {
            EndPhase();
        }
    }
    public void OnClickSkill()
    {
        if (status != GameStatus.Play)
        {
            return;
        }

        if (currentSkillGauge < skillGauge)
        {
            if (bubbleCount > 1)
            {
                bubbleCount--;
                currentSkillGauge++;
                UIManager.Instance.SetBubbleCount(bubbleCount);
            }
        }
        else
        {
            bubbleCount += 1;
            currentSkillGauge = 0;
            CreateSkillBubble();
        }

        fillImage.fillAmount = (float)currentSkillGauge / skillGauge;
    }
    public void AddSkillGauge(int num = 1)
    {
        currentSkillGauge += num;

        if (currentSkillGauge > skillGauge)
        {
            currentSkillGauge = skillGauge;
        }

        fillImage.fillAmount = (float)currentSkillGauge / skillGauge;
    }
    public Transform BubbleParent => bubbleParent;
    public Transform GameObjectParent => gameObjectParent;
    public GameMode GameMode => mode;
    public GameStatus Status
    {
        get { return status; }
        set { status = value; }
    }
    public float CurrentBubbleRadius => shootBubbles[0].Radius;
    public Dictionary<Vector2Int, BubbleObject> Bubbles => bubbles;
    void CreateSkillBubble()
    {
        JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == 9);
        BubbleObject bubbleObject = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();
        bubbleObject.transform.parent = gameObjectParent;
        bubbleObject.Set(bubbleData, Vector2Int.zero, true);
        bubbleObject.gameObject.SetActive(true);

        if (shootBubbles[1] != null)
        {
            ResourcesManager.Instance.Push(shootBubbles[1].name, shootBubbles[1].gameObject);
        }

        shootBubbles[1] = bubbleObject;
        OnClickChangeBubble();
    }

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
        UIManager.Instance.HpOnOff(false);
        currentSkillGauge = 0;
        fillImage.fillAmount = 0;
        stage = mapData.stage;
        targetX = mapData.x;
        targetY = mapData.y;
        bubbleCount = mapData.bubbleCount;
        mode = mapData.GameMode();
        bubbleSize = (float)Horizontal / targetX;
        wall.Set(horizontal/2, bubbleSize);
        CreateBubbles(mapData.layouts);
        CheckShootBubble();
        CreateShootBubble();
        bubbleGameObject.SetActive(true);
        UIManager.Instance.SetBubbleCount(bubbleCount);
        status = GameStatus.Play;
    }

    public void CheckShootBubble()
    {
        shootBubbleID.Clear();

        if (mode == GameMode.Boss)
        {
            List<JsonClass.BubbleData> bubbles = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.FindAll(x => (ClientEnum.Bubble)x.type == ClientEnum.Bubble.Normal);

            foreach (var item in bubbles)
            {
                shootBubbleID.Add(item.index);
            }
        }
        else
        {
            foreach (var item in bubbles)
            {
                if (!shootBubbleID.Contains(item.Value.ID) && ScriptableManager.Instance.bubbleDataScriptable.GetBubbleType(item.Value.ID) == ClientEnum.Bubble.Normal)
                {
                    shootBubbleID.Add(item.Value.ID);
                }
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
                            int rand = shootBubbleID[UnityEngine.Random.Range(0, shootBubbleID.Count)];
                            JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == rand);
                            shootBubbles[i].Set(bubbleData, Vector2Int.zero, true);
                        }
                    }
                }
            }
            else if (!shootBubbleID.Contains(shootBubbles[i].ID))
            {
                int rand = shootBubbleID[UnityEngine.Random.Range(0, shootBubbleID.Count)];
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
        int rand = shootBubbleID[UnityEngine.Random.Range(0, shootBubbleID.Count)];
        JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == rand);
        BubbleObject bubble = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();
        bubble.Set(bubbleData, Vector2Int.zero,true);
        bubble.transform.localScale = Vector3.one * bubbleSize;
        return bubble;
    }

    public BubbleObject GetBubbleObject(int index,int x,int y)
    {
        JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == index);
        BubbleObject bubbleObject = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();

        bubbleObject.Set(bubbleData, new Vector2Int(x, y));
        bubbleObject.transform.parent = bubbleParent;

        if (bubbleObject.ID == 6)
        {
            bubbleObject.transform.localScale = Vector3.one * bubbleSize * 2.5f;
        }
        else
        {
            bubbleObject.transform.localScale = Vector3.one * bubbleSize;
        }

        bubbleObject.transform.position = BubblePos(x, y);
        bubbleObject.gameObject.SetActive(true);
        bubbleObject.OnCreate();
        return bubbleObject;
    }

    void CreateBubbles(List<JsonClass.Layouts> _bubbles)
    {
        bubbleParent.transform.position = Vector3.zero;

        if (bubbles.Count > 0)
        {
            foreach (var item in bubbles)
            {
                ResourcesManager.Instance.Push(item.Value.gameObject.name, item.Value.gameObject);
            }
            bubbles.Clear();
        }

        foreach (JsonClass.Layouts layout in _bubbles)
        {
            BubbleObject bubbleObject = GetBubbleObject(layout.bubble,layout.x,layout.y);
            bubbles[bubbleObject.Grid] = bubbleObject;
        }

        bubbleParent.transform.position += Vector3.up * targetY * bubbleSize;
    }

    public Vector3 BubblePos(int _x,int _y)
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
        bubbleObject.OnCollision();
        visitBubbles.Add(bubbleObject);

        for (int i = 0; i < visitBubbles.Count; i++)
        {
            VisitBubble(visitBubbles[i]);
            visitBubbles.RemoveAt(i);
            i--;
        }

        List<BubbleObject> collisionList = new List<BubbleObject>();
        for (int i = 0; i < closeBubbles.Count; i++)
        {
            AroundCollisionBubble(closeBubbles[i], collisionList);
        }

        if (closeBubbles.Count >= 3)
        {
            AddSkillGauge(closeBubbles.Count);
            ExplosionBubbles(closeBubbles);
        }

        CheckBubbleRoot();
    }

    void ExplosionBubbles(List<BubbleObject> expBubbles)
    {
        for (int i = 0; i < expBubbles.Count; i++)
        {
            expBubbles[i].OnExplosion();
        }
    }

    void AroundCollisionBubble(BubbleObject bubbleObject,List<BubbleObject> collisionList)
    {
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

                    if (!closeBubbles.Contains(near) && !collisionList.Contains(near))
                    {
                        near.OnCollision();
                        collisionList.Add(near);
                    }
                }
            }
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
        effectBubbles.Clear();

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

            visitBubbles.RemoveAt(i);
            i--;

            for (int j = 0; j < closeBubbles.Count; j++)
            {
                if (visitBubbles.Contains(closeBubbles[j]))
                {
                    visitBubbles.Remove(closeBubbles[j]);
                }
            }

            closeBubbles.Clear();
        }

        if (effectBubbles.Count == 0)
        {
            EndPhase();
        }
    }

    void DropBubbles(List<BubbleObject> bubbleObjects)
    {
        for (int i = 0; i < bubbleObjects.Count; i++)
        {
            if (bubbles.ContainsKey(bubbleObjects[i].Grid))
            {
                effectBubbles.Add(bubbleObjects[i]);
                bubbleObjects[i].OnDrop();
            }
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
                SetBubbleParentY();
                break;
            case GameMode.Boss:
                BubbleObject bubbleObject = bubbles[new Vector2Int((TargetX / 2 + 1) - 2, 1)];
                AddEffectBubbles(bubbleObject);
                bubbleObject.OnEnd();
                bubbleObject = bubbles[new Vector2Int((TargetX / 2 + 1) + 2, 1)];
                AddEffectBubbles(bubbleObject);
                bubbleObject.OnEnd();
                break;
            default:
                break;
        }
    }

    void SetBubbleParentY()
    {
        switch (mode)
        {
            case GameMode.Normal:
                bubbleParent.transform.DOLocalMoveY(GetY() * bubbleSize, .3f).SetEase(Ease.OutBack).OnComplete(() => { status = GameStatus.Play; });
                break;
            case GameMode.Boss:
                int y = GetY();

                if (y < targetY)
                {
                    bubbleParent.transform.DOLocalMoveY(TargetY * bubbleSize, .3f).SetEase(Ease.OutBack).OnComplete(() => { status = GameStatus.Play; });
                }
                else
                {
                    bubbleParent.transform.DOLocalMoveY(GetY() * bubbleSize, .3f).SetEase(Ease.OutBack).OnComplete(() => { status = GameStatus.Play; });
                }
                break;
            default:
                break;
        }
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
        if (CheckGameMode())
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
            CheckStage();

            if (effectBubbles.Count == 0)
            {
                status = GameStatus.Play;
            }
        }
    }

    bool CheckGameMode()
    {
        bool result = false;

        switch (mode)
        {
            case GameMode.Normal:
                result = bubbles.Count == 0;
                break;
            case GameMode.Boss:
                BubbleBoss bubbleBoss = bubbles[new Vector2Int(targetX / 2 + 1, 1)] as BubbleBoss;
                result = bubbleBoss.isDeath; 
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

    public void ActiveProperty(ClientEnum.BubbleProperty property)
    {
        switch (property)
        {
            case BubbleProperty.HitBoss:
                BubbleBoss bubbleBoss = bubbles[new Vector2Int(targetX / 2 + 1, 1)] as BubbleBoss;
                bubbleBoss.Hit();
                break;
            case BubbleProperty.CollisionBubble:
                break;
            default:
                break;
        }
    }
}
