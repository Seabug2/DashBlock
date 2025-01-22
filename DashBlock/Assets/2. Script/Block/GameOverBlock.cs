using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBlock : Block
{
    public static Action<Block> OnHit;
    public override bool TryCollision(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        if (hitBlock is DashBlock _)
        {
            return true;
        }

        return movementDistance > 0;
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        if (hitBlock is DashBlock player)
        {
            //�ε��� ����� �÷��̾��� ����̸� �Ÿ��� ������� 
            player.HP = 1;
            OnHit?.Invoke(this);
            return;
        }

        Punching();
    }
}
