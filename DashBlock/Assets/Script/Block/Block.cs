using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

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
    sbyte hp;
    [SerializeField]
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
                Destroy(gameObject);
            }
            else
            {
                tmp.text = hp.ToString();
            }
        }
    }

    [SerializeField] protected TextMeshPro tmp;

    // KJM
    ShockWave shockWaveObject;
    protected ShockWave ShockWaveObject => shockWaveObject ??= Locator.GetUI<ShockWave>();

    protected virtual void Awake()
    {
        BlockManager.Add(GetPos(), this);
    }

    //TODO : ���� ��ġ�� ���� ����
    protected virtual void Start()
    {
        HP = (sbyte)Random.Range(1, 4);
    }

    public bool TakeDamage(sbyte i = 1)
    {
        HP -= i;

        if (HP <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Punching()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }

    public BlockPosition GetPos()
    {
        sbyte x = (sbyte)Mathf.RoundToInt(transform.position.x);
        sbyte y = (sbyte)Mathf.RoundToInt(transform.position.y);
        return new BlockPosition(x, y);
    }

    protected virtual void OnDestroy()
    {
        BlockManager.Remove(GetPos());
    }
}
