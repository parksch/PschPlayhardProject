using JsonClass;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using TMPro.EditorUtilities;
using UnityEngine;

public enum GameStatus
{
    Selection,
    Play,
    Running
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] GameStatus status;
    [SerializeField] GameObject bubbleGameObject;
    [SerializeField] Transform bubbleParent;
    [SerializeField] List<Transform> setBubble;
    [SerializeField] List<BubbleObject> shootBubble;
    [SerializeField] List<BubbleObject> bubbles = new List<BubbleObject>();
    [SerializeField] List<int> shootBubbles = new List<int>();
    [SerializeField] BubbleObject prefab;
    [SerializeField] float bubbleSize = 1;
    [SerializeField] int bubbleCount;
    [SerializeField] int horizontal = 10;
    [SerializeField] int x, y;
    public GameStatus Status => status;
    public float CurrentBubbleRadius => shootBubble[0].Radius;

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

    public void GameReset()
    {
        bubbles.Clear();
    }

    public void BubbleShoot(Vector3 normal)
    {
        shootBubble[0].Shoot(normal);
    }

    public Vector3 RemoveDecimalPoint(Vector3 target)
    {
        Vector3 pos = target;
        pos.x = Mathf.Round(pos.x * 1000f) / 1000f;
        pos.y = Mathf.Round(pos.y * 1000f) / 1000f;
        pos.z = 0;

        return pos;
    }

    public void SetStage(MapData mapData)
    {
        x = mapData.x;
        y = mapData.y;
        bubbleCount = mapData.bubbleCount;
        bubbles = CreateBubbles(mapData.layouts);
        CheckShootBubble();
        CreateShootBubble();
        bubbleGameObject.SetActive(true);
        UIManager.Instance.SetBubbleCount(bubbleCount);
        UIManager.Instance.SetStage();
        status = GameStatus.Play;
    }

    public void CheckShootBubble()
    {
        shootBubbles.Clear();

        foreach (var item in bubbles)
        {
            if (!shootBubbles.Contains(item.ID) && ScriptableManager.Instance.bubbleDataScriptable.GetBubbleType(item.ID) == ClientEnum.Bubble.Normal)
            {
                shootBubbles.Add(item.ID);
            }
        }   
    }

    public void CreateShootBubble()
    {
        for (int i = 0; i < shootBubble.Count; i++)
        {
            if (shootBubble[i] == null)
            {
                for(int j = i + 1; j < shootBubble.Count; j++)
                {
                    if (shootBubble[j] != null && shootBubble[i] == null)
                    {
                        shootBubble[i] = shootBubble[j];
                        shootBubble[i].transform.position = setBubble[i].transform.position;
                        shootBubble[j] = null;
                    }
                }
            }
        }

        for (int i = 0; i < shootBubble.Count; i++)
        {
            if (i < bubbleCount && shootBubble[i] == null)
            {
                shootBubble[i] = GetShootBubble();
                shootBubble[i].transform.position = setBubble[i].transform.position;
            }
        }
    }

    BubbleObject GetShootBubble()
    {
        JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == shootBubbles[Random.Range(0, shootBubbles.Count)]);
        BubbleObject bubble = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();
        bubble.transform.localScale = Vector3.one * bubbleSize;
        return bubble;
    }

    List<BubbleObject> CreateBubbles(List<JsonClass.Layout> bubbles)
    {
        List<BubbleObject> bubbleObjects = new List<BubbleObject>();
        bubbleSize = (float)horizontal / x;
        foreach (JsonClass.Layout layout in bubbles)
        {
            JsonClass.BubbleData bubbleData = ScriptableManager.Instance.bubbleDataScriptable.bubbleData.Find(x => x.index == layout.bubble);
            BubbleObject bubbleObject = ResourcesManager.Instance.Get(bubbleData.prefab).GetComponent<BubbleObject>();
            Vector3 pos = Vector3.zero;
            float offset = 0;
            
            offset = layout.y % 2 == 0 ? bubbleSize : bubbleSize/2;

            pos.x = -offset + (-x * bubbleSize / 2f) + (layout.x * bubbleSize);
            pos.y = (-layout.y * bubbleSize);

            bubbleObject.Set(bubbleData);
            bubbleObject.transform.parent = bubbleParent;
            bubbleObject.transform.localScale *= bubbleSize;
            bubbleObject.transform.localPosition = pos;
            bubbleObject.gameObject.SetActive(true);

            bubbleObjects.Add(bubbleObject);
        }
        bubbleParent.transform.position += Vector3.up * y * bubbleSize;
        return bubbleObjects;
    }

    public void CollisionBubble(BubbleObject bubbleObject)
    {

    }

    public void CheckBubble()
    {

    }

    public void CheckStage()
    {

    }

    public void EndPhase()
    {

    }
}
