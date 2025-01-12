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
            // TODO : �����÷ο� �����ϱ�
            HitBlock.HP = (sbyte)Mathf.Clamp(sum, 1, 99);
            HP = 0;
        }
    }
}
