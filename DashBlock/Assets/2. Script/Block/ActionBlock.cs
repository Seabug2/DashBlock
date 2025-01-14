using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    public static int MovingBlockCount = 0;

    /// <summary>
    /// �����̰� �ִ� Action Block�� �ϳ��� �ִٸ� true�� ��ȯ
    /// </summary>
    private bool isMoving;
    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
        set
        {
            isMoving = value;
            MovingBlockCount += value ? 1 : -1;

            if (MovingBlockCount < 0)
            {
                MovingBlockCount = 0;
            }
        }
    }

    /// <summary>
    /// ������ �ε����� ���
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        //Dir �������� �̵��Ͽ��� �� �̵��� �����Ѱ�?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            // Dir �������� �̵��ϸ� targetPosition ��ġ���� �̵��� ���̰�
            // hitBlock�� �ε��� ��

            //�ڽ��� ���� Ű���� Ÿ�ϸʿ� �����ϸ�
            //�� ����� ���� �ڽ��̶�� Ÿ�ϸʿ��� ����
            if (TileMap.TryGetValue(Position, out Block block)
                && block == this)
            {
                TileMap.Remove(Position);
            }

            IsMoving = true;

            transform
                .DOMove(new Vector3(targetPosition.x, targetPosition.y, 0), .1f)
                .SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    if (hitBlock != null)
                    {
                        hitBlock.TakeDamage(this);
                    }

                    TakeDamage(hitBlock);
                    IsMoving = false;

                    //�̵��� ��ġ��,
                    //���� ��ġ�� Ÿ�ϸʿ� �ƹ� ��ϵ� �������� ������
                    //�ڽ��� ���
                    TileMap.TryAdd(Position, this);
                });
        }
        else
        {
            OnFailedMove();
        }
    }

    public Vector2Int lastDir { get; private set; }

    public bool CheckLine(Vector2Int Dir, out Vector2Int targetPosition, out Block hitBlock)
    {
        lastDir = Dir;

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

    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
    {
        //�ε��� �Ÿ��� 1 �Ʒ��� �и��� �ʴ´�.
        if (movementDistance < 1)
            return false;

        Vector2Int dir = Position - collisionPopsition;

        //�ε��� �Ÿ��� 1 �̻��� ��,
        bool isCleared = CheckLine(dir, out Vector2Int _, out Block _);
        if (isCleared)
        {
            collisionPopsition = Position;
        }
        return true;
    }

    public virtual void OnFailedMove()
    {
        Punching();
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        //�����ִ� �߿� �������� �޾Ҵٴ� ���� �ٸ� �����̴� ��ü�� �ε����� �з��� �Ѵٴ� �ǹ�
        if (!IsMoving)
        {
            if (hitBlock is ActionBlock actionBlock)
            {
                Dash(actionBlock.lastDir);
            }
        }
    }
}
