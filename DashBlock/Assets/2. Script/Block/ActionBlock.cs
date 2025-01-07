using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    Ease ease = Ease.InQuart;

    /// <summary>
    /// ���� �ε����� ���
    /// </summary>
    public virtual void Dash(Vector2 targetPosition)
    {
        transform
            .DOMove(targetPosition, 0.1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage();
            });
    }


    /// <summary>
    /// ������ �ε����� ���
    /// </summary>
    public virtual void Dash(Vector2 targetPosition, Block target)
    {
        if (target.HP == 1)
        {
            targetPosition = target.transform.position;
        }

        transform
            .DOMove(targetPosition, .1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                CameraController.Shake(0.3f, 0.4f);
                TakeDamage();
            });
    }



    public virtual void OnFailedMove()
    { }
}
