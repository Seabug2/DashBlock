using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionBlock : Block
{
    AudioSource audioSource;
    ColorAdjustments colorAdjustments;

    protected override void Init()
    {
        HP = initialLife;

        BlockManager.ActionBlock = this;
        audioSource = Camera.main.gameObject.AddComponent<AudioSource>();
        Volume volume = FindObjectOfType<Volume>();
        if (volume != null && volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments가 성공적으로 로드되었습니다.");
        }
        else
        {
            Debug.LogError("Color Adjustments를 찾을 수 없습니다. Volume 프로파일을 확인하세요.");
        }
    }

    public bool IsMoving { get; private set; }

    [Header("블록에 부딪혔을 때 재생할 파티클"), Space(10)]
    public ParticleSystem bronkenPrtc;
    public ParticleSystem colisionPrtc;

    /// <summary>
    /// 이동하지 못했을 때 실행
    /// </summary>
    public void Wiggle()
    {
        IsMoving = true;
        audioSource.PlayOneShot(wiggle);
        transform.DOShakePosition(0.3f, .3f, 20).OnComplete(() => IsMoving = false);
    }

    public AudioClip wiggle;
    public AudioClip dash;

    public AnimationCurve curve;
    public Ease ease;

    /// <summary>
    /// 벽에 부딪히는 경우
    /// </summary>
    public void Dash(Vector2 targetPosition)
    {
        IsMoving = true;

        transform
            .DOMove(targetPosition, 0.1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                IsMoving = false;
                audioSource.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage();
            });
    }


    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public void Dash(Vector2 targetPosition, Block target)
    {
        IsMoving = true;

        if (target.HP == 1)
        {
            targetPosition = target.transform.position;
        }

        transform
            .DOMove(targetPosition, .1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                if (target.TakeDamage())
                {
                    ShockWaveObject.CallShockWave(transform.position);
                    bronkenPrtc.Play();
                    colorAdjustments.hueShift.value = Random.Range(-180, 180);
                }
                else
                {
                    colisionPrtc.transform.position = Vector3.Lerp(transform.position, target.transform.position, .5f);
                    colisionPrtc.Play();
                    target.Punching();
                }

                IsMoving = false;
                audioSource.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage();
            });
    }

    protected override void OnBlockDestroyed()
    {
        bronkenPrtc.transform.SetParent(null);
        colisionPrtc.transform.SetParent(null);
        bronkenPrtc.Play();

        gameObject.SetActive(false);
    }
}
