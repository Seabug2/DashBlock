using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBlock : Block
{
    public override bool TakeDamage()
    {
        int sum = BlockManager.PlayerBlock.HP + HP;

        // TODO : 오버플로우 조심하기
        BlockManager.PlayerBlock.HP = (sbyte)Mathf.Min(sum, 99);
        HP = 0;
        return true;
    }
}
