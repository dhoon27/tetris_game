using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class TouchInputHandler : MonoBehaviour
{
    private float SwipeThreshold => Screen.width * 0.08f;
    private const float HardDropSpeed = 1500f;

    private Vector2 _touchStartPos;
    private float _touchStartTime;

    private void Update()
    {
        // 일시정지 중에는 입력 무시
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        // 모바일 터치 처리 (Old Input API — Android에서 가장 안정적)
        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == UnityEngine.TouchPhase.Began)
            {
                _touchStartPos = touch.position;
                _touchStartTime = Time.time;
            }
            else if (touch.phase == UnityEngine.TouchPhase.Ended)
            {
                HandleTouchEnd(touch.position);
            }
            return;
        }

// 에디터에서는 마우스를 터치처럼 처리
#if UNITY_EDITOR
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            _touchStartPos = mouse.position.ReadValue();
            _touchStartTime = Time.time;
        }
        else if (mouse.leftButton.wasReleasedThisFrame)
        {
            HandleTouchEnd(mouse.position.ReadValue());
        }
#endif
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
