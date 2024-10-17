using JsonClass;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBoss : BubbleObject
{
    int curHp;

    public bool isDeath => curHp <= 0;
    public void Hit()
    {
        curHp--;
        if (curHp < 0 )
        {
            curHp = 0;
        }

        UIManager.Instance.SetBossHp(curHp, Hp);
    }
    public override bool isRoot => true;

    public override void Set(BubbleData bubbleData, Vector2Int target, bool isShoot = false)
    {
        base.Set(bubbleData, target, isShoot);
        curHp = Hp;
    }

    public override void OnCreate()
    {
        UIManager.Instance.SetBossHp(curHp, Hp);
        UIManager.Instance.HpOnOff(true);
    }
}
