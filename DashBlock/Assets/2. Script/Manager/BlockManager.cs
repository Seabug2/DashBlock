using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    /// <summary>
    /// 블록 프리팹
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
                    Debug.LogWarning($"{block.name} 추가 실패");
                }
            }
        }
    }

    public static PlayerBlock PlayerBlock = null;




    public static sbyte limit_x;
    public static sbyte limit_y;

    public static readonly Dictionary<BlockPosition, Block> Tiles = new();

    // 모든 블록이 제거된 후 호출될 이벤트
    public static event Action OnCompleteAction;

    static sbyte remainCount = 0;

    /// <summary>
    /// 0을 할당하려면 remainCount을 사용하시오
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
        //TODO : 생성된 모든 블록을 다시 Pool에 넣는다.
        Tiles.Clear();
        remainCount = 0;
    }

    /// <summary>
    /// 생성한 블록을 저장할 Pool
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
            Debug.Log($"블록 데이터 개수 : {value.Length}");
            Pools = new Queue<Block>[value.Length];
            for (int i = 2; i < value.Length; i++)
            {
                Pools[i] = new();
            }
            blockData = value;
        }
    }

    // 제네릭 타입 가져오기
    public static Block GetBlock(int blockType)
    {
        //TODO:뭔가 이상한데?
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
