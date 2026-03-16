using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 전체 화면 투명 패널에 부착하여 스와이프/탭 입력을 감지.
/// EventSystem(IPointerDown/Up)을 사용하므로 Android에서 안정적으로 동작.
/// </summary>
public class TouchInputHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static TouchInputHandler Instance { get; private set; }

    private float SwipeThreshold => Screen.width * 0.08f;
    private const float HardDropSpeed = 1500f;

    private Vector2 _touchStartPos;
    private float _touchStartTime;
    private Image _image;

    private void Awake()
    {
        Instance = this;
        _image = GetComponent<Image>();
    }

    /// <summary>
    /// 오버레이 패널(Pause/GameOver)이 활성화될 때 호출.
    /// raycastTarget을 끄면 이 패널이 터치를 가로채지 않아
    /// 오버레이의 버튼들이 정상 동작함.
    /// </summary>
    public void SetInputActive(bool active)
    {
        if (_image != null)
            _image.raycastTarget = active;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 일시정지 중에는 입력 무시
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        _touchStartPos = eventData.position;
        _touchStartTime = Time.unscaledTime;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 일시정지 중에는 입력 무시
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        HandleTouchEnd(eventData.position);
    }

    private void HandleTouchEnd(Vector2 endPos)
    {
        Vector2 delta = endPos - _touchStartPos;
        float absX = Mathf.Abs(delta.x);
        float absY = Mathf.Abs(delta.y);
        float duration = Time.unscaledTime - _touchStartTime;

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
