using DG.Tweening;
using TMPro;
using UnityEngine;

public class Block : MonoBehaviour
{
    public int MyQueueNumber;

    TextMeshPro tmp;
    protected TextMeshPro TMP => tmp ??= GetComponentInChildren<TextMeshPro>(true);

    public Vector2Int Position
    {
        get
        {
            int x = Mathf.RoundToInt(transform.position.x);
            int y= Mathf.RoundToInt(transform.position.y);
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
            Punching();
        }
    }

    /// <summary>
    /// 블록의 현재 HP가 받을 데미지 이하라면 파괴될 것
    /// </summary>
    public virtual bool CanBeDestroyed(int damage = 1)
    {
        return HP <= damage;
    }

    public virtual int MinimunRange()
    {
        return 1;
    }

    public virtual void TakeDamage(int damage = 1, Block HitBlock = null)
    {
        HP -= damage;
    }

    public virtual void Init(Vector3 position, int hp)
    {
        transform.position = position;
        HP = hp;
        gameObject.SetActive(true);

        if (!BlockManager.Tiles.TryAdd(Position, this))
        {
            BlockManager.Enqueue(MyQueueNumber, this);
            return;
        }
    }




    protected virtual void OnBlockDestroyed()
    {
        //모든 블록은 파괴될 때 화면의 색을 바꾼다.
        //ShockWave를 등록해야할 듯
        CameraController.BreakEffect();

        BlockManager.RemainCount--;
        //pull에 자신을 되돌리는 코드
        BlockManager.Enqueue(MyQueueNumber, this);
    }

    public void Punching()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }
}
