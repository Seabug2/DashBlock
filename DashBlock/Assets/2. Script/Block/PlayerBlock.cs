using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBlock : ActionBlock
{
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

    public override void Init(Vector3 position, sbyte hp)
    {
        transform.position = position;
        HP = hp;

        GetComponent<SpriteRenderer>().enabled = true;
        TMP.gameObject.SetActive(true);
    }




    public override void OnFailedMove()
    {
        ActiveMovingBlocks++;
        //Vector3 pos = transform.position;

        // TODO : 이동에 실패했을 때 소리를 재생
        transform
            .DOShakePosition(0.3f, .3f, 20)
            .OnComplete(() =>
            {
                //transform.position = pos;
                ActiveMovingBlocks--;
            });
    }




    protected override void OnBlockDestroyed()
    {
        OnFaildGame();
    }

    void OnFaildGame()
    {
        //TODO : 플레이어의 블록이 부서졌을 때 일어날 이벤트 추가 해야함
        CameraController.BreakEffect();
        GetComponent<SpriteRenderer>().enabled = false;
        TMP.gameObject.SetActive(false);
    }
}
