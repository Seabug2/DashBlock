using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton
{
    [Header("ÆÄÆ¼Å¬")]
    public ParticleSystem collisionParticle;
    public ParticleSystem breakParticle;
    public ParticleSystem restoreParticle;
    public ParticleSystem deadParticle;

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    void Init()
    {
        ActionBlock.OnCollision += CollisionEffect;
        Block.OnDestroyed += BreakEffect;
    }



    public void CollisionEffect(Block block)
    {
        collisionParticle.transform.position = block.transform.position;
        collisionParticle.Play();
    }

    public void BreakEffect(Block block)
    {
        breakParticle.transform.position = block.transform.position;
        breakParticle.Play();
    }

    public void RestoreEffect(Block block)
    {
        restoreParticle.transform.position = block.transform.position;
        restoreParticle.Play();
    }

    public void DeadEffect(Block block)
    {
        deadParticle.transform.position = block.transform.position;
        deadParticle.Play();
    }

    private void OnDestroy()
    {
        ActionBlock.OnCollision -= CollisionEffect;
        Block.OnDestroyed -= BreakEffect;
    }
}
