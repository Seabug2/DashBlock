using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    public override bool IsCleared(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //���� ����ٸ� �ε��� �����
        //�Ÿ��� ������� ����� ��ġ���� �̵��Ѵ�.
        if (FixedBlock == null)
        {
            collisionPosition = Position;
            return true;
        }
        else
        {
            //�̵� �������� �ʿ����, �ε��� ��ϵ� �������
            bool isCleared = FixedBlock.CheckLine(hitBlock.LastDir, out Vector2Int _, out Block _);
            if (isCleared)
            {
                //�̵��� �����ϴٸ� �����̷��� ����� �������� �ڽ��� ��ġ�� ���Ѵ�.
                collisionPosition = Position;
            }
            return isCleared;
        }
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        //�ε��� ����� �����ϰ�,
        //�ε��� ����� ActionBlock�̸�,
        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
            if (FixedBlock != null)
            {
                FixedBlock.TakeDamage(HitBlock);
            }

            //���� ���� ����� FixedBlock�� ����
            FixedBlock = actionBlock;
            FixedBlock.OnMoveBegin += Release;
        }

        Punching();
    }

    void Release()
    {
        FixedBlock = null;
    }
}
