using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    public override int MinimunRange()
    {
        return 0;
    }

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
            actionblock.OnStartedMove += () =>
            {
                ReleaseBlock();
                actionblock.OnStartedMove -= ReleaseBlock;
            };
        }
    }

    void ReleaseBlock()
    {
        if (BlockManager.Tiles.ContainsValue(this))
        {
            return;
        }

        if (BlockManager.Tiles.TryAdd(Position, this))
        {
            Punching();
        }
        else
        {
            Debug.Log("FIx ºí·Ï ¹º°¡ Àß¸øµÊ");
            OnBlockDestroyed();
        }
    }
}
