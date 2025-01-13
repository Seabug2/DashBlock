using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    //FixBlock과 부딪히면 이 블록의 위치까지 오게 된다. 
    public override Vector2Int CollisionPosition(Block hitBlock, Vector2Int collisionDir, int hitDistance)
    {
        if (FixedBlock.CheckLine(collisionDir, out Vector2Int _, out Block _))
        {
            return Position - collisionDir;
        }

        TileMap.Remove(Position);
        return Position;
    }

    public override void TakeDamage(Block HitBlock = null)
    {
        Punching();

        if (HitBlock != null && HitBlock is ActionBlock actionBlock)
        {
        }
    }

    void ReleaseBlock()
    {
        //이 블록이 되돌아가려고 하는데,
        //그 자리에 이미 블록이 존재한다?
        if (!TileMap.TryAdd(Position, this))
        {
            if (TileMap.TryGetValue(Position, out Block nextBlock) && nextBlock is ActionBlock actionBlock)
            {
                
            }
        }

        //if (TileMap.TryAdd(Position, this))
        //{
        //    Punching();
        //}
        //else
        //{
        //    Debug.Log("FIx 블록 뭔가 잘못됨");
        //    Return();
        //}
    }
}
