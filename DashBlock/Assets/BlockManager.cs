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
    }
    public ActionBlock actionBlock;
    Block target;
    private Vector2 touchStartPosition;
    private Vector2 touchEndPosition;
    bool pressed = false;

    private void OnTouchStart(Vector2 value)
    {
        Debug.Log("터치 함");

        // 터치 시작 위치 저장
        touchStartPosition = value;
        Debug.Log($"Touch Start Position: {touchStartPosition}");
    }
    [SerializeField] private float minSwipeDistance = 50f; // 최소 스와이프 거리 (픽셀 단위)

    private void OnTouchEnd(Vector2 value)
    {
        Debug.Log("손가락 뗌");
        // 터치 종료 위치 저장
        touchEndPosition = value; // 마지막 위치를 종료 위치로 사용
        Debug.Log($"Touch End Position: {touchEndPosition}");

        // 두 위치 차이 계산
        Vector2 swipeVector = touchEndPosition - touchStartPosition;
        Debug.Log($"Swipe Vector: {swipeVector}");

        if (swipeVector.magnitude < minSwipeDistance)
        {
            Debug.Log("스와이프 거리 부족");
            return;
        }
        Position dir = new();
        if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
        {
            dir.Set(swipeVector.x > 0 ? 1 : -1, 0); // 수평 이동
        }
        else
        {
            dir.Set(0, swipeVector.y > 0 ? 1 : -1); // 수직 이동
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
            Debug.Log("이동 실패");
            actionBlock.Wiggle();
        }
        else
        {
            Debug.Log("이동");
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
