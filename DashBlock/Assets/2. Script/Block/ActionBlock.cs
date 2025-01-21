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
    /// 벽돌에 부딪히는 경우
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

            IsMoving = true;
            OnMoveBegin?.Invoke();
            OnMoveBegin = null;

            Vector2Int newPosition = targetPosition;

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

                    TileMap.TryAdd(newPosition, this);
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

            //벽에 부딪힌다면 검사를 마친다.
            if (nextPosition.x < 0 || nextPosition.x > limit_x
                || nextPosition.y < 0 || nextPosition.y > limit_y)
            {
                break;
            }

            //블록에 부딪힌다면 검사를 마친다.
            if (TileMap.TryGetValue(nextPosition, out hitBlock))
            {
                break;
            }

            movementDistance++;
            targetPosition = nextPosition;
        }

        if (hitBlock == null)
        {
            return movementDistance > 0;
        }
        else
        {
            return hitBlock.CanMove(this, ref targetPosition, movementDistance);
        }
    }




    public override bool CanMove(ActionBlock hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
    {
        return movementDistance > 0;
    }

    public virtual void OnFailedMove()
    {
        Punching();
    }

    public override void TakeDamage(Block hitBlock = null)
    {
        if (IsMoving)
        {
            if (hitBlock == null || hitBlock.CollisionDamage > 0)
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
