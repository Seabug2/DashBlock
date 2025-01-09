using DG.Tweening;
using UnityEngine;
using System;

public class ActionBlock : Block
{
    public static sbyte ActiveMovingBlocks = 0;
    public static bool IsMoving => ActiveMovingBlocks > 0;

    /// <summary>
    /// 블록이 움직이려는 방향으로 경로를 검사를 합니다.
    /// 움직일 수 없는 경우
    /// 벽 끝까지 미끄러지는 경우
    /// 블록에 부딪히는 경우
    /// </summary>
    /// <param name="dynamicBlock">움직이려는 블록</param>
    /// <param name="dir">움직이려는 방향</param>
    public void CheckLine(BlockPosition dir)
    {
        BlockPosition targetPos = new(transform.position);
        sbyte moveDistance = 0;

        //부딪힐 블록
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
    /// 벽돌에 부딪히는 경우
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
