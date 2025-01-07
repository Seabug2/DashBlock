using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class CameraController
{
    static Camera main;
    public static Camera Main => main ??= Camera.main;

    static ColorAdjustments colorAdjustments;
    static ColorAdjustments ColorAdjustments
    {
        get
        {
            if (colorAdjustments)
            {
                Volume volume = Main.GetComponent<Volume>();
                if (volume != null && volume.profile.TryGet(out colorAdjustments))
                {
                    Debug.Log("Color Adjustments가 성공적으로 로드되었습니다.");
                }
                else
                {
                    Debug.LogError("Color Adjustments를 찾을 수 없습니다. Volume 프로파일을 확인하세요.");
                }
            }

            return colorAdjustments;
        }
    }



    static Vector3 originPos;




    public  static void SetPosition(sbyte x, sbyte y)
    {
        float blockSize = Screen.width / (float)x;

        Vector3 originPos;
        if (x > y)
        {
            float screenHeight = Screen.height / blockSize;
            Main.orthographicSize = screenHeight * 0.5f;
            originPos = Vector3.Lerp(Vector3.zero, new Vector3(x - 1, y, -10), 0.5f);
        }
        else
        {
            Main.orthographicSize = (y + 3) * 0.5f;
            originPos = Vector3.Lerp(new Vector3(0, -2, -10), new Vector3(x - 1, y + 1, -10), 0.5f);
        }

        CameraController.originPos = originPos;
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



    public static void BreakEvent()
    {
        if (ColorAdjustments != null)
        {
            ColorAdjustments.hueShift.value = UnityEngine.Random.Range(-180, 180);
        }
    }
}
