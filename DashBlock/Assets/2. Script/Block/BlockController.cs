using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;

public class BlockController : Singleton
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            CheckLine(new BlockPosition(0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            CheckLine(new BlockPosition(1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CheckLine(new BlockPosition(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            CheckLine(new BlockPosition(0, -1));
        }

#if UNITY_IOS || UNITY_ANDROID
        if (Touchscreen.current == null)
            return;

        var primaryTouch = Touchscreen.current.primaryTouch;

        if (primaryTouch == null)
            return;

        if (primaryTouch.press.wasPressedThisFrame)
        {
            OnTouchStart(primaryTouch.position.ReadValue());
        }

        if (primaryTouch.press.wasReleasedThisFrame)
        {
            OnTouchEnd(primaryTouch.position.ReadValue());
        }
#endif
    }

    void CheckLine(BlockPosition dir)
    {
        if (ActionBlock.IsMoving) return;

        BlockPosition targetPos = ActionBlock.Position;
        int moveDistance = 0;
        Block target = null;
        BlockPosition nextPosition;
        while (true)
        {
            sbyte limit_x = BlockManager.limit_x;
            sbyte limit_y = BlockManager.limit_y;

            nextPosition = targetPos + dir;

            if (nextPosition.x < 0 || nextPosition.x > limit_x || nextPosition.y < 0 || nextPosition.y > limit_y)
            {
                break;
            }

            if (BlockManager.Tiles.TryGetValue(nextPosition, out target))
            {
                break;
            }

            targetPos = nextPosition;
            moveDistance++;
        }

        if (moveDistance < 1)
        {
            ActionBlock.Wiggle();
        }
        else
        {
            if (target == null)
            {
                ActionBlock.Dash(targetPos);
            }
            else
            {
                ActionBlock.Dash(targetPos, target);
            }
        }
    }

    ActionBlock actionBlock;
    public ActionBlock ActionBlock => actionBlock ??= GetComponent<ActionBlock>();

    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    bool pressed = false;

    private void OnTouchStart(Vector2 value)
    {
        pressed = true;

        touchStartPosition = value;
    }

    [SerializeField] private float minSwipeDistance = 50f;
    private void OnTouchEnd(Vector2 value)
    {
        if (!pressed) return;
        pressed = false;

        touchEndPosition = value;
        Vector2 swipeVector = touchEndPosition - touchStartPosition;

        if (swipeVector.magnitude < minSwipeDistance) return;

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            sbyte x = swipeVector.x > 0 ? (sbyte)1 : (sbyte)-1;
            CheckLine(new BlockPosition(x, 0));
        }
        else
        {
            sbyte y = swipeVector.y > 0 ? (sbyte)1 : (sbyte)-1;
            CheckLine(new BlockPosition(0, y));
        }
    }
}