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
        // PC ȯ�� (Ű���� ���)
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
        // ����� ȯ�� (��ġ��ũ�� ���)
        // ���� Ȱ��ȭ�� ��ġ ��ũ���� �ִ��� Ȯ��
        if (Touchscreen.current == null)
            return;

        // ù ��° ��ġ�� �����ɴϴ� (��Ƽ ��ġ�� ó���Ϸ��� �߰� ���� �ʿ�)
        var primaryTouch = Touchscreen.current.primaryTouch;

        if (primaryTouch == null)
            return;

        // ��ġ�� ���۵Ǿ��� ��
        if (primaryTouch.press.wasPressedThisFrame)
        {
            OnTouchStart(primaryTouch.position.ReadValue());
        }

        // ��ġ�� ������ ��
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

        // ��ġ ���� ��ġ ����
        touchStartPosition = value;
    }

    [SerializeField] private float minSwipeDistance = 50f; // �ּ� �������� �Ÿ� (�ȼ� ����)

    private void OnTouchEnd(Vector2 value)
    {
        if (!pressed) return;
        pressed = false;

        // ��ġ ���� ��ġ ����
        touchEndPosition = value; // ������ ��ġ�� ���� ��ġ�� ���

        // �� ��ġ ���� ���
        Vector2 swipeVector = touchEndPosition - touchStartPosition;

        if (swipeVector.magnitude < minSwipeDistance) return;

        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            sbyte x = swipeVector.x > 0 ? (sbyte)1 : (sbyte)-1;
            CheckLine(new BlockPosition(x, 0)); // ���� �̵�
        }
        else
        {
            sbyte y = swipeVector.y > 0 ? (sbyte)1 : (sbyte)-1;
            CheckLine(new BlockPosition(0, y)); // ���� �̵�
        }
    }
}
