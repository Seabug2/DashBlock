using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Block : MonoBehaviour
{
    #region 블록 관리 / 오브젝트 풀링 사용 / Queue<Block>[] 사용
    protected static Block[] Prefabs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadAllBlock()
    {
        //리소스 폴더에 있는 블록 프리팹을 전부 로드하고
        //프리팹 파일 이름 첫 글자의 ASCII 번호를 따서 인덱스로 사용하여 배열에 저장
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
    public static Block GetBlock(char c)
    {
        int i = c - 'A';
        return GetBlock(i);
    }

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

    //집어넣기
    protected void Return()
    {
        //이름의 규칙을 지킬 것
        Pools[name[0] - 'A'].Enqueue(this);

        TileMap.Remove(Position);
        gameObject.SetActive(false);
    }
    #endregion



    #region 타일맵
    //생성된 블록을 타일맵으로 관리하는 Dictionary
    public static readonly Dictionary<Vector2Int, Block> TileMap = new();
    public static int BlockCount = 0;
    public static void ResetTileMap()
    {
        if (TileMap.Values.Count > 0)
        {
            Block[] values = TileMap.Values.ToArray();

            foreach (Block b in values)
            {
                b.Return();
            }

            TileMap.Clear();
        }
        BlockCount = 0;
    }

    public static int limit_x;
    public static int limit_y;
    #endregion




    TextMeshPro tmp;
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
        }
    }

    /// <summary>
    /// 부딪힌 상대방 블록에게 줄 데미지
    /// </summary>
    [SerializeField] int damage = 1;
    public int Damage => damage;

    protected TextMeshPro TMP => tmp ??= GetComponentInChildren<TextMeshPro>(true);




    public virtual void Init(Vector3 position, int hp)
    {
        transform.position = position;

        if (TileMap.TryAdd(Position, this))
        {
            HP = hp > 0 ? hp : 1;
            gameObject.SetActive(true);
        }
        else
        {
            Return();
        }
    }

    /// <summary>
    /// 블록이 특정 거리를 이동하여 어떤 위치에서 부딪혔을 때 그 블록이 이동할 수 있는지, 가능하다면 어느 위치까지 이동할 수 있는지를 반환
    /// </summary>
    /// <param name="hitBlock">부딪히려는 상대방 블록</param>
    /// <param name="collisionPosition">충돌을 계산할 위치</param>
    /// <param name="movementDistance">부딪히려는 블록이 이동한 거리</param>
    public virtual bool TryCollision(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        if (movementDistance < 1)
            return false;

        collisionPosition = (HP <= hitBlock.Damage) ? Position : collisionPosition;
        return true;
    }

    public virtual void TakeDamage(Block HitBlock = null)
    {
        HP -= HitBlock.damage;

        if (HP > 0)
        {
            Punching();
        }
    }

    public static Action<Block> OnDestroyed;
    protected virtual void OnBlockDestroyed()
    {
        OnDestroyed?.Invoke(this);
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
