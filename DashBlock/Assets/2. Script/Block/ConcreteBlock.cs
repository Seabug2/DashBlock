using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConcreteBlock : Block
{
    void Awake()
    {
        //ü�� ǥ���� �ʿ䰡 ����
        TMP.gameObject.SetActive(false);
    }

    public override void TakeDamage(sbyte damage = 1)
    {
        Punching();
    }
}
