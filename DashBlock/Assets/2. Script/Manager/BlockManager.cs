using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    /// <summary>
    /// ��� ������
    /// </summary>
    public static readonly Dictionary<Type, Block> Blocks = new();

    [RuntimeInitializeOnLoadMethod]
    static void LoadAllBlock()
    {
        GameObject[] blocks = Resources.LoadAll<GameObject>("Blocks");
        foreach (GameObject block in blocks)
        {
            if (block.TryGetComponent(out Block instance))
            {
                Type type = instance.GetType();
                if(!Blocks.TryAdd(type, instance))
                {
                    Debug.LogWarning($"{block.name} �߰� ����");
                }
            }
        }
    }

    public static PlayerBlock PlayerBlock = null;




    public static sbyte limit_x;
    public static sbyte limit_y;

    public static readonly Dictionary<BlockPosition, Block> Tiles = new();

    // ��� ����� ���ŵ� �� ȣ��� �̺�Ʈ
    public static event Action OnCompleteAction;

    static sbyte remainCount = 0;

    /// <summary>
    /// 0�� �Ҵ��Ϸ��� remainCount�� ����Ͻÿ�
    /// </summary>
    public static sbyte RemainCount
    {
        get
        {
            return remainCount;
        }
        set
        {
            remainCount = value;

            if (remainCount == 0)
            {
                OnCompleteAction?.Invoke();
            }
        }
    }

    public static void ResetGame()
    {
        //TODO : ������ ��� ����� �ٽ� Pool�� �ִ´�.
        Tiles.Clear();
        remainCount = 0;
    }




    static readonly Dictionary<Type, Queue<Block>> Pools = new();

    // ���׸� Ÿ�� �߰�
    public static void Enqueue(Type type, Block item)
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
        item.gameObject.SetActive(false);
    }

    // ���׸� Ÿ�� ��������
    public static T GetItem<T>() where T : Block
    {
        Type type = typeof(T);

        //���������� Ÿ���� ����� ������ Queue�� �����Ѵٸ�...
        if (Pools.TryGetValue(type, out Queue<Block> qBlock) && qBlock.Count > 0)
        {
            return (T)qBlock.Dequeue();
        }
        //Queue�� �������� �ʴٸ�
        else
        {
            if (Blocks.TryGetValue(type, out Block instance))
            {
                Block block = GameObject.Instantiate(instance);
                return (T)block;
            }
        }
        throw new InvalidOperationException($"No items of type {type} found.");
    }
}
