using UnityEngine;

public class GameWall : MonoBehaviour
{
    public BoxCollider2D top;
    public BoxCollider2D left;
    public BoxCollider2D right;

    public void Set(float radius,float bubbleSize)
    {
        top.size = new Vector2((GameManager.Instance.TargetX + 2) * bubbleSize, bubbleSize);
        left.size = new Vector2(bubbleSize,50);
        right.size = new Vector2(bubbleSize, 50);
        left.transform.localPosition = new Vector3(-radius - bubbleSize, 0, 0);
        right.transform.localPosition = new Vector3(radius + bubbleSize, 0, 0);
    }
}
