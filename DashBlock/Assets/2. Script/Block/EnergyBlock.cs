using UnityEngine;

public class EnergyBlock : Block
{
    public override bool CanMove(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //�ε��� Block�� DashBlock�̶�� �Ÿ��� ������� ������ �������.
        if (hitBlock is DashBlock _)
        {
            collisionPosition = Position;
            return true;
        }

        return movementDistance > 0;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        if (HitBlock is DashBlock dashBlock)
        {
            dashBlock.HP = 99;
            HP = 0;
        }
    }
}