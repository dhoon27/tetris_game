using UnityEngine;

// 모바일 터치 입력을 감지해 게임 조작으로 변환하는 클래스
public class TouchInputHandler : MonoBehaviour
{
    // 스와이프로 인정할 최소 거리 (화면 너비의 8% 이상 움직여야 스와이프)
    // 화면 크기에 비례하므로 기기마다 동일한 느낌을 줌
    private float SwipeThreshold => Screen.width * 0.08f;

    // 빠른 아래 스와이프를 하드 드롭으로 인정할 속도 (px/sec)
    private const float HardDropSpeed = 1500f;

    private Vector2 _touchStartPos;
    private float _touchStartTime;

    private void Update()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

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

    private void HandleTouchEnd(Vector2 endPos)
    {
        Vector2 delta = endPos - _touchStartPos;
        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);
        float duration = Time.time - _touchStartTime;

        // 스와이프 임계값 미만 → 탭으로 판정
        if (absX < SwipeThreshold && absY < SwipeThreshold)
        {
            OnTap();
            return;
        }

        // 가로 스와이프가 더 크면 → 좌우 이동
        if (absX > absY)
        {
            if (delta.x > 0)
                GameManager.Instance.TryMovePiece(Vector2Int.right);
            else
                GameManager.Instance.TryMovePiece(Vector2Int.left);
        }
        // 세로 스와이프 → 아래면 드롭
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
