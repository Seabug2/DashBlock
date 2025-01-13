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

    public event Action OnDashedEvent;

    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            TileMap.Remove(Position);
            IsMoving = true;

            OnDashedEvent?.Invoke();
            OnDashedEvent = null;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Dir">이동 방향</param>
    /// <param name="targetPosition">이동할 목적지</param>
    /// <param name="hitBlock">부딪히는 블록</param>
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

        //부딪힌 블록이 없으면(벽에 부딪힌 것)
        if (hitBlock == null)
        {
            //이동한 거리에 따라 이동 가능 유무 반환
            //벽에 부딪혔을 때 이동한 거리가 1보다 작으면 이동 실패한 것 = false
            return movementDistance > 0;
        }
        else
        {
            //부딪힌 블록이 존재한 경우...
            bool moveable = hitBlock.IsClear(this, Dir, movementDistance);
            targetPosition = moveable ? nextPosition : targetPosition;
            return moveable;
        }
    }

    public override bool IsClear(Block hitBlock, Vector2Int collisionDirection, int movementDistance)
    {
        //부딪힌 거리가 1 아래면 밀리지 않는다.
        if (movementDistance < 1)
            return false;

        //부딪힌 거리가 1 이상일 때,
        return CheckLine(collisionDirection, out Vector2Int _, out Block _);
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
