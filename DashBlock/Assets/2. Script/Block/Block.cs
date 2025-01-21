using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Block : MonoBehaviour
{
    #region ��� ���� / ������Ʈ Ǯ�� ��� / Queue<Block>[] ���
    protected static Block[] Prefabs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void LoadAllBlock()
    {
        //���ҽ� ������ �ִ� ��� �������� ���� �ε��ϰ�
        //������ ���� �̸� ù ������ ASCII ��ȣ�� ���� �ε����� ����Ͽ� �迭�� ����
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
            Debug.LogError("�ε��� ����");
        }

        Queue<Block> q = Pools[i];

        if (q.Count == 0)
        {
            return Instantiate(Prefabs[i]);
        }

        return q.Dequeue();
    }

    //����ֱ�
    protected void Return()
    {
        //�̸��� ��Ģ�� ��ų ��
        Pools[name[0] - 'A'].Enqueue(this);

        TileMap.Remove(Position);
        gameObject.SetActive(false);
    }
    #endregion



    #region Ÿ�ϸ�
    //������ ����� Ÿ�ϸ����� �����ϴ� Dictionary
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
        //�浹�Ÿ��� 1���� ������ �̵� ����
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
