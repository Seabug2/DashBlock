using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class DashBlock : ActionBlock
{
    #region
    static DashBlock player;
    public static DashBlock Player => player;

    void Awake()
    {
        if (player == null)
        {
            player = this;
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



    public override void Init(Vector3 position, int hp)
    {
        transform.position = position;
        HP = 99;

        GetComponent<SpriteRenderer>().enabled = true;
        TMP.gameObject.SetActive(true);
    }

    public UnityEvent OnStartedGame;
    public UnityEvent OnFailedGame;

    public override void OnFailedMove()
    {
        IsMoving = true;
        
        // TODO : 이동에 실패했을 때 소리를 재생
        transform
            .DOShakePosition(0.3f, .3f, 20)
            .OnComplete(() =>
            {
                IsMoving = false;;
            });
    }




    protected override void OnBlockDestroyed()
    {
        OnFaildGame();
    }

    void OnFaildGame()
    {
        CameraController.BreakEffect();

        GetComponent<SpriteRenderer>().enabled = false;
        TMP.gameObject.SetActive(false);

        OnFailedGame.Invoke();
    }
}
