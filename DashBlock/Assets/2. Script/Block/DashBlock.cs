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




    bool isGameover;
    public bool IsGameover
    {
        get
        {
            return isGameover;
        }
        private set
        {
            isGameover = value;

            GetComponent<SpriteRenderer>().enabled = !isGameover;
            TMP.gameObject.SetActive(!isGameover);

            if (!value)
            {
                OnFailedGame.Invoke();
            }
        }
    }

    public UnityEvent OnStartedGame;
    public UnityEvent OnFailedGame;

    public override void Init(Vector3 position, int hp)
    {
        Pools[GetType().Name[0] - 'A'].Enqueue(this);
        transform.position = position;
        HP = 99;
        IsGameover = false;
        gameObject.SetActive(true);
    }

    public override bool TryCollision(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //�浹�Ÿ��� 1���� ������ �̵� ����
        if (movementDistance < 1)
            return false;

        collisionPosition = (HP == hitBlock.Damage) ? Position : collisionPosition;
        return true;
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        int damage = (hitBlock == null) ? 1 : hitBlock.Damage;

        if (damage > 0 || HP > damage)
        {
            OnCollision?.Invoke(this);
        }

        HP -= damage;
    }

    protected override void OnBlockDestroyed()
    {
        OnDestroyed?.Invoke(this);
        IsGameover = true;
    }

    #region ����� �̵�
    public override void Dash(Vector2Int Dir)
    {
        //Dir �������� �̵��Ͽ��� �� �̵��� �����Ѱ�?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            IsMoving = true;

            OnMoveBegin?.Invoke(this);
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
        OnFailedMoveAction?.Invoke(this);
        IsMoving = true;

        transform
            .DOShakePosition(0.3f, .3f, 20)
            .OnComplete(() =>
            {
                IsMoving = false;
            });
    }
    #endregion
}