using DG.Tweening;
using UnityEngine;

public class ActionBlock : Block
{
    Ease ease = Ease.InQuart;

    /// <summary>
    /// 벽에 부딪히는 경우
    /// </summary>
    public virtual void Dash(Vector2 targetPosition)
    {
        BlockManager.PlayerBlock.IsMoving = true;
        transform
            .DOMove(targetPosition, 0.1f)
            .SetEase(ease)
            .OnComplete(() =>
            {
                CameraController.Shake(0.3f, 0.4f);
                BlockManager.PlayerBlock.IsMoving = false;
                TakeDamage();
            });
    }


    /// <summary>
    /// 벽돌에 부딪히는 경우
    /// </summary>
    public virtual void Dash(Vector2 targetPosition, Block target)
    {
        BlockManager.PlayerBlock.IsMoving = true;
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
                BlockManager.PlayerBlock.IsMoving = false;
                TakeDamage();
            });
    }



    public virtual void OnFailedMove()
    { }
}
