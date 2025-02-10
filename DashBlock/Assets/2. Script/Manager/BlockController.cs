using UnityEngine;
using UnityEngine.InputSystem;

public class BlockController : Singleton
{
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"]; // "Move"는 Input Action의 Action 이름

        moveAction.performed += OnMovePerformed;
    }

    private void OnDestroy()
    {
        moveAction.performed -= OnMovePerformed;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Vector2Int direction = Vector2Int.zero;

        if (input.y > 0) direction = new Vector2Int(0, 1); // ↑
        else if (input.y < 0) direction = new Vector2Int(0, -1); // ↓
        else if (input.x > 0) direction = new Vector2Int(1, 0); // →
        else if (input.x < 0) direction = new Vector2Int(-1, 0); // ←

        if (direction != Vector2Int.zero)
        {
            DashBlock.Player.Dash(direction);
        }
    }


    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    void Update()
    {
        if (ActionBlock.IsAnyActionBlockMoving) return;
        //#if !UNITY_ANDROID
        //        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        //        {
        //            DashBlock.Player.Dash(new Vector2Int(0, 1));
        //        }
        //        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        //        {
        //            DashBlock.Player.Dash(new Vector2Int(1, 0));
        //        }
        //        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        //        {
        //            DashBlock.Player.Dash(new Vector2Int(-1, 0));
        //        }
        //        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        //        {
        //            DashBlock.Player.Dash(new Vector2Int(0, -1));
        //        }
        //#endif
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
            int x = swipeVector.x > 0 ? 1 : -1;
            DashBlock.Player.Dash(new Vector2Int(x, 0));
        }
        else
        {
            int y = swipeVector.y > 0 ? 1 : -1;
            DashBlock.Player.Dash(new Vector2Int(0, y));
        }
    }
}