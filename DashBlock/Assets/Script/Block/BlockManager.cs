using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static ActionBlock ActionBlock;
    public static readonly Dictionary<BlockPosition, Block> Blocks = new();

    // ��� ����� ���ŵ� �� ȣ��� �̺�Ʈ
    public static event Action OnCompleteAction;

    // �� ��ε� �� ��ųʸ��� �̺�Ʈ �ʱ�ȭ
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void RegisterUnloadSceneEvent()
    {
        SceneManager.sceneUnloaded += (scene) =>
        {
            ActionBlock = null;
            Blocks.Clear();
            OnCompleteAction = null;
            Debug.Log($"�� {scene.name}�� ��ε�Ǿ����ϴ�. ��� ����� ���ŵǾ����ϴ�.");
        };
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
}
