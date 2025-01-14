using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteBlock : Block
{
    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionDirection, int movementDistance)
    {
        if (movementDistance < 1)
        {
            return false;
        }

        return true;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();
    }
}
