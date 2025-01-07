using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBlock : ActionBlock
{
    public AudioClip wiggle;
    public AudioClip dash;

    void Awake()
    {
        if (BlockManager.PlayerBlock == null)
        {
            BlockManager.PlayerBlock = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        if (BlockManager.PlayerBlock == this)
        {
            BlockManager.PlayerBlock = null;
        }
    }

    public override void Init(BlockPosition position, sbyte hp)
    {
        transform.position = position;
        HP = hp;

        GetComponent<SpriteRenderer>().enabled = true;
        TMP.gameObject.SetActive(true);
    }

    public bool IsMoving = false;

    [Header("부딪혔을 때 재생할 파티클"), Space(10)]
    public ParticleSystem bronkenPrtc;
    public ParticleSystem colisionPrtc;


    public override bool TakeDamage()
    {
        if (HP == 1)
        {
            OnFaildGame();
        }

        return --HP <= 0;
    }

    
    void OnFaildGame()
    {
        //현재 게임을 다시 한다.
        // TODO : 파티클을 재생한다.
        GetComponent<SpriteRenderer>().enabled = false;
        TMP.gameObject.SetActive(false);
    }

    public override void OnFailedMove()
    {
        // Wiggle 효과
        IsMoving = true;
        // TODO : 이동에 실패했을 때 소리를 재생
        transform.DOShakePosition(0.3f, .3f, 20).OnComplete(() => IsMoving = false);
    }

    protected override void OnBlockDestroyed()
    {
        bronkenPrtc.Play();
        gameObject.SetActive(false);
    }
}
