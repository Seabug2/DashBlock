using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteBlock : Block
{
    public override bool CanMove(ActionBlock hitBlock, ref Vector2Int collisionDirection, int movementDistance)
    {
        return movementDistance > 0;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();
    }
}
