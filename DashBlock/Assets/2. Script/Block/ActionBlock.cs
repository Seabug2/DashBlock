using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    public static int ActiveMovingBlocks = 0;

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
            ActiveMovingBlocks += value ? 1 : -1;
            
            if(ActiveMovingBlocks < 0)
            {
                ActiveMovingBlocks = 0;
            }

        }
    }

    /// <summary>
    /// ������ �ε����� ���
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        //Dir �������� �̵��Ͽ��� �� �̵��� �����Ѱ�?
        //�����ϴٸ� ������ �̵��ϴ���?
        //�׸��� � ��ϰ� �ε�������?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
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
                    CameraController.Shake(0.3f, 0.4f);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Dir">�̵� ����</param>
    /// <param name="targetPosition">�̵��� ������</param>
    /// <param name="hitBlock">�ε����� ���</param>
    /// <returns></returns>
    public bool CheckLine(Vector2Int Dir, out Vector2Int targetPosition, out Block hitBlock)
    {
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
            return movementDistance > 0;
        }
        else
        {
            //�̵��� ������ ���

            //�̵��� ������ ���
            ////////��� �ձ��� �̵�
            ////////��� ��ġ���� �̵�

            return hitBlock.IsClear(this, ref targetPosition, movementDistance);
        }
    }

    public override bool IsClear(Block hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
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
    { }

    public override void TakeDamage(Block hitBlock = null)
    {
        //�����ִ� �߿� �������� �޾Ҵٴ� ���� �ٸ� �����̴� ��ü�� �ε����� �з��� �Ѵٴ� �ǹ�
        if (!IsMoving && HP > 1)
        {
            Vector2Int dir = GetDir(this, hitBlock);
            Dash(dir);

            return;
        }

        base.TakeDamage();
    }
}
