using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    public static bool IsAnyActionBlockMoving => movingBlockCount > 0;

    static int movingBlockCount = 0;

    bool isMoving;
    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
        set
        {
            isMoving = value;
            movingBlockCount = Mathf.Max(0, movingBlockCount + (value ? 1 : -1));
        }
    }

    public Action OnMoveBegin;

    /// <summary>
    /// ������ �ε����� ���
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            if (TileMap.TryGetValue(Position, out Block block)
                && block == this)
            {
                TileMap.Remove(Position);
            }

            IsMoving=true;

            OnMoveBegin?.Invoke();
            OnMoveBegin = null;

            transform
                .DOMove(new Vector3(targetPosition.x, targetPosition.y, 0), .2f)
                //.SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    if (hitBlock != null)
                    {
                        hitBlock.TakeDamage(this);
                    }

                    TakeDamage(hitBlock);

                    TileMap.TryAdd(Position, this);
                    IsMoving = false;
                    CameraController.Shake(0.3f, 0.4f);
                });
        }
        else
        {
            OnFailedMove();
        }
    }

    public Vector2Int LastDir { get; private set; }

    public bool CheckLine(Vector2Int Dir, out Vector2Int targetPosition, out Block hitBlock)
    {
        LastDir = Dir;

        targetPosition = Position;
        hitBlock = null;

        Vector2Int nextPosition;
        int movementDistance = 0;
        while (true)
        {
            nextPosition = targetPosition + Dir;

            //���� �ε����ٸ� �˻縦 ��ģ��.
            if (nextPosition.x < 0 || nextPosition.x > limit_x
                || nextPosition.y < 0 || nextPosition.y > limit_y)
            {
                break;
            }

            //��Ͽ� �ε����ٸ� �˻縦 ��ģ��.
            if (TileMap.TryGetValue(nextPosition, out hitBlock))
            {
                break;
            }

            movementDistance++;
            targetPosition = nextPosition;
        }

        //�ε��� ����� ������(���� �ε��� ��)
        if (hitBlock == null)
        {
            //�̵��� �Ÿ��� ���� �̵� ���� ���� ��ȯ
            //���� �ε����� �� �̵��� �Ÿ��� 1���� ������ �̵� ������ �� = false
            //���� ��ġ������ ������... ���� targetPosition������ �̵�
            return movementDistance > 0;
        }
        else
        {
            //�̵��� ������ ���

            //�̵��� ������ ���
            ////////��� �ձ��� �̵�
            ////////��� ��ġ���� �̵�

            return hitBlock.IsCleared(this, ref targetPosition, movementDistance);
        }
    }
    public override bool IsCleared(ActionBlock hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
    {
        return movementDistance > 0;
    }

    public virtual void OnFailedMove()
    {
        Punching();
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        //�����ִ� �߿� �������� �޾Ҵٴ� ���� �ٸ� �����̴� ��ü�� �ε����� �з��� �Ѵٴ� �ǹ�
        if (IsMoving)
        {
            int damage = (hitBlock == null) ? 1 : hitBlock.CollisionDamage;

            if (damage > 0)
            {
                CameraController.Shake(0.34f, 0.56f);
            }
        }
        else
        {
            if (hitBlock is ActionBlock actionBlock)
            {
                Dash(actionBlock.LastDir);
            }
        }
    }
}
