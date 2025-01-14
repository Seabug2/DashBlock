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
            
            if(ActiveMovingBlocks < 0)
            {
                ActiveMovingBlocks = 0;
            }

        }
    }

    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public virtual void Dash(Vector2Int Dir)
    {
        //Dir 방향으로 이동하였을 때 이동이 가능한가?
        //가능하다면 어디까지 이동하는지?
        //그리고 어떤 블록과 부딪히는지?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
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
                    CameraController.Shake(0.3f, 0.4f);
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
            //이동에 실패한 경우

            //이동에 성공한 경우
            ////////블록 앞까지 이동
            ////////블록 위치까지 이동

            return hitBlock.IsClear(this, ref targetPosition, movementDistance);
        }
    }

    public override bool IsClear(Block hitBlock, ref Vector2Int collisionPopsition, int movementDistance)
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
    { }

    public override void TakeDamage(Block hitBlock = null)
    {
        //멈춰있는 중에 데미지를 받았다는 것은 다른 움직이는 물체에 부딪혀서 밀려야 한다는 의미
        if (!IsMoving && HP > 1)
        {
            Vector2Int dir = GetDir(this, hitBlock);
            Dash(dir);

            return;
        }

        base.TakeDamage();
    }
}
