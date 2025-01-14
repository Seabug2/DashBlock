using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBlock : Block
{
    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        if (hitBlock is DashBlock _)
        {
            //�ε��� ����� �÷��̾��� ����̸� �Ÿ��� ������� 
            collisionPosition = Position;
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
        }

        Punching();
    }
}
