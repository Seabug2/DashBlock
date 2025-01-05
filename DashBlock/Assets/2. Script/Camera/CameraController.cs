using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CameraController
{
    static Camera main;
    static Vector3 originPos;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Init()
    {
        SceneManager.sceneLoaded += CameraSetting;
    }

    static void CameraSetting(Scene scene, LoadSceneMode loadSceneMode)
    {
        main = Camera.main;

        if (main != null)
        {
            originPos = main.transform.position;
            Debug.Log("카메라 등록 성공");
        }
        else
        {
            Debug.Log("카메라 등록 못함");
        }
    }

    public static void Shake(float duration, float power, Action OnCompleted = null)
    {
        if (main == null) return;
        main.transform.DOKill();
        main.transform.DOShakePosition(duration * 0.9f, power, 20).OnComplete(() =>
         {
             main.transform.DOMove(originPos, duration * 0.1f).SetEase(Ease.OutQuad)
             .OnComplete(() => OnCompleted?.Invoke());
         });
    }
}
