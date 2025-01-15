using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    public override bool IsCleared(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //속이 비었다면 부딪힌 블록은
        //거리에 상관없이 블록의 위치까지 이동한다.
        if (FixedBlock == null)
        {
            collisionPosition = Position;
            return true;
        }
        else
        {
            //이동 목적지는 필요없음, 부딪힐 블록도 상관없음
            bool isCleared = FixedBlock.CheckLine(hitBlock.LastDir, out Vector2Int _, out Block _);
            if (isCleared)
            {
                //이동이 가능하다면 움직이려는 블록의 목적지를 자신의 위치로 정한다.
                collisionPosition = Position;
            }
            return isCleared;
        }
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        //부딪힌 블록이 존재하고,
        //부딪힌 블록이 ActionBlock이면,
        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
            if (FixedBlock != null)
            {
                FixedBlock.TakeDamage(HitBlock);
            }

            //새로 들어온 블록을 FixedBlock에 저장
            FixedBlock = actionBlock;
            FixedBlock.OnMoveBegin += Release;
        }

        Punching();
    }

    void Release()
    {
        FixedBlock = null;
    }
}
