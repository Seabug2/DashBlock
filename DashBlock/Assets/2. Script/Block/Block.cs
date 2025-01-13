using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Block : MonoBehaviour
{
    /// <summary>
    /// 블록 프리팹을 Dictionary로 저장하고 관리
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

    //사용을 마친 블록을 관리하는 Pooling용 Queue 배열
    protected static Queue<Block>[] Pools;




    //가져오기
    public static Block GetBlock(int i)
    {
        if (i < 0 || i >= Pools.Length)
        {
            i = 0;
            Debug.LogError("인덱싱 오류");
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




    //집어넣기
    protected void Return()
    {
        Pools[GetType().Name[0] - 'A'].Enqueue(this);
        TileMap.Remove(Position);
        gameObject.SetActive(false);
    }

    public static void @Reset()
    {
        //TODO : 남아있는 생성 블록을 다시 Pool에 넣는다.
        foreach (Block b in TileMap.Values)
        {
            b.Return();
        }

        TileMap.Clear();
        BlockCount = 0;
    }


    //생성된 블록을 타일맵으로 관리하는 Dictionary
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
    /// 데미지를 받아 hp가 0이 되면 객체는 파괴된다.
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
