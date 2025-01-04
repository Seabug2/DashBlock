using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BlockManager : Singleton
{
    [Range(4, 100)]
    [SerializeField] sbyte limit_x;
    [Range(4, 100)]
    [SerializeField] sbyte limit_y;
    [SerializeField] ShockWave shockWave;
    public ShockWave ShockWave => shockWave;
    public readonly Dictionary<BlockPosition, Block> Blocks = new();

    /// <summary>
    /// ������ ����� ��ġ ���� key�� �����θ� ��ųʸ��� �߰�, ���� �ߺ��� ��ġ�� ����� �ִٸ� ���߿� ����Ϸ��� ���� ����
    /// </summary>
    /// <param name="key">Ʃ��</param>
    /// <param name="block">��� ��ü</param>
    public void Register(BlockPosition key, Block block)
    {
        if (Blocks.ContainsKey(key))
        {
            Debug.Log("��� ����");
            Destroy(block.gameObject);
        }
        else
        {
            Debug.Log("��� ����");
            Blocks.Add(key, block);
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (ActionBlock.IsMoving) return;

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
        BlockPosition targetPos = ActionBlock.GetPos();
        int moveDistance = 0;
        bool broken = false;
        while (true)
        {
            BlockPosition nextPosition = targetPos + dir;

            if (nextPosition.x < 0 || nextPosition.x >= limit_x || nextPosition.y < 0 || nextPosition.y >= limit_y)
            {
                break;
            }

            if (Blocks.ContainsKey(nextPosition)) //�̵��Ϸ��� ��ġ�� ����� ������ ��
            {
                if (moveDistance < 1) break;

                Block target = Blocks[nextPosition];
                if (target.HP > 1) //�� ����� ü���� 1���� ������
                {
                    target.HP -= 1;
                    target.Punching();
                    break;
                }
                else
                {
                    broken = true;
                    Blocks.Remove(nextPosition);
                    Destroy(target.gameObject);
                }
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
            ActionBlock.Dash(targetPos, broken);
        }
    }

    public ActionBlock ActionBlock { get; private set; }

    public void Register(ActionBlock ActionBlock)
    {
        if (this.ActionBlock == null)
        {
            this.ActionBlock = ActionBlock;
        }
        else
        {
            Destroy(ActionBlock.gameObject);
        }
    }

    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    bool pressed = false;

    private void OnTouchStart(Vector2 value)
    {
        Debug.Log("��ġ ��");

        // ��ġ ���� ��ġ ����
        touchStartPosition = value;
        Debug.Log($"Touch Start Position: {touchStartPosition}");
    }

    [SerializeField] private float minSwipeDistance = 50f; // �ּ� �������� �Ÿ� (�ȼ� ����)

    private void OnTouchEnd(Vector2 value)
    {
        Debug.Log("�հ��� ��");
        // ��ġ ���� ��ġ ����
        touchEndPosition = value; // ������ ��ġ�� ���� ��ġ�� ���
        Debug.Log($"Touch End Position: {touchEndPosition}");

        // �� ��ġ ���� ���
        Vector2 swipeVector = touchEndPosition - touchStartPosition;
        Debug.Log($"Swipe Vector: {swipeVector}");

        if (swipeVector.magnitude < minSwipeDistance)
        {
            Debug.Log("�������� �Ÿ� ����");
            return;
        }

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

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
