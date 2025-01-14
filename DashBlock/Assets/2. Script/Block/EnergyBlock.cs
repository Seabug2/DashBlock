using UnityEngine;

public class EnergyBlock : Block
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitBlock">�ε��� Block�� </param>
    /// <param name="collisionDirection">�浹 �����</param>
    /// <param name="movementDistance">�浹 �Ÿ�</param>
    /// <returns></returns>
    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //�ε��� Block�� DashBlock�̶�� �Ÿ��� ������� ������ �������.
        if (hitBlock is DashBlock _)
        {
            collisionPosition = Position;
            return true;
        }

        return movementDistance > 0;
    }

    /// <summary>
    /// ����� �޾��� ��, DashBlock�� �ε��� �Ŷ��, ������ ������� �ε��� DashBlock�� ü���� 99�� ȸ�������ش�.
    /// </summary>
    /// <param name="HitBlock"></param>
    public override void TakeDamage(Block HitBlock = null)
    {
        if (HitBlock != null && HitBlock is DashBlock dashBlock)
        {
            dashBlock.HP = 100;
            HP = 0;
        }
    }
}
