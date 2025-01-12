using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DynamicBlock : ActionBlock
{
    public override void TakeDamage(int damage = 1, Block HitBlock = null)
    {
        if (HP > damage)
        {
            Vector2 dir = transform.position - BlockManager.PlayerBlock.transform.position;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                sbyte x = dir.x > 0 ? (sbyte)1 : (sbyte)-1;
                CheckLine(new Vector2Int(x, 0));
            }
            else
            {
                sbyte y = dir.y > 0 ? (sbyte)1 : (sbyte)-1;
                CheckLine(new Vector2Int(0, y));
            }

            return;
        }

        base.TakeDamage(damage);
        BlockManager.Tiles.TryAdd(Position, this);
    }

    public override void Dash(Vector2 targetPosition, Block target)
    {
        BlockManager.Tiles.Remove(Position);

        base.Dash(targetPosition, target);
    }
}
