using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    public override bool IsClear(Block hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //���� ����ٸ� �ε��� ����� �� ����� ��ġ���� �̵��Ѵ�.
        if (FixedBlock == null)
        {
            collisionPosition = Position;
            return true;
        }

        //������� ���� ���
        //���� ����� �ִ� ����� �ڸ� �̵��� �������� Ȯ��
        Vector2Int dir = Position - collisionPosition;

        //�̵� �������� �ʿ����, �ε��� ��ϵ� �������
        bool isCleared = FixedBlock.CheckLine(dir, out Vector2Int _, out Block _);
        if (isCleared)
        {
            //�̵��� �����ϴٸ� �����̷��� ����� �������� �ڽ��� ��ġ�� ���Ѵ�.
            collisionPosition = Position;
        }
        return isCleared;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        //�ε��� ����� �����ϰ�,
        //�ε��� ����� ActionBlock�̸�,
        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
            if (FixedBlock != null)
            {
                //����� �ִ� ����� �����ϴµ� �׼Ǻ���� �ε����ٴ� �ǹ̴�,
                //�̹� ������ ��� �ִ� ����� �з��� �� �ִٰ� ����� ���� ��
                //��ϰ� �ε����� ����� �ִ� ����� �����̸� �ȴ�.
                FixedBlock.TakeDamage(HitBlock);
            }

            FixedBlock = actionBlock;
        }

        Punching();
    }
}
