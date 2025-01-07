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

    [Header("�ε����� �� ����� ��ƼŬ"), Space(10)]
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
        //���� ������ �ٽ� �Ѵ�.
        // TODO : ��ƼŬ�� ����Ѵ�.
        GetComponent<SpriteRenderer>().enabled = false;
        TMP.gameObject.SetActive(false);
    }

    public override void OnFailedMove()
    {
        // Wiggle ȿ��
        IsMoving = true;
        // TODO : �̵��� �������� �� �Ҹ��� ���
        transform.DOShakePosition(0.3f, .3f, 20).OnComplete(() => IsMoving = false);
    }

    protected override void OnBlockDestroyed()
    {
        bronkenPrtc.Play();
        gameObject.SetActive(false);
    }
}
