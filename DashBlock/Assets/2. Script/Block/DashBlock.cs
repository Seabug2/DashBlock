using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DashBlock : ActionBlock
{
    #region �̱���
    static DashBlock player;
    public static DashBlock Player => player;

    void Awake()
    {
        if (player == null)
        {
            player = this;
            Pools[GetType().Name[0] - 'A'].Enqueue(this);

            MapLoader.Initialize().Forget();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    private void OnDestroy()
    {
        if (player == this)
        {
            player = null;
        }
    }
    #endregion




    bool isGameOVer;
    public bool IsGameOVer
    {
        get
        {
            return isGameOVer;
        }
        private set
        {
            isGameOVer = value;

            GetComponent<SpriteRenderer>().enabled = !isGameOVer;
            TMP.gameObject.SetActive(!isGameOVer);
        }
    }

    public override void Init(Vector3 position, int hp)
    {
        Pools[GetType().Name[0] - 'A'].Enqueue(this);
        transform.position = position;
        HP = 99;
        IsGameOVer = false;
        gameObject.SetActive(true);
    }

    public UnityEvent OnStartedGame;
    public UnityEvent OnFailedGame;

    /// <summary>
    /// ������ �ε����� ���
    /// </summary>
    public override void Dash(Vector2Int Dir)
    {
        //Dir �������� �̵��Ͽ��� �� �̵��� �����Ѱ�?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            IsMoving = true;

            OnMoveBegin?.Invoke();
            OnMoveBegin = null;

            transform
                .DOMove(new Vector3(targetPosition.x, targetPosition.y, 0), .1f)
                //.SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    if (hitBlock != null)
                    {
                        hitBlock.TakeDamage(this);
                    }

                    TakeDamage(hitBlock);
                    IsMoving = false;
                });
        }
        else
        {
            OnFailedMove();
        }
    }

    public override void OnFailedMove()
    {
        IsMoving = true;

        // TODO : �̵��� �������� �� �Ҹ��� ���
        transform
            .DOShakePosition(0.3f, .3f, 20)
            .OnComplete(() =>
            {
                IsMoving = false;
            });
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        int damage = (hitBlock == null) ? 1 : hitBlock.CollisionDamage;

        if (damage > 0)
        {
            CameraController.Shake(0.34f, 0.56f);
        }

        HP -= damage;
    }

    protected override void OnBlockDestroyed()
    {
        CameraController.BreakEffect();
        IsGameOVer = true;
        OnFailedGame.Invoke();
    }

    public override bool IsCleared(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //�浹�Ÿ��� 1���� ������ �̵� ����
        if (movementDistance < 1)
            return false;

        collisionPosition = (HP == 1) ? Position : collisionPosition;
        return true;
    }
}