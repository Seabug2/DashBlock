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

    // 모든 블록이 제거된 후 호출될 이벤트
    public static event Action OnCompleteAction;

    static sbyte targetCount = 0;
    public static void Reset()
    {
        Blocks.Clear();
        targetCount = 0;
        OnCompleteAction = null;
    }

    // 블록의 최대 위치를 반환
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

    // 블록 제거
    public static void Remove(BlockPosition key)
    {
        if (Blocks.ContainsKey(key))
        {
            Blocks.Remove(key);
            Debug.Log($"블록 제거 성공: {key}");

            if (Blocks.Count == 0)
            {
                OnCompleteAction?.Invoke();
                Debug.Log("모든 블록이 제거되었습니다. OnCompleteAction 호출.");
            }
        }
        else
        {
            Debug.LogWarning($"블록 제거 실패: 키 {key}는 존재하지 않습니다.");
        }
    }
    /*
    static readonly Dictionary<Type, Queue<Type>> Pools = new();
    // 제네릭 타입 추가
    public static void AddItem<T>(T item)
    {
        Type type = typeof(T);
        if (!Pools.ContainsKey(type))
        {
            Pools[type] = new Queue<T>();
        }

        Pools[type].Enqueue(item);
    }

    // 제네릭 타입 가져오기
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
