using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DashBlock : ActionBlock
{
    #region 싱글톤
    static DashBlock player;
    public static DashBlock Player => player;

    void Awake()
    {
        if (player == null)
        {
            player = this;
            Pools[GetType().Name[0] - 'A'].Enqueue(this);
            GameStart();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }


    private void OnDestroy()
    {
        if (player == this)
        {
            player = null;
        }
    }
    #endregion

    void GameStart()
    {
        MapLoader.Initialize().Forget();
    }



    bool isGameOVer;
    public bool IsGameOVer
    {
        get
        {
            return isGameOVer;
        }
        private set
        {
            isGameOVer = value;

            GetComponent<SpriteRenderer>().enabled = !isGameOVer;
            TMP.gameObject.SetActive(!isGameOVer);
        }
    }

    public override void Init(Vector3 position, int hp)
    {
        transform.position = position;
        HP = 99;
        IsGameOVer = false;
        gameObject.SetActive(true);
    }

    public UnityEvent OnStartedGame;
    public UnityEvent OnFailedGame;

    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public override void Dash(Vector2Int Dir)
    {
        //Dir 방향으로 이동하였을 때 이동이 가능한가?
        if (CheckLine(Dir, out Vector2Int targetPosition, out Block hitBlock))
        {
            IsMoving = true;

            transform
                .DOMove(new Vector3(targetPosition.x, targetPosition.y, 0), .1f)
                .SetEase(Ease.InQuart)
                .OnComplete(() =>
                {
                    if (hitBlock != null)
                    {
                        hitBlock.TakeDamage(this);
                    }

                    TakeDamage(hitBlock);
                    CameraController.Shake(0.3f, 0.4f);
                    IsMoving = false;
                });
        }
        else
        {
            OnFailedMove();
        }
    }

    public override void OnFailedMove()
    {
        IsMoving = true;

        // TODO : 이동에 실패했을 때 소리를 재생
        transform
            .DOShakePosition(0.3f, .3f, 20)
            .OnComplete(() =>
            {
                IsMoving = false; ;
            });
    }



    public override void TakeDamage(Block hitBlock = null)
    {
        CameraController.Shake(0.23f,0.45f);
        HP--;
    }

    protected override void OnBlockDestroyed()
    {
        CameraController.BreakEffect();
        OnFailedGame.Invoke();
    }
}
