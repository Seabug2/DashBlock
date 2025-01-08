using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBlock : ActionBlock
{
    [Header("부딪혔을 때 재생할 오디오"), Space(10)]
    public AudioClip wiggle;
    public AudioClip dash;

    [Header("부딪혔을 때 재생할 파티클"), Space(10)]
    public ParticleSystem bronkenPrtc;
    public ParticleSystem colisionPrtc;

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




    public override void OnFailedMove()
    {
        ActiveMovingBlocks++;
        // TODO : 이동에 실패했을 때 소리를 재생
        transform
            .DOShakePosition(0.3f, .3f, 20)
            .OnComplete(() => ActiveMovingBlocks--);
    }

    protected override void OnBlockDestroyed()
    {
        bronkenPrtc.Play();
        OnFaildGame();
    }

    void OnFaildGame()
    {
        //TODO : 플레이어의 블록이 부서졌을 때 일어날 이벤트 추가 해야함
        GetComponent<SpriteRenderer>().enabled = false;
        TMP.gameObject.SetActive(false);
    }
}
