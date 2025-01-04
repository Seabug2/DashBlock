using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ActionBlock : Block
{
    SingleAudio singleAudio;

    ColorAdjustments colorAdjustments;

    protected override void Awake()
    {
        BlockManager.ActionBlock = this;

        Volume volume = FindObjectOfType<Volume>();
        if (volume != null && volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.Log("Color Adjustments�� ���������� �ε�Ǿ����ϴ�.");
        }
        else
        {
            Debug.LogError("Color Adjustments�� ã�� �� �����ϴ�. Volume ���������� Ȯ���ϼ���.");
        }
    }

    protected override void Start()
    {
        base.Start();
        Locator.TryGet(out singleAudio);
    }

    public bool IsMoving { get; private set; }

    [Header("��Ͽ� �ε����� �� ����� ��ƼŬ"), Space(10)]
    public ParticleSystem bronkenPrtc;
    public ParticleSystem colisionPrtc;

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
    public Ease ease;

    public void Dash(Vector2 targetPosition)
    {
        IsMoving = true;

        transform
            .DOMove(targetPosition, 0.1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                IsMoving = false;
                TakeDamage(1);
                singleAudio.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
            });
    }

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
                IsMoving = false;
                if (target.TakeDamage(1))
                {
                    ShockWaveObject.CallShockWave(transform.position);
                    bronkenPrtc.transform.position = transform.position;
                    bronkenPrtc.Play();
                    colorAdjustments.hueShift.value = Mathf.Clamp(colorAdjustments.hueShift.value + Random.Range(30, 60), -180, 180);
                }
                else
                {
                    colisionPrtc.transform.position = Vector3.Lerp(transform.position, target.transform.position, .5f);
                    colisionPrtc.Play();
                    target.Punching();
                }

                singleAudio.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage(1);
            });
    }

    public override bool TakeDamage(sbyte i = 1)
    {
        HP -= i;
        if (HP <= 0)
        {
            bronkenPrtc.transform.position = transform.position;
            bronkenPrtc.Play();
            gameObject.SetActive(false);
            return true;
        }
        else
        {
            tmp.text = HP.ToString();
            return false;
        }
    }
}
