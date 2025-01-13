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




    //생성된 블록을 타일맵으로 관리하는 Dictionary
    public static readonly Dictionary<Vector2Int, Block> TileMap = new();
    public static int BlockCount = 0;
    public static void ResetTileMap()
    {
        //TODO : 남아있는 생성 블록을 다시 Pool에 넣는다.
        foreach (Block b in TileMap.Values)
        {
            b.Return();
        }

        TileMap.Clear();
        BlockCount = 0;
    }

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

    /// <summary>
    /// 특정 거리를 움직인 Block과 부딪힌 결과,
    /// 자신이 Clear 되었는지 유무를 반환
    /// </summary>
    public virtual bool IsClear(Block hitBlock, Vector2Int collisionDirection, int movementDistance)
    {
        //충돌거리가 1보다 작으면 이동 못함
        if (movementDistance < 1)
            return false;

        //HP가 1 이하라면 이 블록의 위치까지 이동한다.
        return HP == 1;
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

    public static int DashDistance(Vector2Int distance)
    {
        return Mathf.RoundToInt(distance.magnitude);
    }

    public static Vector2Int GetDir(Block targetBlock, Block movingBlock)
    {
        Vector3 dir = targetBlock.transform.position - movingBlock.transform.position;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            int x = dir.x > 0 ? 1 : -1;
            return new Vector2Int(x, 0);
        }
        else
        {
            int y = dir.y > 0 ? 1 : -1;
            return new Vector2Int(0, y);
        }
    }
}
