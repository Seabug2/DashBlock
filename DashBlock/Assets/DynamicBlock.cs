using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SlideBlock : Block
{
    public override bool TakeDamage()
    {
        if (HP > 1)
        {
            Vector2 targetPosition = transform.position - ActionBlock.instance.transform.position;
            targetPosition = targetPosition.normalized;
            Dash(targetPosition);
        }
        return --HP <= 0;
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
        ActionBlock.instance.IsMoving = true;

        transform
            .DOMove(targetPosition, 0.1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                ActionBlock.instance.IsMoving = false;
                //audioSource.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage();
            });
    }


    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public void Dash(Vector2 targetPosition, Block target)
    {
        ActionBlock.instance.IsMoving = true;

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
                    //ActionBlock.isntance.bronkenPrtc.Play();
                    //colorAdjustments.hueShift.value = Random.Range(-180, 180);
                }
                else
                {
                    //ActionBlock.isntance.colisionPrtc.transform.position = Vector3.Lerp(transform.position, target.transform.position, .5f);
                    //ActionBlock.isntance.colisionPrtc.Play();
                }

                ActionBlock.instance.IsMoving = false;
                //audioSource.PlayOneShot(dash);
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage();
            });
    }
}
