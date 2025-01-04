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
            Debug.Log("Color Adjustments�� ���������� �ε�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("Color Adjustments�� ã�� �� �����ϴ�. Volume ���������� Ȯ���ϼ���.");
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
    /// ����� �ı��� ������ ����� ��ƼŬ
    /// </summary>
    public ParticleSystem prtc;

    /// <summary>
    /// �̵����� ������ �� ����
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

    //TODO : �����ε� �ؾ��� ��
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
