using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteBlock : Block
{
    void Start()
    {
        //ü�� ǥ���� �ʿ䰡 ����
        TMP.gameObject.SetActive(false);
    }

    public override bool IsClear(Block hitBlock, ref Vector2Int collisionDirection, int movementDistance)
    {
        if (movementDistance < 1)
        {
            return false;
        }

        return true;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();
    }
}
