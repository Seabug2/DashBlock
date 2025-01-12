using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlock : Block
{
    public override void TakeDamage(int damage = 1, Block HitBlock = null)
    {
        if (HitBlock != null)
        {
            int sum = HitBlock.HP + HP;
            // TODO : 오버플로우 조심하기
            HitBlock.HP = (sbyte)Mathf.Clamp(sum, 1, 99);
            HP = 0;
        }
    }
}
