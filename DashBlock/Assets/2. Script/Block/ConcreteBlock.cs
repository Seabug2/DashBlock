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

    public override void TakeDamage(sbyte damage = 1, Block HitBlock = null)
    {
        Punching();
    }
}
