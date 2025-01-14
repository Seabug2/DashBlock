using UnityEngine;

public class EnergyBlock : Block
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hitBlock">부딪힌 Block의 </param>
    /// <param name="collisionDirection">충돌 방향과</param>
    /// <param name="movementDistance">충돌 거리</param>
    /// <returns></returns>
    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionPosition, int movementDistance)
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
        if (HitBlock != null && HitBlock is DashBlock dashBlock)
        {
            dashBlock.HP = 100;
            HP = 0;
        }
    }
}
