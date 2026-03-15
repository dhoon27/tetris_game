using UnityEngine;

public class TouchInputHandler : MonoBehaviour
{
    private float SwipeThreshold => Screen.width * 0.08f;
    private const float HardDropSpeed = 1500f;

    private Vector2 _touchStartPos;
    private float _touchStartTime;

    private void Update()
    {
        // 실제 터치가 있으면 터치 처리
        if (Input.touchCount > 0)
        {
            HandleTouch(Input.GetTouch(0));
            return;
        }

// 에디터/시뮬레이터에서는 마우스를 터치처럼 처리
#if UNITY_EDITOR
        HandleMouse();
#endif
    }

    private void HandleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                _touchStartPos = touch.position;
                _touchStartTime = Time.time;
                break;
            case TouchPhase.Ended:
                HandleTouchEnd(touch.position);
                break;
        }
    }

#if UNITY_EDITOR
    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPos = Input.mousePosition;
            _touchStartTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            HandleTouchEnd(Input.mousePosition);
        }
    }
#endif

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
