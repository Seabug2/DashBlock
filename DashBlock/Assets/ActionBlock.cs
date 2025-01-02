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
    public UnityEvent OnDeadEvent = new();
    public Volume volume; ColorAdjustments colorAdjustments;
    AudioSource audioSource;
    AudioSource AudioSource => audioSource ??= GetComponent<AudioSource>();
    private void Awake()
    {
        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments가 성공적으로 로드되었습니다.");
        }
        else
        {
            Debug.LogError("Color Adjustments를 찾을 수 없습니다. Volume 프로파일을 확인하세요.");
        }
    }
    protected override void Start()
    {
        Manager.actionBlock = this;
        tmp.text = hp.ToString();
    }

    public bool IsMoving { get; private set; }

    public ParticleSystem prtc;
    public void Wiggle()
    {
        IsMoving = true;
        AudioSource.PlayOneShot(wiggle);
        transform.DOShakePosition(0.3f, .3f, 20).OnComplete(() => IsMoving = false);
    }
    public AudioClip wiggle;
    public AudioClip dash;
    public void Slide(Vector2 targetPosition, Block target = null)
    {
        IsMoving = true;
        transform
            .DOMove(targetPosition, .23f)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                IsMoving = false;
                if (target != null)
                {
                    if (target.TakeDamage(1))
                    {
                        transform.position = target.transform.position;
                        prtc.transform.position = target.transform.position;
                        prtc.Play();
                        colorAdjustments.hueShift.value = Random.Range(-180, 180);
                    }
                    else
                    {
                        target.Punching();
                    }
                }

                TakeDamage(1);
                AudioSource.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
            });
    }

    private void OnDestroy()
    {
        OnDeadEvent.Invoke();
    }
}
