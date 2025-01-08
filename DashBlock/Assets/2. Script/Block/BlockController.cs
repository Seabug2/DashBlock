using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;

public class BlockController : Singleton
{
    PlayerBlock PlayerBlock => BlockManager.PlayerBlock;

    void Update()
    {
        if (ActionBlock.IsMoving) return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            PlayerBlock.CheckLine(new BlockPosition(0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            PlayerBlock.CheckLine(new BlockPosition(1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            PlayerBlock.CheckLine(new BlockPosition(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            PlayerBlock.CheckLine(new BlockPosition(0, -1));
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
            PlayerBlock.CheckLine(new BlockPosition(x, 0));
        }
        else
        {
            sbyte y = swipeVector.y > 0 ? (sbyte)1 : (sbyte)-1;
            PlayerBlock.CheckLine(new BlockPosition(0, y));
        }
    }
}