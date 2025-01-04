using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BlockManager
{
    public static readonly Dictionary<BlockPosition, Block> Blocks = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void RegisterUnloadSceneEvent()
    {
        SceneManager.sceneUnloaded += (_) => Blocks.Clear();
    }

    public static BlockPosition GetLimit()
    {
        sbyte limit_x = 0;
        sbyte limit_y = 0;
        BlockPosition p;

        foreach (Block b in Blocks.Values)
        {
            p = b.GetPos();
            limit_x  = p.x > limit_x ? p.x : limit_x;
            limit_y  = p.y > limit_y ? p.y : limit_y;
        }

        return new BlockPosition(limit_x, limit_y);
    }

    public static Block Get(BlockPosition key)
    {
        return Blocks[key];
    }

    public static bool TryGet(BlockPosition key, out Block block)
    {
        if (Blocks.ContainsKey(key))
        {
            block = Blocks[key];
            return true;
        }
        else
        {
            block = null;
            return false;
        }
    }

    public static void Add(BlockPosition key, Block block)
    {
        if (Blocks.ContainsKey(key))
        {
            UnityEngine.Object.Destroy(block.gameObject);
            Debug.Log("��� ����, �ߺ��� ����� �����մϴ�.");
        }
        else
        {
            Debug.Log("��� ����");
            Blocks.Add(key, block);
        }
    }

    public static void Remove(BlockPosition key)
    {
        if (Blocks.ContainsKey(key))
        {
            Debug.Log("��� ���� ����");
            Blocks.Remove(key);             // ��ųʸ����� ����
        }
        else
        {
            Debug.Log("��� ���� ����: �ش� Ű�� �������� ����");
        }
    }
}
