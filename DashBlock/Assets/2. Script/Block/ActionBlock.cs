using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    #region 움직이는 블록의 수를 계산
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
    #endregion
    



    public override void TakeDamage(Block hitBlock = null)
    {
        if (IsMoving)
        {
            if (hitBlock == null || hitBlock.Damage > 0)
            {
                OnCollision?.Invoke(this);
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

    public override bool TryCollision(ActionBlock hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
    {
        return movementDistance > 0;
    }





    public Action<ActionBlock> OnMoveBegin;
    public static Action<ActionBlock> OnCollision;

    public Vector2Int LastDir { get; private set; }

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
            OnMoveBegin?.Invoke(this);

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
                });
        }
        else
        {
            OnFailedMove();
        }
    }

    public static Action<ActionBlock> OnFailedMoveAction;
    public virtual void OnFailedMove()
    {
        OnFailedMoveAction?.Invoke(this);
        Punching();
    }

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
            return hitBlock.TryCollision(this, ref targetPosition, movementDistance);
        }
    }
}
