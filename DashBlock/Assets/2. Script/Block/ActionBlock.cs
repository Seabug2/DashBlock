using DG.Tweening;
using UnityEngine;
using System;

public class ActionBlock : Block
{
    public static sbyte ActiveMovingBlocks = 0;
    public static bool IsMoving => ActiveMovingBlocks > 0;

    /// <summary>
    /// ����� �����̷��� �������� ��θ� �˻縦 �մϴ�.
    /// ������ �� ���� ���
    /// �� ������ �̲������� ���
    /// ��Ͽ� �ε����� ���
    /// </summary>
    /// <param name="dynamicBlock">�����̷��� ���</param>
    /// <param name="dir">�����̷��� ����</param>
    public void CheckLine(BlockPosition dir)
    {
        BlockPosition targetPos = new(transform.position);
        sbyte moveDistance = 0;

        //�ε��� ���
        Block hitBlock = null;
        BlockPosition nextPosition;
        while (true)
        {
            sbyte limit_x = BlockManager.limit_x;
            sbyte limit_y = BlockManager.limit_y;

            nextPosition = targetPos + dir;

            if (nextPosition.x < 0 || nextPosition.x > limit_x || nextPosition.y < 0 || nextPosition.y > limit_y)
            {
                break;
            }

            if (BlockManager.Tiles.TryGetValue(nextPosition, out hitBlock))
            {
                break;
            }

            targetPos = nextPosition;
            moveDistance++;
        }

        if (moveDistance < 1)
        {
            OnFailedMove();
        }
        else
        {
            Dash(targetPos, hitBlock);
        }
    }

    /// <summary>
    /// ������ �ε����� ���
    /// </summary>
    public virtual void Dash(Vector2 targetPosition, Block target)
    {
        ActiveMovingBlocks++;

        if (target != null && target.CanBeDestroyed())
        {
            targetPosition = target.transform.position;
        }

        OnStartedMove?.Invoke();
        OnStartedMove = null;

        transform
            .DOMove(targetPosition, .1f)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                target?.TakeDamage(HitBlock : this);

                CameraController.Shake(0.3f, 0.4f);
                ActiveMovingBlocks--;
                TakeDamage();
            });
    }

    public event Action OnStartedMove;


    public virtual void OnFailedMove()
    { }
}
