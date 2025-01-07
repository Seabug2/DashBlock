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

    // Equals 재정의 (값 비교)
    public override bool Equals(object obj)
    {
        if (obj is BlockPosition other)
        {
            return this.x == other.x && this.y == other.y;
        }
        return false;
    }

    // GetHashCode 재정의 (해시 코드 생성)
    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

    // ToString 재정의 (디버깅 편의)
    public override string ToString()
    {
        return $"({x}, {y})";
    }

    // BlockPosition -> Vector2 (암묵적 변환)
    public static implicit operator Vector2(BlockPosition pos)
    {
        return new Vector2(pos.x, pos.y);
    }

    // BlockPosition -> Vector2 (암묵적 변환)
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
        //모든 블록은 파괴될 때 화면의 색을 바꾼다.
        //ShockWave를 등록해야할 듯
        CameraController.BreakEvent();

        //pull에 자신을 되돌리는 코드
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
