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

    // 1셀에 대응하는 화면 드래그 거리 (화면 너비의 8%)
    private float CellSize => Screen.width * 0.08f;

    // 하드 드롭 판정: 아래로 빠르게 스와이프
    private const float HardDropSpeed = 1500f;

    // 탭 판정: 이동 거리가 이 이하이고 시간이 짧으면 탭
    private float TapThreshold => Screen.width * 0.04f;
    private const float TapMaxDuration = 0.3f;

    private Vector2 _touchStartPos;
    private float _touchStartTime;
    private Image _image;

    // 터치 시작 시점의 블록 위치 (그리드 좌표)
    private Vector2Int _pieceStartPos;
    // 현재까지 적용된 그리드 오프셋
    private int _appliedOffsetX;
    private int _appliedOffsetY;

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
        _hasDragged = false;
        _appliedOffsetX = 0;
        _appliedOffsetY = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused) return;

        Vector2 totalDelta = eventData.position - _touchStartPos;
        float cellSize = CellSize;

        // 터치 시작점 기준으로 몇 칸 이동해야 하는지 계산
        int targetOffsetX = Mathf.RoundToInt(totalDelta.x / cellSize);
        int targetOffsetY = Mathf.RoundToInt(totalDelta.y / cellSize);
        // 위로 드래그는 무시 (아래로만)
        if (targetOffsetY > 0) targetOffsetY = 0;

        // 좌우 이동: 현재 적용된 오프셋과 목표 오프셋의 차이만큼 이동
        while (_appliedOffsetX < targetOffsetX)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TryMovePiece(Vector2Int.right);
            _appliedOffsetX++;
            _hasDragged = true;
        }
        while (_appliedOffsetX > targetOffsetX)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.TryMovePiece(Vector2Int.left);
            _appliedOffsetX--;
            _hasDragged = true;
        }

        // 아래 이동: 소프트 드롭
        while (_appliedOffsetY > targetOffsetY)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.SoftDropPiece();
            _appliedOffsetY--;
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
