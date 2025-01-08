using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerBlock : ActionBlock
{
    [Header("�ε����� �� ����� �����"), Space(10)]
    public AudioClip wiggle;
    public AudioClip dash;

    [Header("�ε����� �� ����� ��ƼŬ"), Space(10)]
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
        // TODO : �̵��� �������� �� �Ҹ��� ���
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
        //TODO : �÷��̾��� ����� �μ����� �� �Ͼ �̺�Ʈ �߰� �ؾ���
        GetComponent<SpriteRenderer>().enabled = false;
        TMP.gameObject.SetActive(false);
    }
}
