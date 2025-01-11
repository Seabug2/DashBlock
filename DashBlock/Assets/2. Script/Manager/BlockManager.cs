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
                if (!Blocks.TryAdd(type, instance))
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

    /// <summary>
    /// ������ ����� ������ Pool
    /// </summary>
    static Queue<Block>[] Pools;

    public static void Enqueue(int typeID, Block item)
    {
        Tiles.Remove(item.Position);
        item.gameObject.SetActive(false);
        Pools[typeID].Enqueue(item);
    }


    static BlockPrefabsList blockData;
    public static BlockPrefabsList BlockData
    {
        get
        {
            return blockData;
        }
        set
        {
            Debug.Log($"��� ������ ���� : {value.Length}");
            Pools = new Queue<Block>[value.Length];
            for (int i = 2; i < value.Length; i++)
            {
                Pools[i] = new();
            }
            blockData = value;
        }
    }

    // ���׸� Ÿ�� ��������
    public static Block GetBlock(int blockType)
    {
        //TODO:���� �̻��ѵ�?
        blockType = Mathf.Clamp(blockType, 2, BlockData.Length - 1);

        Queue<Block> qBlock = Pools[blockType];

        if (qBlock.Count > 0)
        {
            return qBlock.Dequeue();
        }
        else
        {
            Block block = GameObject.Instantiate(BlockData[blockType]).GetComponent<Block>();
            block.myQueueNumber = blockType;
            return block;
        }
    }
}
