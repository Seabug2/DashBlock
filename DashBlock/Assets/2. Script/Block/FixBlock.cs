using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixBlock : Block
{
    ActionBlock FixedBlock;

    //FixBlock�� �ε����� �� ����� ��ġ���� ���� �ȴ�. 
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
        //�� ����� �ǵ��ư����� �ϴµ�,
        //�� �ڸ��� �̹� ����� �����Ѵ�?
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
        //    Debug.Log("FIx ��� ���� �߸���");
        //    Return();
        //}
    }
}
