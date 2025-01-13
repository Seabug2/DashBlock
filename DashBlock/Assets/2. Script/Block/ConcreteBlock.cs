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

    public override Vector2Int CollisionPosition(Block hitBlock ,Vector2Int collisionDir, int hitDistance)
    {
        return Position - collisionDir;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();
    }
}
