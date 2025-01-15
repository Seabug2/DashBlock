using UnityEngine;

public class EnergyBlock : Block
{
    public override bool IsCleared(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //부딪힌 Block이 DashBlock이라면 거리에 상관없이 무조건 사라진다.
        if (hitBlock is DashBlock _)
        {
            collisionPosition = Position;
            return true;
        }

        return movementDistance > 0;
    }

    /// <summary>
    /// 충격을 받았을 때, DashBlock과 부딪힌 거라면, 무조건 사라지고 부딪힌 DashBlock의 체력을 99로 회복시켜준다.
    /// </summary>
    /// <param name="HitBlock"></param>
    public override void TakeDamage(Block HitBlock = null)
    {
        if (HitBlock is DashBlock dashBlock)
        {
            dashBlock.HP = 99;
            HP = 0;
        }
    }
}