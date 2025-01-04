using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : Singleton
{
    public readonly Queue<ParticleSystem> pools = new();

}
