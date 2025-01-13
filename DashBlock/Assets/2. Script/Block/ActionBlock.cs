using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    public static int ActiveMovingBlocks = 0;

    /// <summary>
    /// 움직이고 있는 Action Block이 하나라도 있다면 true를 반환
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
        }
    }


    public Vector2Int Dir { get; private set; }

    public bool CheckLine(Vector2Int Dir, out Vector2Int targetPosition, out Block hitBlock)
    {
        this.Dir = Dir;
        targetPosition = Position;
        hitBlock = null;

        Vector2Int nextPosition;
        int moveDistance = 0;

        while (true)
        {
            nextPosition = targetPosition + Dir;

            //벽이나
            if (nextPosition.x < 0 || nextPosition.x > limit_x
                || nextPosition.y < 0 || nextPosition.y > limit_y)
            {
                break;
            }

            //블록에 부딪힌다면 검사를 마친다.
            if (TileMap.TryGetValue(nextPosition, out hitBlock))
            {
                //부딪힌 상대 블록에게 충돌 방향과 충돌한 거리를 주고
                //위치를 반환 받는다.
                targetPosition = hitBlock.CollisionPosition(this, Dir, moveDistance);
                break;
            }

            targetPosition = nextPosition;
            moveDistance++;
        }

        if (Position.Equals(targetPosition))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            TileMap.Remove(Position);
            IsMoving = true;

            transform
                .DOMove(new Vector3(targetPosition.x, targetPosition.y, 0), .1f)
                .SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    TakeDamage(hitBlock);
                    CameraController.Shake(0.3f, 0.4f);
                    IsMoving = false;
                });
        }
        else
        {
            OnFailedMove();
        }
    }

    public virtual void OnFailedMove()
    { }

    public override void TakeDamage(Block hitBlock = null)
    {
        //멈춰있는 중에 데미지를 받았다는 것은 다른 움직이는 물체에 부딪혀서 밀려야 한다는 의미
        if (!IsMoving && HP > 1)
        {
            Vector2 dir = transform.position - DashBlock.Player.transform.position;

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            {
                int x = dir.x > 0 ? 1 : -1;
                Dash(new Vector2Int(x, 0));
            }
            else
            {
                int y = dir.y > 0 ? 1 : -1;
                Dash(new Vector2Int(0, y));
            }

            return;
        }

        base.TakeDamage();
        TileMap.TryAdd(Position, this);
        hitBlock?.TakeDamage(this);
    }
}
