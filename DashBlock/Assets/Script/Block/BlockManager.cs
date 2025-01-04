using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static ActionBlock ActionBlock;
    public static readonly Dictionary<BlockPosition, Block> Blocks = new();

    // 모든 블록이 제거된 후 호출될 이벤트
    public static event Action OnCompleteAction;

    // 씬 언로드 시 딕셔너리와 이벤트 초기화
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void RegisterUnloadSceneEvent()
    {
        SceneManager.sceneUnloaded += (scene) =>
        {
            ActionBlock = null;
            Blocks.Clear();
            OnCompleteAction = null;
            Debug.Log($"씬 {scene.name}이 언로드되었습니다. 모든 블록이 제거되었습니다.");
        };
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
}
