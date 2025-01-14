using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    public override bool IsClear(Block hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //속이 비었다면 부딪힌 블록은 이 블록의 위치까지 이동한다.
        if (FixedBlock == null)
        {
            collisionPosition = Position;
            return true;
        }

        //비어있지 않은 경우
        //현재 붙잡고 있는 블록의 자리 이동이 가능한지 확인
        Vector2Int dir = Position - collisionPosition;

        //이동 목적지는 필요없음, 부딪힐 블록도 상관없음
        bool isCleared = FixedBlock.CheckLine(dir, out Vector2Int _, out Block _);
        if (isCleared)
        {
            //이동이 가능하다면 움직이려는 블록의 목적지를 자신의 위치로 정한다.
            collisionPosition = Position;
        }
        return isCleared;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        //부딪힌 블록이 존재하고,
        //부딪힌 블록이 ActionBlock이면,
        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
            if (FixedBlock != null)
            {
                //붙잡고 있는 블록이 존재하는데 액션블록이 부딪혔다는 의미는,
                //이미 이전에 잡고 있는 블록이 밀려날 수 있다고 계산이 끝난 것
                //블록과 부딪히면 붙잡고 있던 블록을 움직이면 된다.
                FixedBlock.TakeDamage(HitBlock);
            }

            FixedBlock = actionBlock;
        }

        Punching();
    }
}
