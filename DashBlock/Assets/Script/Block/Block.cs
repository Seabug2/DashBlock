using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Position
{
    public int x = 0;
    public int y = 0;

    public Position()
    {
        x = 0;
        y = 0;
    }

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Position Set(int x, int y)
    {
        this.x = x;
        this.y = y;
        return this;
    }
}

public class Block : MonoBehaviour
{
    BlockManager manager;
    protected BlockManager Manager => manager ??= Locator.GetUI<BlockManager>();

    [SerializeField] public int hp;
    [SerializeField] protected TextMeshPro tmp;

    // KJM
    [SerializeField] private GameObject _shockWaveObject;

    protected virtual void Start()
    {
        Position pos = GetPos();
        transform.position = new Vector3(pos.x, pos.y, 0);
        Manager.Blocks[pos.x, pos.y] = this;

        hp = Random.Range(1, 4);
        tmp.text = hp.ToString();
    }

    public bool TakeDamage(int i)
    {
        hp -= i;
        tmp.text = hp.ToString();

        if (hp <= 0)
        {
            _shockWaveObject.SetActive(true);
            Destroy(gameObject);
            return true;
        }
        else
        {
            return false;
        }
    }
    public void Punching()
    {
        transform.DOPunchScale(Vector3.one * 0.5f, .3f, 20).OnComplete(()=>transform.localScale = Vector3.one);
    }

    public Position GetPos()
    {
        int x = Mathf.RoundToInt(transform.position.x);
        int y = Mathf.RoundToInt(transform.position.y);
        return new Position(x, y);
    }
}
