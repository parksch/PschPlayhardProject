using ClientEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSkill : BubbleObject
{
    public override void OnCollision()
    {
        for (int y = -2; y < 3; y++)
        {
            for(int x = -2 ; x < 3; x++)
            {
                if (y == 0 && x == 0)
                {
                    continue;
                }

                if ((y == -2 || y == 2) && x <= -2 || x >= 2)
                {
                    continue;
                }

                bool isOdd = Grid.y % 2 == 1;
                int xOffset = isOdd && (y == 1 || y == -1) ? 1 : 0 ;

                if (GameManager.Instance.Bubbles.ContainsKey(new Vector2Int(Grid.x + x + xOffset, Grid.y +y)))
                {
                    BubbleObject near = GameManager.Instance.Bubbles[new Vector2Int(Grid.x + x + xOffset, Grid.y + y)];
                    near.OnExplosion();
                }
            }
        }

        OnExplosion();
    }
}
