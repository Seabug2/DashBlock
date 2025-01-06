using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static sbyte limit_x;
    public static sbyte limit_y;

    public static readonly Dictionary<BlockPosition, Block> Tiles = new();

    // ��� ����� ���ŵ� �� ȣ��� �̺�Ʈ
    public static event Action OnCompleteAction;

    public static sbyte targetCount = 0;
    public static void Reset()
    {
        Tiles.Clear();
        targetCount = 0;
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

    // ��� ����
    public static void Remove(BlockPosition key)
    {
        if (Tiles.ContainsKey(key))
        {
            Tiles.Remove(key);
            Debug.Log($"��� ���� ����: {key}");

            if (--targetCount <= 0)
            {
                OnCompleteAction?.Invoke();
                Debug.Log("��� ����� ���ŵǾ����ϴ�. OnCompleteAction ȣ��.");
            }
        }
        else
        {
            Debug.LogWarning($"��� ���� ����: Ű {key}�� �������� �ʽ��ϴ�.");
        }
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
}
