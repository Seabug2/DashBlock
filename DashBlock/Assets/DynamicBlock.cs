using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DynamicBlock : ActionBlock
{
    public override bool TakeDamage()
    {
        if (HP > 1)
        {
            Vector2 dir = transform.position - BlockManager.PlayerBlock.transform.position;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                sbyte x = dir.x > 0 ? (sbyte)1 : (sbyte)-1;
                BlockManager.CheckLine(this, new BlockPosition(x, 0));
            }
            else
            {
                sbyte y = dir.y > 0 ? (sbyte)1 : (sbyte)-1;
                BlockManager.CheckLine(this, new BlockPosition(0, y));
            }
        }

        return --HP <= 0;
    }
}
