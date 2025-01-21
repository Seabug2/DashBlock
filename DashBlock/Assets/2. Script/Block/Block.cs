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
        }
    }

    [SerializeField] int collisionDamage = 1;
    public int CollisionDamage => collisionDamage;

    public virtual void TakeDamage(Block HitBlock = null)
    {
        HP -= HitBlock.collisionDamage;

        if (HP > 0)
        {
            Punching();
        }
    }


    public virtual bool CanMove(ActionBlock hitBlock, ref Vector2Int collisionPosition, int movementDistance)
    {
        //충돌거리가 1보다 작으면 이동 못함
        if (movementDistance < 1)
            return false;

        collisionPosition = (HP <= hitBlock.CollisionDamage) ? Position : collisionPosition;
        return true;
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
