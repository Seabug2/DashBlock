using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BlockController : Singleton
{
    BlockPosition limit;

    private void Start()
    {
        limit = BlockManager.GetLimit();
        Debug.Log(limit);
    }

    void Update()
    {
        // PC 환경 (키보드 사용)
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
        // 모바일 환경 (터치스크린 사용)
        // 현재 활성화된 터치 스크린이 있는지 확인
        if (Touchscreen.current == null)
            return;

        // 첫 번째 터치를 가져옵니다 (멀티 터치를 처리하려면 추가 로직 필요)
        var primaryTouch = Touchscreen.current.primaryTouch;

        if (primaryTouch == null)
            return;

        // 터치가 시작되었을 때
        if (primaryTouch.press.wasPressedThisFrame)
        {
            OnTouchStart(primaryTouch.position.ReadValue());
        }

        // 터치가 끝났을 때
        if (primaryTouch.press.wasReleasedThisFrame)
        {
            OnTouchEnd(primaryTouch.position.ReadValue());
        }
#endif
    }

    void CheckLine(BlockPosition dir)
    {
        if (ActionBlock.IsMoving) return;

        BlockPosition targetPos = ActionBlock.GetPos();
        int moveDistance = 0;
        Block target = null;

        while (true)
        {
            BlockPosition nextPosition = targetPos + dir;

            if (nextPosition.x < 0 || nextPosition.x > limit.x || nextPosition.y < 0 || nextPosition.y > limit.y)
            {
                break;
            }

            if (BlockManager.TryGet(nextPosition, out target))
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
            ActionBlock.Dash(targetPos, target);
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

        // 터치 시작 위치 저장
        touchStartPosition = value;
    }

    [SerializeField] private float minSwipeDistance = 50f; // 최소 스와이프 거리 (픽셀 단위)

    private void OnTouchEnd(Vector2 value)
    {
        if (!pressed) return;
        pressed = false;

        // 터치 종료 위치 저장
        touchEndPosition = value; // 마지막 위치를 종료 위치로 사용

        // 두 위치 차이 계산
        Vector2 swipeVector = touchEndPosition - touchStartPosition;

        if (swipeVector.magnitude < minSwipeDistance) return;

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            sbyte x = swipeVector.x > 0 ? (sbyte)1 : (sbyte)-1;
            CheckLine(new BlockPosition(x, 0)); // 수평 이동
        }
        else
        {
            sbyte y = swipeVector.y > 0 ? (sbyte)1 : (sbyte)-1;
            CheckLine(new BlockPosition(0, y)); // 수평 이동
        }
    }
}
