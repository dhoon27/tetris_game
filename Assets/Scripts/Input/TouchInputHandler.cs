using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputHandler : MonoBehaviour
{
    private float SwipeThreshold => Screen.width * 0.08f;
    private const float HardDropSpeed = 1500f;

    private InputAction _touchPressAction;
    private InputAction _touchPositionAction;

    private Vector2 _touchStartPos;
    private Vector2 _lastTouchPos;
    private float _touchStartTime;
    private bool _isTouching;

    private void OnEnable()
    {
        // InputAction으로 터치 바인딩 (New Input System 정석 방식)
        _touchPressAction = new InputAction("TouchPress", binding: "<Touchscreen>/primaryTouch/press");
        _touchPositionAction = new InputAction("TouchPosition", binding: "<Touchscreen>/primaryTouch/position");

        // 에디터에서 마우스도 사용할 수 있도록 바인딩 추가
        _touchPressAction.AddBinding("<Mouse>/leftButton");
        _touchPositionAction.AddBinding("<Mouse>/position");

        _touchPressAction.Enable();
        _touchPositionAction.Enable();
    }

    private void OnDisable()
    {
        _touchPressAction?.Disable();
        _touchPositionAction?.Disable();
    }

    private void Update()
    {
        // 일시정지 중에는 입력 무시
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        if (_touchPressAction.WasPressedThisFrame())
        {
            _touchStartPos = _touchPositionAction.ReadValue<Vector2>();
            _touchStartTime = Time.time;
            _isTouching = true;
        }

        if (_isTouching && _touchPressAction.IsPressed())
        {
            _lastTouchPos = _touchPositionAction.ReadValue<Vector2>();
        }

        if (_touchPressAction.WasReleasedThisFrame() && _isTouching)
        {
            _isTouching = false;
            HandleTouchEnd(_lastTouchPos);
        }
    }

    private void HandleTouchEnd(Vector2 endPos)
    {
        Vector2 delta = endPos - _touchStartPos;
        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);
        float duration = Time.time - _touchStartTime;

        if (absX < SwipeThreshold && absY < SwipeThreshold)
        {
            OnTap();
            return;
        }

        if (absX > absY)
        {
            if (delta.x > 0)
                GameManager.Instance.TryMovePiece(Vector2Int.right);
            else
                GameManager.Instance.TryMovePiece(Vector2Int.left);
        }
        else if (delta.y < 0)
        {
            float speed = absY / Mathf.Max(duration, 0.01f);
            if (speed >= HardDropSpeed)
                GameManager.Instance.HardDropPiece();
            else
                GameManager.Instance.SoftDropPiece();
        }
    }

    private void OnTap()
    {
        GameManager.Instance.RotatePiece();
    }
}
