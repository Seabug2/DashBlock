using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CameraController
{
    static Camera main;
    public static Camera Main => main ??= Camera.main;
    static Vector3 originPos;

    public static void SetOriginPosition(Vector3 originPos)
    {
        CameraController.originPos = originPos;
        Main.transform.position = CameraController.originPos;
    }

    public static void Shake(float duration, float power, Action OnCompleted = null)
    {
        if (Main == null) return;
        Main.transform.DOKill();
        Main.transform.DOShakePosition(duration * 0.9f, power, 20).OnComplete(() =>
         {
             Main.transform.DOMove(originPos, duration * 0.1f).SetEase(Ease.OutQuad)
             .OnComplete(() => OnCompleted?.Invoke());
         });
    }
}
