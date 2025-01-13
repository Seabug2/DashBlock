using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteBlock : Block
{
    void Start()
    {
        //체력 표시할 필요가 없음
        TMP.gameObject.SetActive(false);
    }

    public override bool IsClear(Block hitBlock, Vector2Int collisionDirection, int movementDistance)
    {
        return false;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();
    }
}
