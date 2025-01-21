using UnityEngine;

public class EnergyBlock : Block
{
    public override bool CanMove(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //부딪힌 Block이 DashBlock이라면 거리에 상관없이 무조건 사라진다.
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