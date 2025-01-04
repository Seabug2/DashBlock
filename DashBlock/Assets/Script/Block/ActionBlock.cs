using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionBlock : Block
{
    SingleAudio singleAudio;
    public Volume volume; ColorAdjustments colorAdjustments;

    protected override void Awake()
    {
        if (volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments가 성공적으로 로드되었습니다.");
        }
        else
        {
            Debug.LogError("Color Adjustments를 찾을 수 없습니다. Volume 프로파일을 확인하세요.");
        }

        Locator.TryGetUI(out singleAudio);
    }

    public sbyte initialLife = 99;

    protected override void Start()
    {
        HP = initialLife;
    }

    public bool IsMoving { get; private set; }

    /// <summary>
    /// 블록을 파괴할 때마다 재생할 파티클
    /// </summary>
    public ParticleSystem prtc;

    /// <summary>
    /// 이동하지 못했을 때 실행
    /// </summary>
    public void Wiggle()
    {
        IsMoving = true;
        singleAudio.PlayOneShot(wiggle);
        transform.DOShakePosition(0.3f, .3f, 20).OnComplete(() => IsMoving = false);
    }

    public AudioClip wiggle;
    public AudioClip dash;

    public AnimationCurve curve;

    //TODO : 오버로딩 해야할 듯
    public void Dash(Vector2 targetPosition, Block target)
    {
        transform.DOKill();
        IsMoving = true;

        bool isBroken = (target == null) ? false : ((target.HP - 1) == 0);

        transform
            .DOMove(isBroken ? target.transform.position : targetPosition, .1f)
            .SetEase(curve)
            .OnComplete(() =>
            {
                if (target != null)
                {
                    if (target.TakeDamage(1))
                    {
                        ShockWaveObject.CallShockWave(GetPos());
                        prtc.transform.position = transform.position;
                        prtc.Play();
                        colorAdjustments.hueShift.value = Mathf.Clamp(colorAdjustments.hueShift.value + Random.Range(30, 60), -180, 180);
                    }
                    else
                    {
                        target.Punching();
                    }
                }

                TakeDamage(1);
                singleAudio.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);

                IsMoving = false;
            });
    }
}
