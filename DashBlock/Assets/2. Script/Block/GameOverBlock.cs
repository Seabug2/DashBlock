using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBlock : Block
{
    public override bool CanMove(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
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
            return;
        }

        Punching();
    }
}
