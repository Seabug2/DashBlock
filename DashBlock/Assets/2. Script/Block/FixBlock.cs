using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    public override bool IsClear(Block hitBlock, Vector2Int collisionDirection, int movementDistance)
    {
        if (FixedBlock == null)
            return true;

        return FixedBlock.IsClear(hitBlock, collisionDirection, movementDistance);
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();

        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
            FixedBlock = actionBlock;

            FixedBlock.OnDashedEvent += () =>
            {
                FixedBlock = null;
            };
        }
    }
}
