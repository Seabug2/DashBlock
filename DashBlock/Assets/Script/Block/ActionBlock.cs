using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionBlock : Block
{
    AudioSource audioSource;
    public Volume volume; ColorAdjustments colorAdjustments;
    public UnityEvent OnDeadEvent = new();

    private void Awake()
    {
        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments�� ���������� �ε�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("Color Adjustments�� ã�� �� �����ϴ�. Volume ���������� Ȯ���ϼ���.");
        }
        audioSource = GetComponent<AudioSource>();
    }

    public sbyte initialLife = 99;

    protected override void Start()
    {
        Manager.Register(this);
        HP = initialLife;
    }

    public bool IsMoving { get; private set; }

    /// <summary>
    /// ����� �ı��� ������ ����� ��ƼŬ
    /// </summary>
    public ParticleSystem prtc;

    public void Wiggle()
    {
        IsMoving = true;
        audioSource.PlayOneShot(wiggle);
        transform.DOShakePosition(0.3f, .3f, 20).OnComplete(() => IsMoving = false);
    }

    public AudioClip wiggle;
    public AudioClip dash;

    public AnimationCurve curve;

    void Off()
    {
        IsMoving = false;
    }
    public void Dash(Vector2 targetPosition, bool brake)
    {
        IsMoving = true;
        transform.position = targetPosition;
        TakeDamage(1);

        audioSource.PlayOneShot(dash);
        CameraController.Shake(0.3f, 0.4f);
        if (brake)
        {
            //prtc.Play();
            colorAdjustments.hueShift.value = Random.Range(-180, 180);
        }

        Invoke(nameof(Off), 0.08f);

        //transform
        //    .DOMove(target == null ? targetPosition : target.transform.position, .23f)
        //    //.SetEase(curve)
        //    .OnComplete(() =>
        //    {
        //        IsMoving = false;
        //        if (target != null)
        //        {
        //            if (target.TakeDamage(1))
        //            {
        //                transform.position = target.transform.position;
        //                prtc.transform.position = target.transform.position;
        //            }
        //            else
        //            {
        //                target.Punching();
        //            }
        //        }

        //        TakeDamage(1);
        //        audioSource.PlayOneShot(dash);
        //        CameraController.Shake(0.3f, 0.4f);
        //    });
    }
}
