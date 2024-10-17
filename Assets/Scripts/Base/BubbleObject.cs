using ClientEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BubbleObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] SpriteRenderer front;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] int id;
    [SerializeField] int hp;
    [SerializeField] ClientEnum.Bubble type;
    [SerializeField] Vector3 normal;
    [SerializeField] Vector2Int grid;
    [SerializeField] List<ClientEnum.BubbleProperty> properties;

    bool isShoot = false;

    public void Properties(ClientEnum.BubbleProperty bubbleProperty)
    {
        front.gameObject.SetActive(true);
        properties.Add(bubbleProperty);
    }
    public int Hp => hp;
    public virtual bool isRoot => grid.y == 1;
    public float Radius => transform.localScale.x * circleCollider.radius;
    public int ID => id;
    public Vector2Int Grid 
    { 
        set
        {
            grid = value;
        }
        get
        {
            return grid;
        }
    }
    public ClientEnum.Bubble Type => type;

    public virtual void Set(JsonClass.BubbleData bubbleData, Vector2Int target, bool isShoot = false)
    {
        front.gameObject.SetActive(false);
        properties.Clear();
        hp = bubbleData.hp;
        sprite.sprite = bubbleData.Sprite();
        id = bubbleData.index;
        type = bubbleData.Type();
        grid = target;
        rigid.isKinematic = true;
        circleCollider.isTrigger = true;

        if (isShoot)
        {
            circleCollider.enabled = false;
        }
    }

    public void Shoot(Vector3 _normal)
    {
        OnShoot();
        normal = _normal;
        isShoot = true;
        circleCollider.enabled = true;
    }

    void FixedUpdate()
    {
        if (!isShoot)
        {
            return;
        }

        transform.position += normal * Time.fixedDeltaTime * 8f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isShoot)
        {
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            normal.x *= -1;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("TopWall") || collision.gameObject.layer == LayerMask.NameToLayer("Bubble"))
        {
            isShoot = false;

            GameManager.Instance.CollisionBubble(this, collision);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("BubbleDeath"))
        {
            GameManager.Instance.AddSkillGauge();
            GameManager.Instance.RemoveDropBubble(this);
            ResourcesManager.Instance.Push(name, gameObject);
        }
    }

    public virtual void OnCreate()
    {

    }

    public virtual void OnCollision()
    {

    }

    public virtual void OnShoot()
    {

    }

    public virtual void OnExplosion()
    {
        GameManager.Instance.Bubbles.Remove(Grid);

        if (GameManager.Instance.GameMode == GameMode.Boss)
        {
            BubblePath bubbleObject = GameManager.Instance.Bubbles[new Vector2Int((GameManager.Instance.TargetX / 2 + 1) - 2, 1)] as BubblePath; 
            bubbleObject.CheckBubble(this);
            bubbleObject = GameManager.Instance.Bubbles[new Vector2Int((GameManager.Instance.TargetX / 2 + 1) + 2, 1)] as BubblePath ;
            bubbleObject.CheckBubble(this);
        }

        for (int i = 0; i < properties.Count; i++)
        {
            GameManager.Instance.ActiveProperty(properties[i]);
        }

        ResourcesManager.Instance.Push(name,gameObject);
    }

    public virtual void OnDrop()
    {
        GameManager.Instance.Bubbles.Remove(Grid);

        if (GameManager.Instance.GameMode == GameMode.Boss)
        {
            BubblePath bubbleObject = GameManager.Instance.Bubbles[new Vector2Int((GameManager.Instance.TargetX / 2 + 1) - 2, 1)] as BubblePath;
            bubbleObject.CheckBubble(this);
            bubbleObject = GameManager.Instance.Bubbles[new Vector2Int((GameManager.Instance.TargetX / 2 + 1) + 2, 1)] as BubblePath;
            bubbleObject.CheckBubble(this);
        }

        for (int i = 0; i < properties.Count; i++)
        {
            GameManager.Instance.ActiveProperty(properties[i]);
        }

        transform.parent = GameManager.Instance.GameObjectParent;
        rigid.isKinematic = false;
        circleCollider.isTrigger = false;
    }

    public virtual void OnEnd()
    {

    }
}
