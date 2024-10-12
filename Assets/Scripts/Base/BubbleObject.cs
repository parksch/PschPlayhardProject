using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] int id;
    [SerializeField] Vector3 normal;

    bool isShoot = false;
    public float Radius => transform.localScale.x * circleCollider.radius;


    public void Shoot(Vector3 _normal)
    {
        normal = _normal;
        isShoot = true;
    }

    void FixedUpdate()
    {
        if (!isShoot)
        {
            return;
        }

        transform.position += normal * Time.fixedDeltaTime * 3f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            normal.x *= -1;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("TopWall") || collision.gameObject.layer == LayerMask.NameToLayer("Bubble"))
        {
            isShoot = false;

            GameManager.Instance.CollisionBubble(this);
        }
    }
}
