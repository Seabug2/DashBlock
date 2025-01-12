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
            Punching();
        }
    }

    /// <summary>
    /// ����� ���� HP�� ���� ������ ���϶�� �ı��� ��
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
        //��� ����� �ı��� �� ȭ���� ���� �ٲ۴�.
        //ShockWave�� ����ؾ��� ��
        CameraController.BreakEffect();

        BlockManager.RemainCount--;
        //pull�� �ڽ��� �ǵ����� �ڵ�
        BlockManager.Enqueue(MyQueueNumber, this);
    }

    public void Punching()
    {
        transform.DOKill();
        transform.DOPunchScale(Vector3.one, .3f, 20).OnComplete(() => transform.localScale = Vector3.one);
    }
}
