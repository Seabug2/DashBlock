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

    public BlockPosition(Vector2 position)
    {
        x = (sbyte)Mathf.RoundToInt(position.x);
        y = (sbyte)Mathf.RoundToInt(position.y);
    }

    public BlockPosition(Vector3 position)
    {
        x = (sbyte)Mathf.RoundToInt(position.x);
        y = (sbyte)Mathf.RoundToInt(position.y);
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
        return System.HashCode.Combine(x, y);
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

    // BlockPosition -> Vector2 (�Ϲ��� ��ȯ)
    public static implicit operator Vector3(BlockPosition pos)
    {
        return new Vector3(pos.x, pos.y, 0);
    }
}

public class Block : MonoBehaviour
{
    public static int id;

    TextMeshPro tmp;
    protected TextMeshPro TMP => tmp ??= GetComponentInChildren<TextMeshPro>(true);

    public BlockPosition Position => new (transform.position);

    sbyte hp;
    /// <summary>
    /// �������� �޾� hp�� 0�� �Ǹ� ��ü�� �ı��ȴ�.
    /// </summary>
    public sbyte HP
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

    /// <summary>
    /// ����� ���� HP�� ���� ������ ���϶�� �ı��� ��
    /// </summary>
    public virtual bool CanBeDestroyed(sbyte damage = 1)
    {
        return HP <= damage;
    }

    public virtual int MinimunRange()
    {
        return 1;
    }

    public virtual void TakeDamage(sbyte damage = 1, Block HitBlock = null)
    {
        HP -= damage;
    }




    public int myQueueNumber;

    public virtual void Init(Vector3 position, sbyte hp)
    {
        transform.position = position;
        HP = hp;
        gameObject.SetActive(true);

        if (!BlockManager.Tiles.TryAdd(Position, this))
        {
            BlockManager.Enqueue(myQueueNumber, this);
            return;
        }
    }




    protected virtual void OnBlockDestroyed()
    {
        //��� ����� �ı��� �� ȭ���� ���� �ٲ۴�.
        //ShockWave�� ����ؾ��� ��
        CameraController.BreakEffect();

        BlockManager.RemainCount--;
        //pull�� �ڽ��� �ǵ����� �ڵ�
        BlockManager.Enqueue(myQueueNumber, this);
    }

    public void Punching()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }
}
