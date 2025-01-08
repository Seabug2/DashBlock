using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    public override bool CanBeDestroyed(sbyte damage = 1)
    {
        BlockManager.Tiles.Remove(Position);
        return true;
    }

    public override void TakeDamage(sbyte damage = 1, Block HitBlock = null)
    {
        Punching();

        if (HitBlock != null && HitBlock is ActionBlock actionblock)
        {
            actionblock.OnStartedMove += BlockOut;
        }
    }

    void BlockOut()
    {
        if (BlockManager.Tiles.TryAdd(Position, this))
        {
            Punching();
        }
        else
        {
            OnBlockDestroyed();
        }
    }
}
