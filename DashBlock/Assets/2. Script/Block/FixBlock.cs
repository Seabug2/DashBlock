using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    public static Action<Block> OnHit;
    ActionBlock FixedBlock;

    public override bool TryCollision(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        if (FixedBlock == null)
        {
            collisionPosition = Position;
            return true;
        }
        else
        {
            if (movementDistance < 1) return false;

            bool isCleared = FixedBlock.CheckLine(hitBlock.LastDir, out Vector2Int _, out Block _);
            if (isCleared)
            {
                collisionPosition = Position;
            }

            return true;
        }
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
            if (FixedBlock != null)
            {
                FixedBlock.TakeDamage(actionBlock);
            }
            if (FixedBlock == null)
            {
                FixedBlock = actionBlock;
                FixedBlock.OnMoveBegin += Release;
            }
        }

        OnHit?.Invoke(this);
        Punching();
    }

    void Release(ActionBlock releasedBlock)
    {
        releasedBlock.OnMoveBegin -= Release;
        FixedBlock = null;
    }
}
