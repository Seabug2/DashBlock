using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBlock : Block
{
    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        if (hitBlock is DashBlock _)
        {
            //부딪힐 블록이 플레이어의 블록이면 거리에 상관없이 
            collisionPosition = Position;
            return true;
        }

        return movementDistance > 0;
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        if (hitBlock is DashBlock player)
        {
            //부딪힐 블록이 플레이어의 블록이면 거리에 상관없이 
            player.HP = 1;
        }

        Punching();
    }
}
