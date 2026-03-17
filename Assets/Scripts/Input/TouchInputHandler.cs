using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 전체 화면 투명 패널에 부착하여 드래그/탭 입력을 감지.
/// 드래그 중 손가락 이동 거리에 따라 블록이 연속으로 이동함.
/// </summary>
public class TouchInputHandler : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static TouchInputHandler Instance { get; private set; }

    // 1셀 이동에 필요한 드래그 거리 (화면 너비의 8%)
    private float CellDragThreshold => Screen.width * 0.08f;

    // 하드 드롭 판정: 아래로 빠르게 스와이프
    private const float HardDropSpeed = 1500f;

    // 탭 판정: 이동 거리가 이 이하이고 시간이 짧으면 탭
    private float TapThreshold => Screen.width * 0.04f;
    private const float TapMaxDuration = 0.3f;

    // 소프트 드롭 1칸에 필요한 드래그 거리 (화면 높이의 5%)
    private float DropDragThreshold => Screen.height * 0.05f;

    private Vector2 _touchStartPos;
    private float _touchStartTime;
    private Image _image;

    // 드래그 누적 거리 (좌우/상하 각각 추적)
    private float _dragAccumX;
    private float _dragAccumY;
    private Vector2 _lastDragPos;

    // 드래그 중 이동이 발생했는지 (탭 판정용)
    private bool _hasDragged;

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
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        _touchStartPos = eventData.position;
        _touchStartTime = Time.unscaledTime;
        _lastDragPos = eventData.position;
        _dragAccumX = 0f;
        _dragAccumY = 0f;
        _hasDragged = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        Vector2 currentPos = eventData.position;
        Vector2 delta = currentPos - _lastDragPos;
        _lastDragPos = currentPos;

        _dragAccumX += delta.x;
        _dragAccumY += delta.y;

        // 좌우 이동: 누적 거리가 1셀 임계값을 넘을 때마다 1칸 이동
        float cellThreshold = CellDragThreshold;
        while (_dragAccumX >= cellThreshold)
        {
            GameManager.Instance.TryMovePiece(Vector2Int.right);
            _dragAccumX -= cellThreshold;
            _hasDragged = true;
        }
        while (_dragAccumX <= -cellThreshold)
        {
            GameManager.Instance.TryMovePiece(Vector2Int.left);
            _dragAccumX += cellThreshold;
            _hasDragged = true;
        }

        // 아래로 드래그: 소프트 드롭
        float dropThreshold = DropDragThreshold;
        while (_dragAccumY <= -dropThreshold)
        {
            GameManager.Instance.SoftDropPiece();
            _dragAccumY += dropThreshold;
            _hasDragged = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        Vector2 endPos = eventData.position;
        Vector2 totalDelta = endPos - _touchStartPos;
        float duration = Time.unscaledTime - _touchStartTime;

        // 탭 판정: 이동 거리가 작고 시간이 짧으면 회전
        float totalDistance = totalDelta.magnitude;
        if (totalDistance < TapThreshold && duration < TapMaxDuration)
        {
            GameManager.Instance.RotatePiece();
            return;
        }

        // 하드 드롭 판정: 아래로 빠르게 스와이프
        if (totalDelta.y < 0 && Mathf.Abs(totalDelta.y) > Mathf.Abs(totalDelta.x))
        {
            float speed = Mathf.Abs(totalDelta.y) / Mathf.Max(duration, 0.01f);
            if (speed >= HardDropSpeed)
            {
                GameManager.Instance.HardDropPiece();
            }
        }
    }
}
