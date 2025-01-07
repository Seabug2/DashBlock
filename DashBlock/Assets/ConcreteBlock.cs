using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidBlock : Block
{
    public override bool TakeDamage()
    {
        Punching();
        return false;
    }
}
