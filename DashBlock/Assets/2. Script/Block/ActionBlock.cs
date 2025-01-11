using DG.Tweening;
using UnityEngine;
using System;

public class ActionBlock : Block
{
    public static sbyte ActiveMovingBlocks = 0;

    /// <summary>
    /// 움직이고 있는 Action Block이 하나라도 있다면 true를 반환
    /// </summary>
    public static bool IsMoving => ActiveMovingBlocks > 0;




    public void CheckLine(BlockPosition dir)
    {
        BlockPosition targetPos = Position;
        BlockPosition nextPosition;
        int moveDistance = 0;
        Block hitBlock = null;

        while (true)
        {
            nextPosition = targetPos + dir;

            if (nextPosition.x < 0 || nextPosition.x > BlockManager.limit_x
                || nextPosition.y < 0 || nextPosition.y > BlockManager.limit_y)
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

        if (hitBlock == null && moveDistance == 0 )
        {
            OnFailedMove();
        }
        else if (hitBlock != null && moveDistance < hitBlock.MinimunRange())
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

        transform
            .DOMove(targetPosition, .1f)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                target?.TakeDamage(HitBlock: this);

                CameraController.Shake(0.3f, 0.4f);
                ActiveMovingBlocks--;
                TakeDamage();
            });
    }

    public event Action OnStartedMove;


    public virtual void OnFailedMove()
    { }
}
