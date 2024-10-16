using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] int id;
    [SerializeField] ClientEnum.Bubble type;
    [SerializeField] Vector3 normal;
    [SerializeField] Vector2Int grid;

    bool isShoot = false;

    public bool isRoot => grid.y == 1;
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

    public void Set(JsonClass.BubbleData bubbleData, Vector2Int target, bool isShoot = false)
    {
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
            ResourcesManager.Instance.Push(name, gameObject);
        }
    }

    public virtual void OnCollision()
    {

    }

    public virtual void OnShoot()
    {

    }

    public virtual void OnExplosion()
    {

    }

    public virtual void OnDrop()
    {
        transform.parent = GameManager.Instance.GameObjectParent;
        rigid.isKinematic = false;
        circleCollider.isTrigger = false;
    }

    public virtual void OnEnd()
    {

    }
}
