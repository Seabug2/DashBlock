using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBlock : Block
{
    /// <summary>
    /// 부딪힌 DashBlcok의 HP를 0으로 만든다.
    /// </summary>
    /// <param name="HitBlock"></param>
    public override void TakeDamage(Block HitBlock = null)
    {
        if (HitBlock != null && HitBlock is DashBlock dashBlock)
        {
            dashBlock.HP = 0;
        }

        Punching();
    }
}
