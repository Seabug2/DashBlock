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
    /// 각각의 블록이 위치 값을 key로 스스로를 딕셔너리에 추가, 만약 중복된 위치의 블록이 있다면 나중에 등록하려는 쪽을 삭제
    /// </summary>
    /// <param name="key">튜플</param>
    /// <param name="block">블록 객체</param>
    public void Register(BlockPosition key, Block block)
    {
        if (Blocks.ContainsKey(key))
        {
            Debug.Log("등록 실패");
            Destroy(block.gameObject);
        }
        else
        {
            Debug.Log("등록 성공");
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

            if (Blocks.ContainsKey(nextPosition)) //이동하려는 위치에 블록이 존재할 때
            {
                if (moveDistance < 1) break;

                Block target = Blocks[nextPosition];
                if (target.HP > 1) //그 블록의 체력이 1보다 높으면
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

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void LoadScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
