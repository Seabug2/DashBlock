using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static ActionBlock ActionBlock;
    public static string MapName;
    public static sbyte limit_x;
    public static sbyte limit_y;

    public static readonly Dictionary<BlockPosition, Block> Blocks = new();

    // ��� ����� ���ŵ� �� ȣ��� �̺�Ʈ
    public static event Action OnCompleteAction;

    static sbyte targetCount = 0;
    public static void Reset()
    {
        Blocks.Clear();
        targetCount = 0;
        OnCompleteAction = null;
    }

    // ����� �ִ� ��ġ�� ��ȯ
    public static BlockPosition GetLimit()
    {
        sbyte limit_x = 0;
        sbyte limit_y = 0;

        foreach (Block b in Blocks.Values)
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
        if (Blocks.ContainsKey(key))
        {
            Blocks.Remove(key);
            Debug.Log($"��� ���� ����: {key}");

            if (Blocks.Count == 0)
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
    /*
    static readonly Dictionary<Type, Queue<Type>> Pools = new();
    // ���׸� Ÿ�� �߰�
    public static void AddItem<T>(T item)
    {
        Type type = typeof(T);
        if (!Pools.ContainsKey(type))
        {
            Pools[type] = new Queue<T>();
        }

        Pools[type].Enqueue(item);
    }

    // ���׸� Ÿ�� ��������
    public static T GetItem<T>()
    {
        Type type = typeof(T);
        if (Pools.ContainsKey(type) && Pools[type].Count > 0)
        {
            return (T)Pools[type].Dequeue();
        }

        throw new InvalidOperationException($"No items of type {type} found.");
    }
    */
}
