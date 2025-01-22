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
    /// �ε��� ���� ��Ͽ��� �� ������
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
    /// ����� Ư�� �Ÿ��� �̵��Ͽ� � ��ġ���� �ε����� �� �� ����� �̵��� �� �ִ���, �����ϴٸ� ��� ��ġ���� �̵��� �� �ִ����� ��ȯ
    /// </summary>
    /// <param name="hitBlock">�ε������� ���� ���</param>
    /// <param name="collisionPosition">�浹�� ����� ��ġ</param>
    /// <param name="movementDistance">�ε������� ����� �̵��� �Ÿ�</param>
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
