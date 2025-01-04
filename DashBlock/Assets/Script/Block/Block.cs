using DG.Tweening;
using TMPro;
using UnityEngine;

public struct BlockPosition
{
    public sbyte x, y;
    public BlockPosition(sbyte x, sbyte y)
    {
        this.x = x;
        this.y = y;
    }

    public static BlockPosition operator +(BlockPosition a, BlockPosition b)
    {
        return new BlockPosition((sbyte)(a.x + b.x), (sbyte)(a.y + b.y));
    }

    public static BlockPosition operator -(BlockPosition a, BlockPosition b)
    {
        return new BlockPosition((sbyte)(a.x - b.x), (sbyte)(a.y - b.y));
    }

    // Equals ������ (�� ��)
    public override bool Equals(object obj)
    {
        if (obj is BlockPosition other)
        {
            return this.x == other.x && this.y == other.y;
        }
        return false;
    }

    // GetHashCode ������ (�ؽ� �ڵ� ����)
    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

    // ToString ������ (����� ����)
    public override string ToString()
    {
        return $"({x}, {y})";
    }

    // BlockPosition -> Vector2 (�Ϲ��� ��ȯ)
    public static implicit operator Vector2(BlockPosition pos)
    {
        return new Vector2(pos.x, pos.y);
    }
}

public class Block : MonoBehaviour
{
    [SerializeField, Header("�ʱ� ü��"), Space(10)]
    protected sbyte initialLife = -1;
    public sbyte HP { get; protected set; }

    [SerializeField] protected TextMeshPro tmp;

    // KJM
    ShockWave shockWaveObject;
    protected ShockWave ShockWaveObject => shockWaveObject ??= Locator.GetUI<ShockWave>();

    protected virtual void Awake()
    {
        if (!BlockManager.Blocks.TryAdd(Position, this))
        {
            Destroy(gameObject);
            return;
        }
    }

    //TODO : ���� ��ġ�� ���� ����
    protected virtual void Start()
    {
        Init();
    }

    void Init()
    {
        HP = initialLife <= 0 ? (sbyte)Random.Range(1, 4) : initialLife;
        tmp.text = HP.ToString();
    }

    public virtual bool TakeDamage(sbyte i = 1)
    {
        HP -= i;
        if (HP <= 0)
        {
            BlockManager.Remove(Position);
            gameObject.SetActive(false);
            return true;
        }
        else
        {
            tmp.text = HP.ToString();
            return false;
        }
    }

    public void Punching()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }

    public BlockPosition Position
    {
        get
        {
            sbyte x = (sbyte)Mathf.RoundToInt(transform.position.x);
            sbyte y = (sbyte)Mathf.RoundToInt(transform.position.y);
            return new BlockPosition(x, y);
        }
    }
}
