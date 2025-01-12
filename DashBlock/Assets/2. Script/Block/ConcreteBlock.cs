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

    public override bool CanBeDestroyed(int damage = 1)
    {
        return false;
    }

    public override void TakeDamage(int damage = 1, Block HitBlock = null)
    {
        Punching();
    }
}
