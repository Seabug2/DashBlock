using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public static class CameraController
{
    static ColorAdjustments colorAdjustments;
    static ColorAdjustments ColorAdjustments
    {
        get
        {
            if (colorAdjustments == null)
            {
                Volume volume = Camera.main.GetComponent<Volume>();
                if (volume != null && volume.profile.TryGet(out colorAdjustments))
                {
                    Debug.Log("Color Adjustments�� ���������� �ε�Ǿ����ϴ�.");
                }
                else
                {
                    Debug.LogError("Color Adjustments�� ã�� �� �����ϴ�. Volume ���������� Ȯ���ϼ���.");
                }
            }

            return colorAdjustments;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        ActionBlock.OnCollision += CollisionEffect;
        Block.OnDestroyed += BreakEffect;
    }

    static Vector3 originPos;




    public static void SetPosition(float x, float y)
    {
        Debug.Log($"ī�޶� ��ġ�� �����մϴ�. Screen.width : {Screen.width} / Screen.height : {Screen.height}");

        float blockSize = Screen.width / x;
        Debug.Log($"��� ������ :: {blockSize}");

        if (x > y)
        {
            float screenHeight = Screen.height / blockSize;
            Camera.main.orthographicSize = screenHeight * 0.5f;
            originPos = Vector3.Lerp(Vector3.zero, new Vector3(x - 1, y, -10), 0.5f);
        }
        else
        {
            Camera.main.orthographicSize = (y + 3) * 0.5f;
            originPos = Vector3.Lerp(new Vector3(0, -2, -10), new Vector3(x - 1, y + 1, -10), 0.5f);
        }

        Camera.main.transform.position = originPos;
    }




    public static void Shake(float duration, float power)
    {
        Camera.main.transform.DOKill();
        Camera.main.transform.DOShakePosition(duration * 0.9f, power, 20).OnComplete(() =>
        {
            Camera.main.transform.DOMove(originPos, duration * 0.1f).SetEase(Ease.OutQuad);
        });
    }

    public static void Shake(float duration, float power, Action OnCompleted = null)
    {
        Camera.main.transform.DOKill();
        Camera.main.transform.DOShakePosition(duration * 0.9f, power, 20).OnComplete(() =>
        {
            Camera.main.transform.DOMove(originPos, duration * 0.1f).SetEase(Ease.OutQuad)
            .OnComplete(() => OnCompleted?.Invoke());
        });
    }



    public static void CollisionEffect(Block _)
    {
        Shake(0.34f, 0.56f);
    }

    public static void BreakEffect(Block _)
    {
        Shake(0.45f, 0.67f);
        ColorAdjustments.hueShift.value = UnityEngine.Random.Range(-180, 180);
    }
}
