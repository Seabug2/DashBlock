using DG.Tweening;
using TMPro;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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

    // BlockPosition -> Vector2 (�Ϲ��� ��ȯ)
    public static implicit operator Vector3(BlockPosition pos)
    {
        return new Vector3(pos.x, pos.y, 0);
    }
}

public class Block : MonoBehaviour
{
    TextMeshPro tmp;
    protected TextMeshPro TMP => tmp ??= GetComponentInChildren<TextMeshPro>();



    public BlockPosition Position
    {
        get
        {
            sbyte x = (sbyte)Mathf.RoundToInt(transform.position.x);
            sbyte y = (sbyte)Mathf.RoundToInt(transform.position.y);
            return new BlockPosition(x, y);
        }
    }

    sbyte hp;
    public sbyte HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;

            TMP.text = hp.ToString();

            if (hp <= 0)
            {
                OnBlockDestroyed();
            }
            else
            {
                Punching();
            }
        }
    }

    public virtual bool TakeDamage()
    {
        return --HP <= 0;
    }




    public virtual void Init(BlockPosition position, sbyte hp)
    {
        transform.position = position;
        HP = hp;
        gameObject.SetActive(true);

        if (!BlockManager.Tiles.TryAdd(Position, this))
        {
            gameObject.SetActive(false);
            return;
        }
    }




    protected virtual void OnBlockDestroyed()
    {
        //��� ����� �ı��� �� ȭ���� ���� �ٲ۴�.
        //ShockWave�� ����ؾ��� ��
        CameraController.BreakEvent();

        //pull�� �ڽ��� �ǵ����� �ڵ�
        BlockManager.AddItem(GetType(), this);
        BlockManager.Tiles.Remove(Position);
        BlockManager.RemainCount--;

        gameObject.SetActive(false);
    }

    public void Punching()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }
}
