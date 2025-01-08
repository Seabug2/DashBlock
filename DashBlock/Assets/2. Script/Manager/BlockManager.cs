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
                if(!Blocks.TryAdd(type, instance))
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




    static readonly Dictionary<Type, Queue<Block>> Pools = new();

    // 제네릭 타입 추가
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

    // 제네릭 타입 가져오기
    public static T GetItem<T>() where T : Block
    {
        Type type = typeof(T);

        //가져오려는 타입의 블록을 저장한 Queue가 존재한다면...
        if (Pools.TryGetValue(type, out Queue<Block> qBlock) && qBlock.Count > 0)
        {
            return (T)qBlock.Dequeue();
        }
        //Queue가 존재하지 않다면
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
