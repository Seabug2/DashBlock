using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static PlayerBlock PlayerBlock;

    public static sbyte limit_x;
    public static sbyte limit_y;

    public static readonly Dictionary<BlockPosition, Block> Tiles = new();

    // ��� ����� ���ŵ� �� ȣ��� �̺�Ʈ
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

    public static void Reset()
    {
        Tiles.Clear();
        RemainCount = 0;
        OnCompleteAction = null;
    }

    // ����� �ִ� ��ġ�� ��ȯ
    public static BlockPosition GetLimit()
    {
        sbyte limit_x = 0;
        sbyte limit_y = 0;

        foreach (Block b in Tiles.Values)
        {
            BlockPosition p = b.Position;
            if (p.x > limit_x) limit_x = p.x;
            if (p.y > limit_y) limit_y = p.y;
        }

        return new BlockPosition(limit_x, limit_y);
    }

    static readonly Dictionary<Type, Queue<Block>> Pools = new();

    // ���׸� Ÿ�� �߰�
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

    // ���׸� Ÿ�� ��������
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

    static Dictionary<Type, GameObject> Blocks = new();



    /// <summary>
    /// ����� �����̷��� �������� ��θ� �˻縦 �մϴ�.
    /// ������ �� ���� ���
    /// �� ������ �̲������� ���
    /// ��Ͽ� �ε����� ���
    /// </summary>
    /// <param name="dynamicBlock">�����̷��� ���</param>
    /// <param name="dir">�����̷��� ����</param>
    public static void CheckLine(ActionBlock ActionBlock, BlockPosition dir)
    {
        BlockPosition targetPos = ActionBlock.Position;
        int moveDistance = 0;

        //�ε��� ���
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

            if (Tiles.TryGetValue(nextPosition, out hitBlock))
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

}
