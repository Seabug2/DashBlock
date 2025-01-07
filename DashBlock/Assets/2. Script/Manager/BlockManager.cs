using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static PlayerBlock PlayerBlock;

    public static sbyte limit_x;
    public static sbyte limit_y;

    public static readonly Dictionary<BlockPosition, Block> Blocks = new();
    
    // 모든 블록이 제거된 후 호출될 이벤트
    public static event Action OnCompleteAction;

    static sbyte remainCount = 0;
    public static sbyte RemainCount
    {
        get
        {
            return remainCount;
        }
        set
        {
            remainCount = value;

            if(remainCount == 0)
            {
                OnCompleteAction?.Invoke();
            }
        }
    }

    public static void ResetGame()
    {
        //Block[] remainBlocks



        Blocks.Clear();
        OnCompleteAction = null;
        RemainCount = 0;
    }


    static readonly Dictionary<Type, Queue<Block>> Pools = new();

    // 제네릭 타입 추가
    public static void AddItem(Type type, Block item)
    {
        if (!typeof(Block).IsAssignableFrom(type))
        {
            return;
        }

        if (!Pools.ContainsKey(type))
        {
            Pools[type] = new Queue<Block>();
        }

        Pools[type].Enqueue(item);
    }

    // 제네릭 타입 가져오기
    public static T GetItem<T>() where T : Block
    {
        Type type = typeof(T);
        if (Pools.TryGetValue(type, out Queue<Block> qBlock) && qBlock.Count > 0)
        {
            return (T)qBlock.Dequeue();
        }
        else
        {
            //return
        }
        throw new InvalidOperationException($"No items of type {type} found.");
    }

    /// <summary>
    /// 블록이 움직이려는 방향으로 경로를 검사를 합니다.
    /// 움직일 수 없는 경우
    /// 벽 끝까지 미끄러지는 경우
    /// 블록에 부딪히는 경우
    /// </summary>
    /// <param name="dynamicBlock">움직이려는 블록</param>
    /// <param name="dir">움직이려는 방향</param>
    public static void CheckLine(ActionBlock ActionBlock, BlockPosition dir)
    {
        BlockPosition targetPos = ActionBlock.Position;
        int moveDistance = 0;

        //부딪힐 블록
        Block hitBlock = null;
        BlockPosition nextPosition;
        while (true)
        {
            sbyte limit_x = BlockManager.limit_x;
            sbyte limit_y = BlockManager.limit_y;

            nextPosition = targetPos + dir;

            if (nextPosition.x < 0 || nextPosition.x > limit_x || nextPosition.y < 0 || nextPosition.y > limit_y)
            {
                break;
            }

            if (Blocks.TryGetValue(nextPosition, out hitBlock))
            {
                break;
            }

            targetPos = nextPosition;
            moveDistance++;
        }

        if (moveDistance < 1)
        {
            ActionBlock.OnFailedMove();
        }
        else
        {
            if (hitBlock == null)
            {
                ActionBlock.Dash(targetPos);
            }
            else
            {
                ActionBlock.Dash(targetPos, hitBlock);
            }
        }
    }



    public static Block[] BlockType;
}
