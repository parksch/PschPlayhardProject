using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] List<int> bubbles = new List<int>();
    [SerializeField] BubbleObject prefab;
    [SerializeField] BubbleObject currentBubble;
    [SerializeField] int bubbleCount;

    public GameStatus Status => status;
    public float CurrentBubbleRadius => currentBubble.Radius;

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
        currentBubble.Shoot(normal);
    }

    public void CollisionBubble(BubbleObject bubbleObject)
    {

    }

    public Vector3 RemoveDecimalPoint(Vector3 target)
    {
        Vector3 pos = target;
        pos.x = Mathf.Round(pos.x * 1000f) / 1000f;
        pos.y = Mathf.Round(pos.y * 1000f) / 1000f;
        pos.z = 0;

        return pos;
    }
}
