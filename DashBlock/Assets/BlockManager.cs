using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BlockManager : Singleton
{
    [SerializeField] int row;
    [SerializeField] int column;

    public Block[,] Blocks { get; set; }

    protected override void Awake()
    {
        base.Awake();
        Blocks = new Block[row, column];
    }

    void Update()
    {
        if (actionBlock.IsMoving)
        {
            return;
        }

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
    }
    public ActionBlock actionBlock;
    Block target;
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
        Position dir = new();
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            dir.Set(swipeVector.x > 0 ? 1 : -1, 0); // ���� �̵�
        }
        else
        {
            dir.Set(0, swipeVector.y > 0 ? 1 : -1); // ���� �̵�
        }

        Position targetPos = actionBlock.GetPos();
        int moveRange = 0;
        target = null;
        while (true)
        {
            int x = targetPos.x + dir.x;
            int y = targetPos.y + dir.y;

            if (x < 0 || x >= row || y < 0 || y >= column)
            {
                break;
            }

            if (Blocks[x, y] != null)
            {
                if (moveRange > 0)
                    target = Blocks[x, y];
                break;
            }

            targetPos.Set(x, y);
            moveRange++;
        }

        if (moveRange < 1)
        {
            Debug.Log("�̵� ����");
            actionBlock.Wiggle();
        }
        else
        {
            Debug.Log("�̵�");
            actionBlock.Slide(new Vector3(targetPos.x, targetPos.y, 0), target);
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
