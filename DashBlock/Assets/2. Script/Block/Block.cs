using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Block : MonoBehaviour
{
    /// <summary>
    /// ��� �������� Dictionary�� �����ϰ� ����
    /// </summary>
    protected static Block[] Prefabs;

    [RuntimeInitializeOnLoadMethod]
    static void LoadAllBlock()
    {
        Block[] resources = Resources.LoadAll<Block>("Blocks");
        int size = resources.Length;
        Prefabs = new Block[size];
        Pools = new Queue<Block>[size];

        foreach (Block b in resources)
        {
            string name = b.name;
            int number = name[0] - 'A';
            Prefabs[number] = b;
            Pools[number] = new Queue<Block>();
        }
    }

    //����� ��ģ ����� �����ϴ� Pooling�� Queue �迭
    protected static Queue<Block>[] Pools;




    //��������
    public static Block GetBlock(int i)
    {
        if (i < 0 || i >= Pools.Length)
        {
            i = 0;
            Debug.LogError("�ε��� ����");
        }

        Queue<Block> q = Pools[i];
        if (q.Count == 0)
        {
            return Instantiate(Prefabs[i]);
        }

        return q.Dequeue();
    }

    public static Block GetBlock<T>()
    {
        Type type = typeof(T);
        int i = type.Name[0];

        return GetBlock(i);
    }




    //����ֱ�
    protected void Return()
    {
        Pools[GetType().Name[0] - 'A'].Enqueue(this);
        TileMap.Remove(Position);
        gameObject.SetActive(false);
    }

    public static void @Reset()
    {
        //TODO : �����ִ� ���� ����� �ٽ� Pool�� �ִ´�.
        foreach (Block b in TileMap.Values)
        {
            b.Return();
        }

        TileMap.Clear();
        BlockCount = 0;
    }


    //������ ����� Ÿ�ϸ����� �����ϴ� Dictionary
    public static readonly Dictionary<Vector2Int, Block> TileMap = new();
    public static int BlockCount = 0;

    public static int limit_x;
    public static int limit_y;




    TextMeshPro tmp;
    protected TextMeshPro TMP => tmp ??= GetComponentInChildren<TextMeshPro>(true);

    public Vector2Int Position
    {
        get
        {
            int x = Mathf.RoundToInt(transform.position.x);
            int y = Mathf.RoundToInt(transform.position.y);
            return new(x, y);
        }
    }

    int hp;
    /// <summary>
    /// �������� �޾� hp�� 0�� �Ǹ� ��ü�� �ı��ȴ�.
    /// </summary>
    public int HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;

            if (hp <= 0)
            {
                OnBlockDestroyed();
                return;
            }

            TMP.text = hp.ToString();
            Punching();
        }
    }

    public virtual int MinimunRange()
    {
        return 1;
    }

    public virtual void TakeDamage(Block HitBlock = null)
    {
        HP--;
    }

    public virtual void Init(Vector3 position, int hp)
    {
        transform.position = position;
        HP = hp;
        gameObject.SetActive(true);

        if (!TileMap.TryAdd(Position, this))
        {
            Return();
            return;
        }
    }

    public virtual Vector2Int CollisionPosition(Block hitBlock, Vector2Int collisionDir, int hitDistance)
    {
        return HP > 1 ? Position + collisionDir : Position;
    }

    protected virtual void OnBlockDestroyed()
    {
        CameraController.BreakEffect();
        BlockCount--;
        Return();
    }

    public void Punching()
    {
        transform.DOKill();
        transform.localScale = Vector3.one;
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }
}
