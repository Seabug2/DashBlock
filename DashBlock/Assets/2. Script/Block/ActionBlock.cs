using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    public static int MovingBlockCount = 0;

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
            MovingBlockCount += value ? 1 : -1;

            if (MovingBlockCount < 0)
            {
                MovingBlockCount = 0;
            }
        }
    }

    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        //Dir 방향으로 이동하였을 때 이동이 가능한가?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            // Dir 방향으로 이동하면 targetPosition 위치까지 이동할 것이고
            // hitBlock에 부딪힐 것

            //자신이 가진 키값이 타일맵에 존재하며
            //그 저장된 값이 자신이라면 타일맵에서 제거
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

                    //이동을 마치고,
                    //현재 위치의 타일맵에 아무 블록도 존재하지 않으면
                    //자신을 등록
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
            //다음 위치까지는 못가고... 원래 targetPosition까지만 이동
            return movementDistance > 0;
        }
        else
        {
            //이동에 실패한 경우

            //이동에 성공한 경우
            ////////블록 앞까지 이동
            ////////블록 위치까지 이동

            return hitBlock.IsCleared(this, ref targetPosition, movementDistance);
        }
    }

    public override bool IsCleared(Block hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
    {
        //부딪힌 거리가 1 아래면 밀리지 않는다.
        if (movementDistance < 1)
            return false;

        Vector2Int dir = Position - collisionPopsition;

        //부딪힌 거리가 1 이상일 때,
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
        //멈춰있는 중에 데미지를 받았다는 것은 다른 움직이는 물체에 부딪혀서 밀려야 한다는 의미
        if (!IsMoving)
        {
            if (hitBlock is ActionBlock actionBlock)
            {
                Dash(actionBlock.lastDir);
            }
        }
    }
}
